using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace FiddlesticksReworked
{
    public static class SpellHandler
    {

        public enum _spells
        {
            Q,
            W,
            E,
            R
        }

        public enum SummonerSpell
        {
            Flash,
            Ignite,
            Smite
        }

        public static Dictionary<_spells, Spell> Spells = new Dictionary<_spells, Spell>
        {
            {_spells.Q, new Spell(SpellSlot.Q, 123)},
            {_spells.W, new Spell(SpellSlot.W, 123)},
            {_spells.E, new Spell(SpellSlot.E, 123)},
            {_spells.R, new Spell(SpellSlot.R, 123)}
        };

        public static Dictionary<SummonerSpell, SpellSlot> SummonerDictionary = new Dictionary<SummonerSpell, SpellSlot>
        {
            {SummonerSpell.Flash, Player.GetSpellSlot("SummonerFlash")},
            {SummonerSpell.Ignite, Player.GetSpellSlot("SummonerDot")}
        };

        public static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public static void CastQ(Obj_AI_Base target)
        {
            if (target == null || !Spells[_spells.Q].IsReady()) return;
            Spells[_spells.Q].CastOnUnit(target, MenuHandler.PacketCast);
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (target == null || !Spells[_spells.W].IsReady()) return;
            Spells[_spells.W].CastOnUnit(target, MenuHandler.PacketCast);
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (target == null || !Spells[_spells.E].IsReady()) return;
            Spells[_spells.E].CastOnUnit(target, MenuHandler.PacketCast);
        }

        public static void CastR(Vector3 position)
        {
            if (position == null || !Spells[_spells.R].IsReady()) return;
            Spells[_spells.R].Cast(Queuer.Target.Position, MenuHandler.PacketCast);
        }

        public static void CastFlash(Vector3 position)
        {
            if (position == null || !SummonerDictionary[SummonerSpell.Flash].IsReady()) return;
            Player.Spellbook.CastSpell(SummonerDictionary[SummonerSpell.Flash], position);
        }
    }
}