#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace SmartRyze
{
    internal class Program
    {
        public const string CharName = "Ryze";
        public static Orbwalking.Orbwalker Orbwalker;
        public static List<Spell> Spells = new List<Spell>();
        public static Spell Q, W, E, R;
        public static SpellSlot IgniteSlot;
        public static Menu Config;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        // Custom vars
        public static bool PacketCast;
        public static bool DebugEnabled;
        // Items
        public static Items.Item Biscuit = new Items.Item(2010, 10);
        public static Items.Item HPpot = new Items.Item(2003, 10);
        public static Items.Item Flask = new Items.Item(2041, 10);
        public static Items.Item SEmbrace = new Items.Item(3048);

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != CharName) return;

            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R);

            Spells.Add(Q);
            Spells.Add(W);
            Spells.Add(E);
            Spells.Add(R);

            IgniteSlot = Player.GetSpellSlot("summonerdot");

            Config = new Menu("Smart Ryze", "ryze", true);

            //Orbwalker Menu
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Target Selector Menu
            var tsMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(tsMenu);
            Config.AddSubMenu(tsMenu);

            //Combo Menu
            Config.AddSubMenu(new Menu("[SR] Combo Settings", "ryze.combo"));
            Config.SubMenu("ryze.combo")
                .AddItem(new MenuItem("combo.mode", "Combo Mode").SetValue(new StringList(new[] {"Burst", "Long"})));
            Config.SubMenu("ryze.combo").AddItem(new MenuItem("combo.useR", "Use R in Combo").SetValue(true));
            Config.SubMenu("ryze.combo").AddItem(new MenuItem("combo.smartAA", "Smart AA").SetValue(true));
            Config.SubMenu("ryze.combo").AddItem(new MenuItem("combo.nochainW", "Don't W if already immobile").SetValue(true));

            //Killsteal
            Config.AddSubMenu(new Menu("[SR] Killsteal Settings", "ryze.killsteal"));
            Config.SubMenu("ryze.killsteal")
                .AddItem(new MenuItem("killsteal.enabled", "Smart KS Enabled").SetValue(true));
            Config.SubMenu("ryze.killsteal")
                .AddItem(new MenuItem("killsteal.useIgnite", "KS with Ignite").SetValue(true));

            //Harass Menu
            Config.AddSubMenu(new Menu("[SR] Harass Settings", "ryze.harass"));
            Config.SubMenu("ryze.harass").AddItem(new MenuItem("harass.useW", "Use W in Harass")).SetValue(true);

            //Farm Menu
            Config.AddSubMenu(new Menu("[SR] Farming Settings", "ryze.farm"));
            Config.SubMenu("ryze.farm").AddItem(new MenuItem("farm.useQ", "Farm with Q").SetValue(true));
            Config.SubMenu("ryze.farm").AddItem(new MenuItem("farm.useW", "Farm with W").SetValue(true));
            Config.SubMenu("ryze.farm").AddItem(new MenuItem("farm.useE", "Farm with E").SetValue(true));

            //Laneclear Menu
            Config.AddSubMenu(new Menu("[SR] Laneclear Settings", "ryze.laneclear"));
            Config.SubMenu("ryze.laneclear").AddItem(new MenuItem("laneclear.useQ", "Laneclear with Q").SetValue(true));
            Config.SubMenu("ryze.laneclear").AddItem(new MenuItem("laneclear.useW", "Laneclear with W").SetValue(true));
            Config.SubMenu("ryze.laneclear").AddItem(new MenuItem("laneclear.useE", "Laneclear with E").SetValue(true));

            //Jungle Clear Menu
            Config.AddSubMenu(new Menu("[SR] Jungle Clear Settings", "ryze.jungle"));
            Config.SubMenu("ryze.jungle").AddItem(new MenuItem("jungle.useQ", "Clear with Q").SetValue(true));
            Config.SubMenu("ryze.jungle").AddItem(new MenuItem("jungle.useW", "Clear with W").SetValue(true));
            Config.SubMenu("ryze.jungle").AddItem(new MenuItem("jungle.useE", "Clear with E").SetValue(true));

            //Drawing Menu
            Config.AddSubMenu(new Menu("[SR] Draw Settings", "ryze.drawing"));
            Config.SubMenu("ryze.drawing").AddItem(new MenuItem("drawing.disableAll", "Disable drawing").SetValue(false));
            Config.SubMenu("ryze.drawing").AddItem(new MenuItem("drawing.drawComboMode", "Draw Combo Mode").SetValue(false));
            Config.SubMenu("ryze.drawing").AddItem(new MenuItem("drawing.drawKillability", "Draw Killability").SetValue(false));
            Config.SubMenu("ryze.drawing").AddItem(new MenuItem("drawing.target", "Highlight Target").SetValue(new Circle(true,Color.FromArgb(255, 255, 255, 0))));
            Config.SubMenu("ryze.drawing").AddItem(new MenuItem("drawing.drawQ", "Draw Q Range").SetValue(new Circle(true,Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("ryze.drawing").AddItem(new MenuItem("drawing.drawW", "Draw W Range").SetValue(new Circle(true,Color.FromArgb(255, 255, 255, 255))));
            Config.SubMenu("ryze.drawing").AddItem(new MenuItem("drawing.drawDamage.enabled", "Draw Damage").SetValue(true));
            Config.SubMenu("ryze.drawing").AddItem(new MenuItem("drawing.drawDamage.fill", "Draw Damage Fill Color").SetValue(new Circle(true,Color.FromArgb(90, 255, 169, 4))));

            //Misc Menu
            Config.AddSubMenu(new Menu("[SR] Misc Settings", "ryze.misc"));
            Config.SubMenu("ryze.misc")
                .AddItem(new MenuItem("misc.interruptGapclosers", "Interrupt Gapclosers").SetValue(true));
            Config.SubMenu("ryze.misc")
                .AddItem(new MenuItem("misc.usePackets", "Use Packets to Cast Spells").SetValue(true));
            Config.SubMenu("ryze.misc").AddItem(new MenuItem("misc.debug", "Enable debug").SetValue(false));
            Config.SubMenu("ryze.misc")
                .AddItem(new MenuItem("misc.autoSEmbrace.enabled", "Auto Serapths Embrace").SetValue(true));
            Config.SubMenu("ryze.misc")
                .AddItem(new MenuItem("misc.autoSEmbrace.percent", "SEmbrace HP %").SetValue(new Slider(10)));
            Config.SubMenu("ryze.misc").AddItem(new MenuItem("misc.autoR.enabled", "Auto R when HP low").SetValue(true));
            Config.SubMenu("ryze.misc")
                .AddItem(new MenuItem("misc.autoR.percent", "Auto R HP %").SetValue(new Slider(10)));

            //Resources manager
            Config.AddSubMenu(new Menu("[SR] Resources Manager", "ryze.resmanager"));
            Config.SubMenu("ryze.resmanager")
                .AddItem(new MenuItem("resmanager.enabled", "Resource Manager enabled").SetValue(true));
            //Config.SubMenu("ryze.resmanager").AddItem(new MenuItem("resmanager.preserveforCombo", "Preserve mana for Combo").SetValue(true));
            Config.SubMenu("ryze.resmanager")
                .AddItem(new MenuItem("resmanager.harass", "Harass Mana %").SetValue(new Slider(50)));
            Config.SubMenu("ryze.resmanager")
                .AddItem(new MenuItem("resmanager.farm", "Farm Mana %").SetValue(new Slider(50)));
            Config.SubMenu("ryze.resmanager")
                .AddItem(new MenuItem("resmanager.laneclear", "Laneclear Mana %").SetValue(new Slider(50)));
            Config.SubMenu("ryze.resmanager").AddItem(new MenuItem("resmanager.hp", "Health Pot").SetValue(true));
            Config.SubMenu("ryze.resmanager")
                .AddItem(new MenuItem("resmanager.hp.percent", "Health Pot %").SetValue(new Slider(35, 1)));
            Config.SubMenu("ryze.resmanager").AddItem(new MenuItem("resmanager.mp", "Mana Pot").SetValue(true));
            Config.SubMenu("ryze.resmanager")
                .AddItem(new MenuItem("resmanager.mp.percent", "Mana Pot %").SetValue(new Slider(35, 1)));
            Config.SubMenu("ryze.resmanager")
                .AddItem(new MenuItem("resmanager.antiignite", "Auto pot if Ignited / Morde R").SetValue(true));

            //Make menu visible
            Config.AddToMainMenu();

            PacketCast = Config.Item("misc.usePackets").GetValue<bool>();
            DebugEnabled = Config.Item("misc.debug").GetValue<bool>();

            //Damage Drawing
            DamageIndicator.DamageToUnit = ComboDamage;
            DamageIndicator.Enabled = GetConfigBool("drawing.drawDamage.enabled");
            DamageIndicator.Fill = Config.Item("drawing.drawDamage.fill").GetValue<Circle>().Active;
            DamageIndicator.FillColor = Config.Item("drawing.drawDamage.fill").GetValue<Circle>().Color;

            Config.Item("drawing.drawDamage.enabled").ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };
            Config.Item("drawing.drawDamage.fill").ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };

            //Set up Events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

            //Announce that the assembly has been loaded
            Game.PrintChat("<font color=\"#00BFFF\">Smart Ryze -</font> <font color=\"#FFFFFF\">Loaded</font>");
            Game.PrintChat(
                "<font color=\"#00BFFF\">Smart Ryze -</font> <font color=\"#FFFFFF\">Version: " + Assembly.GetExecutingAssembly().GetName().Version+ "</font>");
            Game.PrintChat(
                "<font color=\"#00BFFF\">Smart Ryze -</font> <font color=\"#FFFFFF\">Thank you for using my scripts, feel free to suggest features and report bugs on the forums.</font>");
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe) return;

            //Removing spells from Queue when they are casted.
            if (spell.Name.ToLower() == "overload") Queuer.Remove("Q");
            if (spell.Name.ToLower() == "runeprison") Queuer.Remove("W");
            if (spell.Name.ToLower() == "spellflux") Queuer.Remove("E");
            if (spell.Name.ToLower() == "desperatepower") Queuer.Remove("R");
        }

        private static void Game_OnUpdate(EventArgs args)
        {

            if (!Orbwalking.CanMove(40)) return;
            if (Player.IsDead) return;

            //Spells Queue
            if (Queuer.Queue.Count > 0) Queuer.DoQueue();

            //Main features with Orbwalker
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboHandler();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    JungleClear();
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Farm();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }

            OnUpdateFunctions();

        }

        private static void OnUpdateFunctions()
        {
            ResManager();
            KillSteal();
            AutoR();
            AutoSEmbrace();
            //AutoW();
        }

        //No AA in Combo
        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {

            if (GetConfigBool("combo.smartAA") && Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.Combo))
                args.Process = !(Q.IsReady() || W.IsReady() || E.IsReady());
        }        

        //Config Bool shortcut
        static bool GetConfigBool(string c)
        {
            return Config.Item(c).GetValue<bool>();
        }

        //Auto Functions
        private static void AutoSEmbrace()
        {
            var sEmbracePercent = (Config.Item("misc.autoSEmbrace.percent").GetValue<Slider>().Value/100);

            //if (!GetConfigBool("misc.autoSEmbrace.enabled")) return;
            if (!Items.HasItem(SEmbrace.Id)) return;
            if (Player.HealthPercent < sEmbracePercent)
            {
                Items.UseItem(SEmbrace.Id);
            }
        }

        //Auto R
        private static void AutoR()
        {
            var rPercent = Config.Item("misc.autoR.percent").GetValue<Slider>().Value/100;

            if (!GetConfigBool("misc.autoR.enabled")) return;
            if (R.IsReady() && Player.HealthPercent <= rPercent) R.Cast(PacketCast);
        }

        //Drawing
        private static void Drawing_OnDraw(EventArgs args)
        {
            //Main drawig.ng switch
            if (GetConfigBool("drawing.disableAll")) return;

            //Spells drawing
            foreach (
                var spell in
                    Spells.Where(spell => Config.Item("drawing.draw" + spell.Slot).GetValue<Circle>().Active))
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position, spell.Range, spell.IsReady() ? Color.Green : Color.Red);
            }

            //Target Drawing
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Config.Item("Target").GetValue<Circle>().Active && target != null)
            {
                Render.Circle.DrawCircle(target.Position, 50, Config.Item("Target").GetValue<Circle>().Color);
            }

            //Combo Mode Drawing
            var heropos = Drawing.WorldToScreen(Queuer.Player.Position);
            var comboMode = Config.Item("combo.mode").GetValue<StringList>().SelectedIndex.ToString();
            if (Config.Item("drawing.drawComboMode").GetValue<bool>())
            {
                Drawing.DrawText(heropos[0] - 70, heropos[1] + 40, Color.Green, "Combo Mode: " + comboMode);
            }

            //Killability
            if (target == null) return;
            var targetpos = Drawing.WorldToScreen(Queuer.Target.Position);
            if (Config.Item("drawing.drawKillability").GetValue<bool>())
            {
            Drawing.DrawText(targetpos[0], targetpos[1], (target.Health < ComboDamage(target)
                        ? Color.Green
                        : Color.Red),
                    (target.Health < ComboDamage(target))
                        ? "Killable!"
                        : "Unkillable.");
            }           
        }

        //Anti Gapcloser
        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!GetConfigBool("misc.interruptGapclosers")) return;
            if (!gapcloser.Sender.IsValidTarget(W.Range)) return;

            W.CastOnUnit(gapcloser.Sender, PacketCast);

            if (DebugEnabled) Console.WriteLine("Debug - W Casted to interrupt GAPCLOSER");
        }

        //Killsteal
        private static void KillSteal()
        {
            if (!GetConfigBool("killsteal.enabled")) return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() &&
                target.Health < Q.GetDamage(target) &&
                Q.IsInRange(target))
            {
                Q.Cast(target, PacketCast);
                if (DebugEnabled) Console.WriteLine("Debug - Q casted to KILLSTEAL.");
            }

            if (E.IsReady() &&
                target.Health < E.GetDamage(target) &&
                target.IsValidTarget(W.Range))
            {
                E.Cast(target, PacketCast);
                if (DebugEnabled) Console.WriteLine("Debug - Q casted to KILLSTEAL.");
            }

            if (W.IsReady() &&
                target.Health < W.GetDamage(target) &&
                target.IsValidTarget(Q.Range))
            {
                W.Cast(target, PacketCast);
                if (DebugEnabled) Console.WriteLine("Debug - Q casted to KILLSTEAL.");
            }

            if (IgniteSlot == SpellSlot.Unknown ||
                Player.Spellbook.CanUseSpell(IgniteSlot) != SpellState.Ready ||
                target.IsValidTarget(600)) return;
            if ((Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) < target.Health)) return;

            Player.Spellbook.CastSpell(IgniteSlot, target);
            if (DebugEnabled) Console.WriteLine("Debug - Ignite casted to KILLSTEAL.");
        }

        //Resource Manager
        private static void ResManager()
        {
            if (!GetConfigBool("resmanager.enabled")) return;

            //Auto Ignite Counter
            if (GetConfigBool("resmanager.ignite"))
            {
                if (Player.HasBuff("summonerdot") || Player.HasBuff("MordekaiserChildrenOfTheGrave"))
                {
                    if (!Player.InFountain())
                    {
                        if (Items.HasItem(Biscuit.Id) && Items.CanUseItem(Biscuit.Id) &&
                            !Player.HasBuff("ItemMiniRegenPotion"))
                        {
                            Biscuit.Cast(Player);
                            if (DebugEnabled) Console.WriteLine("Debug - Biscuit used to counter IGNITE.");
                        }
                        else if (Items.HasItem(HPpot.Id) && Items.CanUseItem(HPpot.Id) &&
                                 !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("Health Potion"))
                        {
                            HPpot.Cast(Player);
                            if (DebugEnabled) Console.WriteLine("Debug - HP Pot used to counter IGNITE.");
                        }
                        else if (Items.HasItem(Flask.Id) && Items.CanUseItem(Flask.Id) &&
                                 !Player.HasBuff("ItemCrystalFlask"))
                        {
                            Flask.Cast(Player);
                            if (DebugEnabled) Console.WriteLine("Debug - Flask used to counter IGNITE.");
                        }
                    }
                }
            }

            if (ObjectManager.Player.HasBuff("Recall") || Player.InFountain() && Player.InShop()) return;

            //Health Pots
            if (!GetConfigBool("resmanager.hp")) return;
            if (Player.HealthPercent <= Config.Item("resmanager.hp.percent").GetValue<Slider>().Value/100.0 &&
                !Player.HasBuff("RegenerationPotion", true))
            {
                Items.UseItem(2003);
                if (DebugEnabled) Console.WriteLine("Debug - HP Pot used because of LOW HP");
            }

            //Mana Pots
            if (!GetConfigBool("resmanager.mp")) return;
            if (Player.ManaPercent <= Config.Item("resmanager.mp.percent").GetValue<Slider>().Value/100.0 &&
                !Player.HasBuff("FlaskOfCrystalWater", true))
            {
                Items.UseItem(2004);
                if (DebugEnabled) Console.WriteLine("Debug - MP Pot used because of LOW MP");
            }
        }

        //Combo Handler
        private static void ComboHandler()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            //Smart R
            if (GetConfigBool("combo.useR") && R.IsReady())
            {
                if (target.MoveSpeed > Player.MoveSpeed && target.Distance(Player, false) < 1500) R.Cast(PacketCast);
            }

            //Smart W
            if (Player.Distance(target, false) < 575 && !target.IsFacing(Player) && W.IsReady())
            {
                Queuer.Add("W", target);
            }
            else if (target.IsInvulnerable)
			{
				Queuer.Add("W", target);
				return;
			}

            var comboMode = Config.Item("combo.mode").GetValue<StringList>().SelectedIndex;

            switch (comboMode)
            {
                case 0:
                {
                    Combo_Burst(target);
                    break;
                }
                case 1:
                {
                    Combo_Long(target);
                    break;
                }
            }
        }

        private static void Combo_Burst(Obj_AI_Base target)
        {
            if (target.IsValid)
            {
            Queuer.Add("Q", target);
            Queuer.Add("W", target);
            Queuer.Add("E", target);
            Queuer.Add("Q", target);
            }
            else if (Math.Abs(Player.PercentCooldownMod) >= 0.2)
            {
                Queuer.Add("Q", target);
                Queuer.Add("W", target);
                Queuer.Add("E", target);
                Queuer.Add("Q", target);
            }
            
        }

        private static void Combo_Long(Obj_AI_Base target)
        {
            Queuer.Add("Q", target);
            Queuer.Add("W", target);
            Queuer.Add("Q", target);
            Queuer.Add("E", target);
            Queuer.Add("Q", target);
            Queuer.Add("R");
            Queuer.Add("Q", target);
        }

        //Harass
        private static void Harass()
        {
            if (Queuer.Queue.Count > 0) return;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var harassMana = Player.MaxMana*(Config.Item("resmanager.harass").GetValue<Slider>().Value/100.0);

            if (Player.Mana < harassMana) return;
            if (target == null) return;

            if (Q.IsReady() && Q.IsInRange(target)) Queuer.Add("Q", target);
            if (GetConfigBool("harass.useW") && W.IsReady() && W.IsInRange(target))
            {
                Queuer.Add("W", target);
                Queuer.Add("Q", target);
            }
        }

        //Farm
        private static void Farm()
        {
            if (Queuer.Queue.Count > 0) return;
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);
            var farmMana = Player.MaxMana*(Config.Item("resmanager.farm").GetValue<Slider>().Value/100.0);
            if (Player.Mana < farmMana) return;

            if (GetConfigBool("farm.useQ") && Q.IsReady())
            {
                foreach (var minion in
                    minions.Where(
                        minion =>
                            minion != null && minion.IsValidTarget(Q.Range) &&
                            HealthPrediction.GetHealthPrediction(minion, (int) (Player.Distance(minion.Position))) <=
                            Player.GetSpellDamage(minion, SpellSlot.Q)))
                {
                    Q.CastOnUnit(minion, PacketCast);
                    return;
                }
            }

            if (GetConfigBool("farm.useE") && E.IsReady())
            {
                foreach (var minion in
                    minions.Where(
                        minion =>
                            minion != null && minion.IsValidTarget(E.Range) &&
                            HealthPrediction.GetHealthPrediction(minion, (int) (Player.Distance(minion.Position))) <=
                            Player.GetSpellDamage(minion, SpellSlot.E)))
                {
                    Queuer.Add("E", minion);
                    return;
                }
            }

            if (GetConfigBool("farm.useW") && W.IsReady())
            {
                foreach (var minion in
                    minions.Where(
                        minion =>
                            minion != null && minion.IsValidTarget(W.Range) &&
                            HealthPrediction.GetHealthPrediction(minion, (int) (Player.Distance(minion.Position))) <=
                            Player.GetSpellDamage(minion, SpellSlot.W)))
                {
                    Queuer.Add("W", minion);
                    return;
                }
            }
        }

        // Laneclear
        private static void LaneClear()
        {
            //if (Queuer.Queue.Count > 0) return;
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy,MinionOrderTypes.MaxHealth);
            var laneclearMana = Player.MaxMana*(Config.Item("resmanager.laneclear").GetValue<Slider>().Value/100.0);
            if (Player.Mana < laneclearMana) return;

            foreach (var minion in minions)
            {
                if (GetConfigBool("laneclear.useQ"))
                    Q.CastOnUnit(minion, PacketCast);

                if (GetConfigBool("laneclear.useW"))
                    W.CastOnUnit(minion, PacketCast);

                if (GetConfigBool("laneclear.useE"))
                    E.CastOnUnit(minion, PacketCast);
            }
        }

        //Jungleclear
        private static void JungleClear()
        {
            if (Queuer.Queue.Count > 0) return;
            var mobs = MinionManager.GetMinions(
                Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count <= 0) return;

            var mob = mobs[0];
            if (mob == null) return;

            Combo_Burst(mob);
        }

        //Combo Damage calculating
        private static float ComboDamage(Obj_AI_Base target)
        {
            var dmg = 0d;

            if (Q.IsReady())
            {
                dmg += 3*Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (W.IsReady())
            {
                dmg += Player.GetSpellDamage(target, SpellSlot.W);
            }

            if (E.IsReady())
            {
                dmg += Player.GetSpellDamage(target, SpellSlot.E);
            }

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                dmg += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite);
            }

            return (float) dmg;
        }
    }
}