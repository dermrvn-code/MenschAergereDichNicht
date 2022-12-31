using MenschAergerDichNicht.GameClasses;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MenschAergerDichNicht
{
    public partial class CheatWindow : Window
    {
        public CheatWindow()
        {
            InitializeComponent();
        }

        public void OnSpecificRoll(object sender, EventArgs args)
        {
            Button btn = sender as Button;
            int value = Int32.Parse(btn.Content.ToString());
            GameManager.Instance.Dice.RollNumber(value);
        }

        public void ChangePlayer(object sender, EventArgs args)
        {
            Button btn = sender as Button;
            int value = Int32.Parse(btn.Content.ToString());
            int playerId = value - 1;

            GameManager.Instance.CurrentPlayer.HasPlayed = true;
            GameManager.Instance.CheatMenu_SetCurrentPlayer(GameManager.Instance.Players[playerId]);
            GameManager.Instance.UpdateTextBar(GameManager.Instance.Players[playerId].Name + ", du bist dran!", GameManager.Instance.Players[playerId].Icon, GameManager.Instance.Players[playerId].Color);
        }

        public void ChangeJump(object sender, EventArgs args)
        {
            GameManager.Instance.Dice.RollNumber(1);

            Button btn = sender as Button;
            int value;
            try
            {
                value = Int32.Parse(rollInput.Text);
            }
            catch{ return; }

            GameManager.Instance.Dice.CheatMenu_SetLastRolled(value);
        }
    }
}
