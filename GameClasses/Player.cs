using System;
using System.Collections.Generic;
using System.Windows.Media;
using MenschAergerDichNicht.DrawableClasses;
using System.Windows.Media.Imaging;
using System.IO;

namespace MenschAergerDichNicht.GameClasses
{
    public class Player
    {
        public int Id { get; set; }
        public Color Color { get; set; }
        public House House { get; set; }
        public ImageSource Icon { get; set; }
        public Field Spawn { get; set; }
        public Field HouseEntry { get; set; }
        public Start Start { get; set; }
        public Figure[] Figures { get; set; }
        public string Name { get; set; }
        public bool AutoRoll { get; set; }

        public int Score
        {
            get
            {
                int s = Start.FiguresInside * 100;
                foreach (var f in Figures)
                {
                    s += f.StepsToGo;
                }
                return s + scoreOffset;
            }
        }
        private int scoreOffset = 0;

        public bool HasPlayed { get; set; }

        public bool IsFinished { get { return isFinished; } }
        private bool isFinished = false;

        public int OpenRolls
        {
            get { return openRolls; }
            set { if (value >= 0) openRolls = value; }
        }
        private int openRolls;

        private Board board;

        private GameManager gameManager;


        public Player(int id, Color color, string name, ImageSource icon, bool autoroll)
        {
            gameManager = GameManager.Instance;

            Icon = icon;

            HasPlayed = false;
            isFinished = false;

            AutoRoll = autoroll;

            OpenRolls = 3;


            this.Id = id;
            this.Color = color;
            this.Name = name;
            board = gameManager.Board;

            this.Figures = new Figure[4];
            for (int i = 0; i < 4; i++)
            {
                Figures[i] = new Figure(this, i);
            }

            House = new House(this);
            Start = new Start(this);
        }

        public void Finish()
        {
            isFinished = true;
            scoreOffset += (gameManager.FinishedPlayers);
            gameManager.FinishedPlayers++;

            gameManager.ShowOverlay(Name + " hat alle Figuren im Haus", Color, true);
        }

        public void SpawnFigure(int fig)
        {
            Figure figure = Figures[fig];
        }

        public List<Figure> GetMoveableFigures(int spaceNeeded)
        {
            List<Figure> moveableFigures = new List<Figure>();

            for (int figureId = 0; figureId < Figures.Length; figureId++)
            {
                Figure figure = Figures[figureId];

                if (figure.IsFinished) continue;

                // FIGURE IS IN HOUSE AND DICE ROLLED A 6
                if (figure.Field.Type == Field.FieldTypes.Start && spaceNeeded == 6)
                {
                    if (!Spawn.IsOccupying(this))
                    {
                        moveableFigures.Add(figure);
                        Spawn.Mark(figure);
                    }
                }


                if (figure.StepsToGo < spaceNeeded) continue;

                Field futureField = Field.GetFutureField(figure, spaceNeeded);

                if (futureField.IsOccupying(this)) continue;


                moveableFigures.Add(figure);
                futureField.Mark(figure);

            }

            return moveableFigures;
        }

        public static ImageSource GetPlayerIcon(int iconId)
        {
            return new BitmapImage(new Uri("Assets/Images/Playericons/ship" + iconId + ".png", UriKind.Relative));
        }

        public static int GetIconId(ImageSource source)
        {
            string path = ((System.Windows.Media.Imaging.BitmapImage)source).UriSource.ToString();
            string fileName = Path.GetFileName(path);
            int id = Int32.Parse(fileName.Replace("ship", "").Replace(".png", ""));
            return id;
        }


    }
}
