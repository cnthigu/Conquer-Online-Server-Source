﻿using System.Collections.Generic;

namespace COServer.Game.MsgServer.AttackHandler
{
    public class Recruit
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {

                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.HealingRain:
                    case (ushort)Role.Flags.SpellID.Nectar:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
, 0, Attack.X, Attack.Y, ClientSpell.ID
, ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Damage = 0;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, user.Status.MaxHitpoints, (uint)user.Player.HitPoints);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, Damage));
                                user.Player.HitPoints += (int)Damage;
                            }

                            else if (user.Team != null)
                            {
                                foreach (var Member in user.Team.Temates)
                                {
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, Member.client.Player.X, Member.client.Player.Y) < 5 /*DBSpell.Range*/)
                                    {
                                        Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, (uint)Member.client.Status.MaxHitpoints, (uint)(Member.client.Player.HitPoints));
                                        Member.client.Player.HitPoints += (int)Damage;
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(Member.client.Player.UID, Damage));

                                    }
                                }
                            }
                            /* else
                             {
                                 if (user.Player.MyGuild != null)
                                 {
                                     Role.IMapObj target;
                                     if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                     {
                                         var attacked = target as Role.Player;
                                         Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, attacked.Owner.Status.MaxHitpoints, (uint)attacked.HitPoints);
                                         attacked.HitPoints += (int)Damage;
                                         MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, Damage));

                                     }
                                 }
                             }*/
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)DBSpell.Damage, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Cure:
                    case (ushort)Role.Flags.SpellID.SpiritHealing:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
     , 0, Attack.X, Attack.Y, ClientSpell.ID
     , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Damage = 0;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, user.Status.MaxHitpoints, (uint)user.Player.HitPoints);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, Damage));
                                user.Player.HitPoints += (int)Damage;
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                {
                                    var attacked = target as Role.Player;
                                    Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, attacked.Owner.Status.MaxHitpoints, (uint)attacked.HitPoints);
                                    attacked.HitPoints += (int)Damage;
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, Damage));

                                }
                                else user.OnAutoAttack = false;
                            }
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)DBSpell.Damage, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    default:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
    , 0, Attack.X, Attack.Y, ClientSpell.ID
    , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Damage = 0;
                            if (user.Player.UID == Attack.OpponentUID)
                            {
                                Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, user.Status.MaxHitpoints, (uint)user.Player.HitPoints);
                                MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, Damage));
                                user.Player.HitPoints += (int)Damage;
                            }
                            else
                            {
                                Role.IMapObj target;
                                if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                                {
                                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                    Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, (uint)attacked.Family.MaxHealth, (uint)(attacked.HitPoints + DBSpell.Damage));
                                    attacked.HitPoints += Damage;
                                    MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(attacked.UID, Damage));

                                }
                            }
                            if (user.Team != null)
                            {
                                foreach (var Member in user.Team.Temates)
                                {
                                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, Member.client.Player.X, Member.client.Player.Y) < DBSpell.Range)
                                    {
                                        Damage = Calculate.Base.CalculateHealtDmg((uint)DBSpell.Damage, (uint)Member.client.Status.MaxHitpoints, (uint)(Member.client.Player.HitPoints));
                                        Member.client.Player.HitPoints += (int)Damage;
                                        MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(Member.client.Player.UID, Damage));

                                    }
                                }
                            }
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, (uint)DBSpell.Damage, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }

                }


            }
        }
    }
}
