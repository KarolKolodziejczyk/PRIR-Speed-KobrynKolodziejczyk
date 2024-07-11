using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speed
{
    internal class SpeedGameApp
    {
        private SpeedGameWindow GUI;

        public SpeedGameApp(SpeedGameWindow gameWindow)
        {
            this.GUI = gameWindow;
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
