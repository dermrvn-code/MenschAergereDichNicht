using MenschAergerDichNicht.GameClasses;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MenschAergerDichNicht
{

    public partial class GameWindow : Window
    {
        private GameManager gameManager;
        private CheatWindow cheat;

        public MainWindow main { get; set; }

        private ObservableCollection<Player> scoreboardList = new ObservableCollection<Player>();
        public ObservableCollection<Player> ScoreboardList
        {
            get { return scoreboardList; }
            set { scoreboardList = value; }
        }

        public GameWindow()
        {
            InitializeComponent();
            gameManager = GameManager.Instance;


            // INIT GAME
            Loaded += delegate
            {
                gameManager.Init(this);
                gameManager.StartGame();
            };


            DataContext = this;

        }



        // DISPLAY CLICK ON CANVAS
        private void cnv_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {

            Canvas canvas = (Canvas)sender;

            // CATCH CLICK ON ELLIPSE
            if (args.OriginalSource is Ellipse)
            {
                Ellipse ClickedPlayer = (Ellipse)args.OriginalSource;

                if (ClickedPlayer.Name.Contains("p") && ClickedPlayer.Name.Contains("f"))
                {
                    string[] splitted = ClickedPlayer.Name.Replace("p", "").Split("f");
                    int playerId = int.Parse(splitted[0]);
                    int figureId = int.Parse(splitted[1]);

                    gameManager.Players[playerId].Figures[figureId].Click();
                }
            }
            else if (args.OriginalSource is Rectangle)
            {
                Rectangle ClickedField = (Rectangle)args.OriginalSource;
                string name = ClickedField.Name;

                if (name.Contains("normal"))
                {
                    int id = int.Parse(name.Replace("normal", ""));
                    gameManager.Board.Fields[id].MoveFigureTo();
                }
                else if (name.Contains("house")) {
                    int id = int.Parse(name.Replace("house", ""));
                    gameManager.CurrentPlayer.House.Fields[id].MoveFigureTo();
                }
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.H && Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt))
            {
                MessageBoxResult result = MessageBox.Show("Möchtest du den Cheat-Modus öffnen?", "Cheat Modus öffnen", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    cheat = new CheatWindow();
                    cheat.Show();
                }
            }
            else if(e.Key == Key.Escape)
            {
                if (!gameManager.PauseGame)
                {
                    gameManager.ShowOverlay("Spiel ist pausiert", Colors.White, false, false);
                }
                else
                {
                    gameManager.HideOverlay();
                }
                gameManager.PauseGame = !gameManager.PauseGame;
            }
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseApplication(object sender, EventArgs args)
        {
            Close();
            if (cheat != null) cheat.Close();
            main.Menu.InitMainMenu();
            main.Show();
        }
    }


}

