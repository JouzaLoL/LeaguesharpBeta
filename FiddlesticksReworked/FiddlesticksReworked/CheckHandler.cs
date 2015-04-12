using LeagueSharp;
using LeagueSharp.Common;

namespace FiddlesticksReworked
{
    public class CheckHandler
    {
        public static Obj_AI_Base Player { get { return ObjectManager.Player; } }

        public static bool Ulting, Draining, TargetFeared, QReady, WReady, EReady, RReady;

        public static void Init()
        {
            
        }

        public static void CheckHandler_OnUpdate()
        {
            Ulting = Player.HasBuff("Crowstorm");
            Draining = Player.HasBuff("Drain");
            TargetFeared = Player.HasBuff("Fear");
            QReady = SpellHandler.Spells[SpellHandler._spells.Q].IsReady();
            WReady = SpellHandler.Spells[SpellHandler._spells.W].IsReady();
            EReady = SpellHandler.Spells[SpellHandler._spells.E].IsReady();
            RReady = SpellHandler.Spells[SpellHandler._spells.R].IsReady();
        }



        internal static void Interrupter(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!MenuHandler.GetBool("misc.interrupt") || !SpellHandler.Spells[SpellHandler._spells.Q].IsReady()) return;
            if (sender.IsValidTarget() && SpellHandler.Spells[SpellHandler._spells.Q].IsInRange(sender))
                SpellHandler.Spells[SpellHandler._spells.Q].CastOnUnit(sender, MenuHandler.PacketCast);
        }

        internal static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;

            if (!sender.IsMe) return;

            if (spell.Name.ToLower() == "") return;
        }
    }
}