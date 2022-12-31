using MenschAergerDichNicht.GameClasses;
using MenschAergerDichNicht.Helper;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MenschAergerDichNicht.DrawableClasses
{
    public class Dice
    {
        public int LastRolled { get { return lastRolled; } }
        private int lastRolled;

        public bool HasUnusedRoll { get; set; }

        public bool Clickable { get { return clickable; } set { clickable = value; } }
        private bool clickable;

        private Image displayImage;
        private Button rollButton;
        private string diceFacesFolder = "Assets/Images/Dice/";
        private ImageSource[] diceFaces = new ImageSource[6];
        private ImageSource diceRollFace;

        private GameManager gameManager;

        private bool isActive = true;

        public Dice(Button rollButton, Image image)
        {
            gameManager = GameManager.Instance;

            this.rollButton = rollButton;
            displayImage = image;
            Clickable = true;
            lastRolled = -1;

            diceRollFace = new BitmapImage(new Uri(diceFacesFolder + "roll.png", UriKind.Relative));

            for (int diceFaceId = 0; diceFaceId < 6; diceFaceId++)
            {
                diceFaces[diceFaceId] = new BitmapImage(new Uri(diceFacesFolder + (diceFaceId + 1) + ".png", UriKind.Relative));
            }

            rollButton.Click += RollEvent;
        }

        public void ResetFace()
        {
            displayImage.Source = diceRollFace;
        }

        public void RollEvent(object sender, EventArgs args)
        {
            if (!Clickable) return;
            Roll();
        }

        public void Roll()
        {
            if (!isActive) return;
            if (gameManager.State != GameManager.GameState.Rolling) return;

            Random random = new Random();
            int diceRoll = random.Next(0, 6);

            RollNumber(diceRoll + 1);
        }

        public async void RollNumber(int number)
        {
            isActive = false;
            if (number < 0 || number > 6) return;

            double animationDurationInMs = AnimationHelper.AnimateRotation(rollButton);

            await Task.Delay((int)animationDurationInMs);

            
            HasUnusedRoll = true;
            gameManager.CurrentPlayer.OpenRolls--;
            lastRolled = number;
            displayImage.Source = diceFaces[number - 1];
            isActive = true;
        }

        public void CheatMenu_SetLastRolled(int number)
        {
            lastRolled = number;
        }

    }
}
