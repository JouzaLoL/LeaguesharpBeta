using System;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;

namespace FiddlesticksReworked
{
    class Program
    {

        public static Obj_AI_Hero Player;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += delegate
            {
                // Loading the Assembly Async
                var onGameStart = new Thread(Game_OnGameStart);
                onGameStart.Start();
            };
        }

        private static void Game_OnGameStart()
        {
            if (ObjectManager.Player.ChampionName != "Fiddlesticks") return;

            CheckHandler.Init();
            MenuHandler.CreateMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnUpdate += eventArgs => StateHandler.Tick();
            Obj_AI_Base.OnProcessSpellCast += CheckHandler.Obj_AI_Hero_OnProcessSpellCast;
            Drawing.OnDraw += DrawHandler.OnDraw;
            Interrupter2.OnInterruptableTarget += CheckHandler.Interrupter;
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            OnUpdateFunctions();
        }

        private static void OnUpdateFunctions()
        {
            CheckHandler.CheckHandler_OnUpdate();
            StateHandler.OnUpdate();

        }
    }
}
