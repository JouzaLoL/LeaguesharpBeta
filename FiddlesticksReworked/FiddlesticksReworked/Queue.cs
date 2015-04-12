using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;

namespace FiddlesticksReworked
{
    internal class Queuer
    {
        public static List<String> Queue = new List<String>();
        private static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        public static Vector3 RPos;
        public static Obj_AI_Base Target;

        public static void DoQueue()
        {
            if (Queue.Count == 0) return;

            switch (Queue[0])
            {
                case "Q":
                    Queue_Q();
                    break;
                case "W":
                    Queue_W();
                    break;
                case "E":
                    Queue_E();
                    break;
                case "R":
                    Queue_R();
                    break;
                case "Flash":
                    Queue_Flash();
                    break;
            }
        }

        public static void Add(String spell)
        {
            Queue.Add(spell);
        }

        public static void Add(String spell, Obj_AI_Base target)
        {
            Queue.Add(spell);
            Target = target;
        }

        public static void Add(String spell, Vector3 pos)
        {
            Queue.Add(spell);
            RPos = pos;

        }

        public static void Remove(String spell)
        {
            if (Queue.Count == 0 || Queue[0] != spell) return;

            Queue.RemoveAt(0);
        }


        private static void Queue_Q()
        {

            if (!SpellHandler.Spells[SpellHandler._spells.Q].IsReady())
            {
                Queue.Remove("Q");
                return;
            }

            if (SpellHandler.Spells[SpellHandler._spells.Q].IsReady())
            {

                SpellHandler.CastQ(Target);
            }
        }

        private static void Queue_W()
        {
            if (!SpellHandler.Spells[SpellHandler._spells.Q].IsReady())
            {
                Queue.Remove("W");
                return;
            }

            if (SpellHandler.Spells[SpellHandler._spells.Q].IsReady())
            {
                SpellHandler.CastW(Target);
            }
        }

        private static void Queue_E()
        {
            if (!SpellHandler.Spells[SpellHandler._spells.E].IsReady())
            {
                Queue.Remove("E");
                return;
            }

            if (SpellHandler.Spells[SpellHandler._spells.E].IsReady())
            {
                SpellHandler.CastE(Target);
            }
        }

        private static void Queue_R()
        {
            if (!SpellHandler.Spells[SpellHandler._spells.R].IsReady() || CheckHandler.Ulting())
            {
                Queue.Remove("R");
                return;
            }

            if (SpellHandler.Spells[SpellHandler._spells.R].IsReady())
            {
                SpellHandler.CastR(Target.Position);
            }
        }

        private static void Queue_Flash()
        {
            if (!SpellHandler.SummonerDictionary[SpellHandler.SummonerSpell.Flash].IsReady())
            {
                Queue.Remove("Flash");
                return;
            }
            SpellHandler.CastFlash(Target.Position);
        }
    }
}