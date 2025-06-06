﻿using System;
using System.Collections.Generic;

namespace COServer.Game.MsgServer.AttackHandler
{
    public class Sector
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.ScatterFire:
                        {
                            //if (user.Player.Map != 1039)
                            //        if (DateTime.Now > user.Player.LastMove.AddMilliseconds(1500))
                            //            return;
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.Sector SpellSector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);

                            uint Experience = 0;
                              foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                               {
                                   MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (COServer.Role.Core.CanSee(attacked.X, attacked.Y, user.Player.X, user.Player.Y, 18))
                                {
                                    if (SpellSector.Inside(attacked.X, attacked.Y))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                               }


                               foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                               {
                                   var attacked = targer as Role.Player;
                                   if (SpellSector.Inside(attacked.X, attacked.Y))
                                   {
                                       if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                       {
                                           MsgSpellAnimation.SpellObj AnimationObj;
                                           Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                           ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                           AnimationObj.Hit = 0;
                                           MsgSpell.Targets.Enqueue(AnimationObj);
                                       }
                                   }

                               }
                               foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                               {
                                   var attacked = targer as Role.SobNpc;
                                   if (SpellSector.Inside(attacked.X, attacked.Y))
                                   {
                                       if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                       {
                                           MsgSpellAnimation.SpellObj AnimationObj;
                                           Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                           AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                           Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                           MsgSpell.Targets.Enqueue(AnimationObj);
                                       }
                                   }

                               }

                            //while (MsgSpell.Targets.Count > 30)
                            //    MsgSpell.Targets.Dequeue();

                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    default:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            Algoritms.Sector SpellSector = new Algoritms.Sector(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            SpellSector.Arrange(DBSpell.Sector, DBSpell.Range);
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (SpellSector.Inside(attacked.X, attacked.Y)
                                    && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 5)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        //MsgSpell.TargetSend(user, Attack, Experience, DBSpells, true, stream);

                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (SpellSector.Inside(attacked.X, attacked.Y) && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 5)
                                {
                                    if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        //MsgSpell.TargetSend(user, Attack, Experience, DBSpells, true, stream);
                                    }
                                }

                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (SpellSector.Inside(attacked.X, attacked.Y) && Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < 5)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                        //MsgSpell.TargetSend(user, Attack, Experience, DBSpells,true, stream);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }

    }
}
