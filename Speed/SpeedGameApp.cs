using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speed.Backend;

namespace Speed
{
    internal class SpeedGameApp
    {
        private SpeedGameWindow GUI;

        Game game; 
        public SpeedGameApp(SpeedGameWindow gameWindow, string IP)
        {
            this.GUI = gameWindow;
            game = new Game(IP);
        }

        public int OnCardChosen()
        {
            // 0 - no changes
            // 1 - reload view

            return 0;
        }

        public void OnGameStart()
        {
            //
        }

        public void OnStop()
        {
            //
        }

        public void OnSurrender()
        {
            //
        }
    }
}
