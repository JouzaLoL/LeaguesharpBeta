using LeagueSharp.Common;

namespace FiddlesticksReworked
{
    public class MenuHandler
    {
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;

        public static void CreateMenu()
        {
            Config = new Menu("[Jouza] Fiddlesticks Reworked", "fiddlesticksreworked", true);

            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalking", "fr.orbwalking")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "fr.targetselector")));

            var combo = Config.AddSubMenu(new Menu("[FR] Combo Settings", "fr.combo"));
            {
                combo.AddItem(new MenuItem("combo.mode", "Combo Mode")).SetValue(new StringList(new[] {"Smart", "Normal"}));
                combo.AddItem(new MenuItem("combo.smartFear", "Smart Fear")).SetValue(true);

                foreach (var spell in SpellHandler.Spells.Keys)
                {
                    combo.AddItem(new MenuItem("combo.use" + spell, "Use " + spell + " in Combo")).SetValue(true);
                }

                foreach (var summoner in SpellHandler.SummonerDictionary.ToString())
                {
                    combo.AddItem(new MenuItem("combo.use" + summoner, "Use " + summoner + " in Combo")).SetValue(true);
                }                
            }

            var harass = Config.AddSubMenu(new Menu("[FR] Harass Settings", "fr.harass"));
            {
                harass.AddItem(new MenuItem("harass.useE", "Use E in Harass")).SetValue(true);
            }

            var killsteal = Config.AddSubMenu(new Menu("[FR] Killsteal settings", "fr.killsteal"));
            {
                killsteal.AddItem(new MenuItem("killsteal.useE", "Use E in Killsteal")).SetValue(true);
            }

            var farm = Config.AddSubMenu(new Menu("[FR] Farm Settings", "fr.farm"));
            {
                farm.AddItem(new MenuItem("farm.useE", "Use E in Farm")).SetValue(true);
            }

            var laneclear = Config.AddSubMenu(new Menu("[FR] Laneclear Settings", "fr.laneclear"));
            {
                laneclear.AddItem(new MenuItem("laneclear.useE", "Use E in Laneclear")).SetValue(true);
            }

            var jungleclear = Config.AddSubMenu(new Menu("[FR] Jungleclear Settings", "fr.jungleclear"));
            {
                jungleclear.AddItem(new MenuItem("jungleclear.useW", "Use W in Jungleclear")).SetValue(true);
                jungleclear.AddItem(new MenuItem("jungleclear.useE", "Use E in Jungleclear")).SetValue(true);
            }

            var resmanager = Config.AddSubMenu(new Menu("[FR] Resource Manager", "fr.resmanager"));
            {
                resmanager.AddItem()
            }

            var misc = Config.AddSubMenu(new Menu("[FR] Misc Settings", "fr.misc"));
            {
                misc.AddItem()
            }

        }

        public static bool GetBool(string item)
        {
            return MenuHandler.Config.Item(item).GetValue<bool>();
        }

        public static StringList GetStringList(string item)
        {
            return MenuHandler.Config.Item(item).GetValue<StringList>();
        }
    }
}