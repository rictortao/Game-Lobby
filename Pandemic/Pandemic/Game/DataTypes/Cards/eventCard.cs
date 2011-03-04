using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pandemic.Game.DataTypes.Cards
{
    class eventCard : playerCard
    {
        string title;

        public eventCard(string inTitle) : base(inTitle)
        {
            title = inTitle;
        }

        public void performAction()
        {
            switch (title)
            {
                case "Forecast":
                    forecast();
                    break;
                case "Airlift":
                    airlift();
                    break;
                case "Resilient Population":
                    resilPop();
                    break;
                case "One Quiet Night":
                    onequietNight();
                    break;
                case "Government Grant":
                    govGrant();
                    break;
            }
        }

        #region /* Event Actions */
        private void forecast()
        {
        }

        private void airlift()
        {
        }

        private void resilPop()
        {
        }

        private void onequietNight()
        {
        }

        private void govGrant()
        {
        }
        #endregion

    }
}
