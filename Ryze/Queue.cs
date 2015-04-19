using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace SmartRyze
{
    internal class Queuer
    {
        public static List<String> Queue = new List<string>();
        public static Obj_AI_Base Target;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static void DoQueue()
        {
            if (Queue.Count == 0) return;

            switch (Queue[0])
            {
                case "Q":
                    Q_Q(Target);
                    break;
                case "W":
                    Q_W(Target);
                    break;
                case "E":
                    Q_E(Target);
                    break;
                case "R":
                    Q_R();
                    break;
            }
            Console.WriteLine(Queue[0]);
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

        public static void Remove(String spell)
        {
            if (Queue.Count == 0 || Queue[0] != spell) return;

            Queue.RemoveAt(0);
        }

        private static void Q_Q(Obj_AI_Base target)
        {
            if (!Program.Q.IsReady() || !Program.Q.IsInRange(target))
            {
                Queue.Remove("Q");
                return;
            }

            if (Program.Q.IsReady())
            {

                Program.Q.Cast(target);
            }
        }

        private static void Q_W(Obj_AI_Base target)
        {
            if (!Program.W.IsReady() || !Program.W.IsInRange(target))
            {
                Queue.Remove("W");
                return;
            }

            if (Program.Config.Item("combo.nochainW").GetValue<bool>())
            {
                if (!target.IsImmovable)
                {
                    Program.W.CastOnUnit(target);
                }
            }
            else
            {
                Program.W.CastOnUnit(target);
            }
        }

        private static void Q_E(Obj_AI_Base target)
        {

            if (!Program.E.IsReady() || !Program.E.IsInRange(target))
            {
                Queue.Remove("E");
                return;
            }

            if (Program.E.IsReady())
            {
                Program.E.CastOnUnit(target);
            }
        }

        private static void Q_R()
        {
            if (!Program.R.IsReady() || !(Target.Distance(Player) < 1000))
            {
                Queue.Remove("R");
                return;
            }

            if (Program.R.IsReady())
            {
                Program.R.Cast();
            }
        }
    }
}