using MenschAergerDichNicht.GameClasses;
using MenschAergerDichNicht.Helper;
using System;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MenschAergerDichNicht.DrawableClasses
{
    public class Figure : IDrawable
    {

        public Field Field { get; set; }

        public int StepsToGo
        {
            get { return stepsToGo-(player.House.occupiedFields); }
        }
        private int stepsToGo;

        public int Id { get { return id; } }
        private int id;

        public Storyboard[] Storyboards { get; set; }

        public Player Player { get { return player; } }
        private Player player;

        public bool IsFinished { get { return isFinished; } }
        private bool isFinished = false;

        private Ellipse ellipse;

        private bool isHighlighted = false;

        private GameManager gameManager;

        public Figure(Player player, int id)
        {
            gameManager = GameManager.Instance;
            this.player = player;
            this.id = id;
            stepsToGo = -1;
        }

        public override void Draw()
        {
            if (Field != null)
            {
                double offsetPixels = gameManager.Board.FieldSizePixels * 0.1 - (gameManager.Board.FieldSizePixels-Field.SizePixels)/2;
                if (ellipse == null)
                {
                    ellipse = CreateEllipse(player.Color, gameManager.Board.FieldSizePixels - offsetPixels * 2, Field.XPositionPixels + offsetPixels, Field.YPositionPixels + offsetPixels, "p" + player.Id + "f" + Id);
                    gameManager.GameWindow.mainCanvas.Children.Add(ellipse);
                }
                else
                {
                    AnimationHelper.AnimateXY(ellipse, Field.XPositionPixels + offsetPixels, Field.YPositionPixels + offsetPixels);

                    if (IsFinished)
                    {
                        ellipse.Fill = new SolidColorBrush(Color.FromArgb((byte)255/4, player.Color.R, player.Color.G, player.Color.B));
                    }
                }
            }
        }

        public void Finish(bool overlay)
        {
            isFinished = true;
            if(overlay) gameManager.ShowOverlay(player.Name + " hat die " + (player.House.occupiedFields+1) + ". Figur im Haus", player.Color, true);
            Draw();

            if (Field.Id > 0)
            {
                if (player.House.Fields[Field.Id - 1].Occupant != null)
                {
                    player.House.Fields[Field.Id - 1].Occupant.Finish(Player.House.FreeFields != 0);
                    player.House.FreeFields--;

                    if (Player.House.FreeFields == 0) player.Finish();

                }
            }
        }


        public void MoveToField(int fieldId)
        {
            Field[] fields = gameManager.Board.Fields;
            if (fieldId < fields.Length && fieldId > 0)
            {
                Field.Occupant = null;
                Field = fields[fieldId];
                Field.Occupant = this;
                Draw();
            }
        }


        public void MoveToField(Field field)
        {
            Field.Occupant = null;
            Field = field;
            Field.Occupant = this;
            Draw();
        }

        public Field GetFieldInDistance(int distance)
        {
            Field[] fields = gameManager.Board.Fields;
            int fieldCount = fields.Length;

            int newField = Field.Id + distance;

            if (newField > fieldCount - 1)
            {
                newField = newField - fieldCount;
            }
            return GameManager.Instance.Board.Fields[newField];
        }


        public void Jump(int distance)
        {
            if (StepsToGo >= distance)
            {
                if(Field.Type != Field.FieldTypes.House && StepsToGo-distance < player.House.FreeFields)
                {
                    int destinationId = player.House.FreeFields - (StepsToGo - distance) - 1;
                    player.House.Enter(destinationId, this);
                    stepsToGo = 4 - (destinationId+1);
                }
                else if(Field.Type == Field.FieldTypes.House)
                {
                    int destinationId = Field.Id + distance;
                    player.House.Enter(destinationId, this);
                    stepsToGo = 4 - (destinationId + 1);
                }

                else
                {
                    Field destination = GetFieldInDistance(distance);

                    if (destination.Occupant != null)
                    {
                        if (destination.Occupant.Player != Player)
                        {
                            destination.Occupant.Capture();
                        }
                        else if (destination.Occupant.Player == player)
                        {
                            return;
                        }
                    }

                    MoveToField(destination);
                    stepsToGo -= distance;
                }
            }

        }

        

        public void Highlight()
        {
            if (!isHighlighted)
            {
                ellipse.Stroke = new SolidColorBrush(Colors.White);
                if (Storyboards == null || Storyboards.Length == 0)
                {
                    Storyboards = AnimationHelper.HighlightAnimation(ellipse, 500);
                }
                else
                {
                    foreach (var storyboard in Storyboards)
                    {
                        storyboard.Begin();
                    }
                }
                isHighlighted = true;
            }
        }

        public void StopHighlight()
        {
            if (isHighlighted)
            {
                ellipse.Stroke = null;
                foreach (var storyboard in Storyboards)
                {
                    storyboard.Stop();
                }
                isHighlighted = false;
            }
        }

        public void Spawn()
        {
            Field spawn = player.Spawn;
            if (spawn.Occupant != null)
            {
                if (spawn.Occupant.Player == player) return;

                spawn.Occupant.Capture();
            }
            MoveToField(player.Spawn.Id);
            stepsToGo = gameManager.Board.BoardSizeFields-((int)(SettingsVariables.shortSideFields/2)) + 4;
            player.Start.FiguresInside -= 1;
        }

        public void Capture()
        {
            MoveToField(player.Start.Fields[Id]);
            stepsToGo = -1;
            gameManager.CurrentPlayer.OpenRolls++;
            player.Start.FiguresInside += 1;
        }

        public void Click()
        {
            if (!player.HasPlayed)
            {
                if (gameManager.State == GameManager.GameState.Moving)
                {
                    if (isHighlighted)
                    {
                        int diceRoll = gameManager.Dice.LastRolled;
                        if (Field.Type == Field.FieldTypes.Start && diceRoll == 6)
                        {
                            Spawn();
                        }
                        else if (StepsToGo >= diceRoll)
                        {
                            Jump(diceRoll);
                        }

                        player.HasPlayed = true;

                        foreach (var figure in player.Figures)
                        {
                            figure.StopHighlight();
                        }
                    }
                }
            }
        }
    }
}
