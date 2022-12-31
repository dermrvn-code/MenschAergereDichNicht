using MenschAergerDichNicht.GameClasses;
using System.Windows.Controls;

namespace MenschAergerDichNicht.DrawableClasses
{
    public class Start : IDrawable
    {

        public double XPositionPixels { get; set; }
        public double YPositionPixels { get; set; }

        private TextBlock playerName;

        public Field[] Fields { get { return fields; } }
        private Field[] fields;

        public int FiguresInside
        {
            get { return figuresInside; }
            set
            {
                if (value >= 0)
                {
                    figuresInside = value;
                }
            }
        }
        private int figuresInside;


        private Player player;

        private GameManager gameManager;

        public Start(Player player)
        {
            gameManager = GameManager.Instance;
            this.player = player;
            fields = new Field[4];

            FiguresInside = 4;

            for (int fieldId = 0; fieldId < Fields.Length; fieldId++)
            {
                Fields[fieldId] = new Field((player.Id + 1) * -100, Field.FieldTypes.Start, player.Color);
            }

            XPositionPixels = 0;
            YPositionPixels = 0;
        }

        public override void Draw()
        {

            if (player.Id == 0)
            {
                XPositionPixels = (SettingsVariables.bufferFields + 1) * gameManager.Board.FieldSizePixels;
                YPositionPixels = (SettingsVariables.bufferFields + 1) * gameManager.Board.FieldSizePixels;
            }
            else if (player.Id == 1)
            {
                XPositionPixels = (gameManager.Board.BoardWidthFields + SettingsVariables.bufferFields - 3) * gameManager.Board.FieldSizePixels;
                YPositionPixels = (SettingsVariables.bufferFields + 1) * gameManager.Board.FieldSizePixels;
            }
            else if (player.Id == 2)
            {
                XPositionPixels = (gameManager.Board.BoardWidthFields + SettingsVariables.bufferFields - 3) * gameManager.Board.FieldSizePixels;
                YPositionPixels = (gameManager.Board.BoardWidthFields + SettingsVariables.bufferFields - 3) * gameManager.Board.FieldSizePixels;
            }
            else if (player.Id == 3)
            {
                XPositionPixels = (SettingsVariables.bufferFields + 1) * gameManager.Board.FieldSizePixels;
                YPositionPixels = (gameManager.Board.BoardWidthFields + SettingsVariables.bufferFields - 3) * gameManager.Board.FieldSizePixels;
            }

            double fieldSizePixels = gameManager.Board.FieldSizePixels;

            DrawField(Fields[0], XPositionPixels, YPositionPixels);
            DrawField(Fields[1], XPositionPixels + fieldSizePixels, YPositionPixels);
            DrawField(Fields[2], XPositionPixels, YPositionPixels + fieldSizePixels);
            DrawField(Fields[3], XPositionPixels + fieldSizePixels, YPositionPixels + fieldSizePixels);


            playerName = CreateText(player.Name, player.Color, 10, false, XPositionPixels, YPositionPixels - 20);
            gameManager.GameWindow.mainCanvas.Children.Add(playerName);

        }
    }
}
