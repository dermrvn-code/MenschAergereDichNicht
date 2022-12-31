using MenschAergerDichNicht.DrawableClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MenschAergerDichNicht.GameClasses
{
    public class GameManager
    {
        private static GameManager instance = null;

        private bool pauseLoop = false;
        public bool PauseGame { get; set; }


        private double loopSpeed = 0.4;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }


        public PlayerData[] PlayerDataSets { get; set; }

        public enum GameState
        {
            Rolling,
            Moving,
            Finishing,
            Finished
        }
        public GameState State { get { return gameState; } }
        private GameState gameState;

        public int FinishedPlayers
        {
            get { return finishedPlayers; }
            set
            {
                if (value <= players.Length && value >= 0)
                {
                    finishedPlayers = value;
                }
            }
        }
        private int finishedPlayers;


        public GameWindow GameWindow { get { return gameWindow; } }
        private GameWindow gameWindow;


        public Player[] Players { get { return players; } }
        private Player[] players;

        public Player CurrentPlayer { get { return currentPlayer; } }
        private Player currentPlayer;

        public Board Board { get { return board; } }
        private Board board;

        public Dice Dice { get { return dice; } }
        private Dice dice;

        private bool currentPlayerCanMove = true;

        public string[] ComputerNames { get; set; }



        private GameManager() { }

        public void Init(GameWindow window)
        {
            gameWindow = window;
            PauseGame = false;

            ComputerNames = ShuffleComputerNamesArray();

            CreatePlayers(SettingsVariables.playerCount);

            currentPlayer = players[0];
            UpdateTextBar(currentPlayer.Name + ", du bist dran!", CurrentPlayer.Icon, CurrentPlayer.Color);

            // CREATE AND DRAW BOARD
            board = new Board();
            Board.Draw();

            // CREATE DICE
            dice = new Dice(gameWindow.rollDiceBtn, gameWindow.diceImage);

            // PLACE FIGURES
            foreach (Player player in players)
            {
                for (int figureId = 0; figureId < 4; figureId++)
                {
                    player.Figures[figureId].Field = player.Start.Fields[figureId];
                    player.Figures[figureId].Draw();
                }
            }
            FinishedPlayers = 0;

        }

        public string[] ShuffleComputerNamesArray()
        {
            Random random = new Random();
            return SettingsVariables.computerNames.OrderBy(x => random.Next()).ToArray();
        }


        public void StartGame()
        {
            gameState = GameState.Rolling;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(loopSpeed);
            timer.Tick += (s, e) => GameLoop(timer);
            timer.Start();
        }

        private void SortScoreboard()
        {
            var list = gameWindow.ScoreboardList;

            // BUBBLE SORT
            for (int i = 0; i < list.Count() - 1; i++)
            {
                for (int j = 0; j < list.Count - i - 1; j++)
                {
                    if (list[i].Score <= list[i + 1].Score) continue;

                    gameWindow.ScoreboardList.Move(i, (i + 1));
                }
            }
        }

        private void GameLoop(DispatcherTimer timer)
        {
            if (pauseLoop || PauseGame) return;

            SortScoreboard();


            if (gameState == GameState.Rolling)
            {
                GameStateRolling();
            }

            else if (gameState == GameState.Moving)
            {
                GameStateMoving();
            }

            else if (gameState == GameState.Finishing)
            {
                GameStateFinishing();
                timer.Stop();
            }


        }

        private void GameStateRolling()
        {
            if (dice.HasUnusedRoll)
            {
                gameState = GameState.Moving;
                dice.HasUnusedRoll = false;
                return;
            }

            dice.Clickable = true;
            if (currentPlayer.AutoRoll)
            {
                dice.Clickable = false;
                dice.Roll();
            }
        }


        private void GameStateMoving()
        {
            bool noFiguresOnBoard = (currentPlayer.Start.FiguresInside - currentPlayer.House.FreeFields) == 0;

            if (!currentPlayer.HasPlayed)
            {

                List<Figure> moveableFigures = currentPlayer.GetMoveableFigures(dice.LastRolled);
                if (moveableFigures.Count() > 0)
                {
                    UpdateTextBar(currentPlayer.Name + ", du darfst ziehen!", CurrentPlayer.Icon, CurrentPlayer.Color);

                    currentPlayerCanMove = true;
                    foreach (Figure figure in moveableFigures)
                    {
                        figure.Highlight();
                    }
                    currentPlayer.OpenRolls = 0;

                    if (moveableFigures.Count == 1)
                    {
                        gameState = GameState.Moving;
                        moveableFigures.First().Click();
                        currentPlayer.HasPlayed = true;
                    }

                    if (currentPlayer is ComputerPlayer)
                    {
                        ((ComputerPlayer)currentPlayer).ChooseBest(moveableFigures);
                    }
                }
                else
                {
                    if (!noFiguresOnBoard)
                    {
                        currentPlayer.OpenRolls = 0;
                    }
                    currentPlayer.HasPlayed = true;
                    currentPlayerCanMove = false;
                }
            }
            else
            {
                board.CleanBoard(currentPlayer);
                gameState = GameState.Rolling;
                if ((dice.LastRolled == 6 && currentPlayerCanMove) ||
                    currentPlayer.OpenRolls > 0)
                {
                    UpdateTextBar(currentPlayer.Name + ", du bist nochmal!", CurrentPlayer.Icon, CurrentPlayer.Color);
                    currentPlayer.HasPlayed = false;
                }
                else
                {
                    if (FinishedPlayers >= 3)
                    {
                        gameState = GameState.Finishing;
                        return;
                    }

                    NextPlayer();
                }
            }
        }

        public async void GameStateFinishing()
        {
            gameState = GameState.Finished;
            int place = 1;
            foreach (Player player in gameWindow.ScoreboardList)
            {
                ShowOverlay(player.Name + " hat den " + place + ". Platz erreicht", player.Color, false, false);
                await Task.Delay(5000);
                place++;
            }
            await Task.Delay(100);

            FinishGame();
        }

        public void FinishGame()
        {
            gameWindow.main.Menu.InitMainMenu();
            gameWindow.main.Show();
            gameWindow.Close();
            return;
        }


        private void CreatePlayers(int count)
        {
            players = new Player[count];
            for (int playerId = 0; playerId < count; playerId++)
            {
                string playerName = PlayerDataSets[playerId].Name;
                ImageSource playerIcon = PlayerDataSets[playerId].Icon;
                Color playerColor = PlayerDataSets[playerId].Color;
                bool autoRoll = PlayerDataSets[playerId].AutoRoll;
                if (playerName != "")
                {
                    players[playerId] = new Player(playerId, playerColor, playerName, playerIcon, autoRoll);
                }
                else
                {
                    players[playerId] = new ComputerPlayer(playerId, playerIcon, playerColor);
                }
                gameWindow.ScoreboardList.Add(players[playerId]);
            }
        }

        private void NextPlayer()
        {
            dice.ResetFace();

            if (currentPlayer != null)
            {
                currentPlayer.HasPlayed = false;
                currentPlayer.OpenRolls = 0;
            }

            if (currentPlayer == null || currentPlayer.Id == 3)
            {
                currentPlayer = players[0];
            }
            else
            {
                currentPlayer = players[currentPlayer.Id + 1];
            }

            if (currentPlayer.IsFinished)
            {
                NextPlayer();
                return;
            }

            currentPlayer.OpenRolls = 3;
            UpdateTextBar(currentPlayer.Name + ", du bist dran!", CurrentPlayer.Icon, CurrentPlayer.Color);
        }

        public async void ShowOverlay(string text, Color color, bool pause, bool autohide = true, int delayInMilliseconds = 1500)
        {
            if(pause) pauseLoop = true;
            await Task.Delay(500);
            gameWindow.overlayText.Foreground = new SolidColorBrush(color);
            gameWindow.overlayText.Text = text;
            gameWindow.overlay.Visibility = Visibility.Visible;
            if (autohide)
            {
                await Task.Delay(delayInMilliseconds);
                gameWindow.overlay.Visibility = Visibility.Hidden;
                if (pause) pauseLoop = false;
            }
        }

        public void HideOverlay()
        {
            gameWindow.overlay.Visibility = Visibility.Hidden;
        }

        public void UpdateTextBar(string text, ImageSource icon, Color color)
        {
            gameWindow.PlayerIcon.ImageSource = icon;
            gameWindow.PlayerIconColor.Fill = new SolidColorBrush(color);
            gameWindow.BottomTextBar.Text = text;
        }

        public void CheatMenu_SetCurrentPlayer(Player player)
        {
            currentPlayer = player;
        }


    }
}
