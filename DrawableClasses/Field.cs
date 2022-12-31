using System.Windows.Media;
using System.Windows.Shapes;
using MenschAergerDichNicht.GameClasses;

namespace MenschAergerDichNicht.DrawableClasses
{
    public class Field : IDrawable
    {
        public enum FieldTypes
        {
            Normal,
            Spawn,
            HouseEntry,
            Start,
            House
        }

        public Color Color { get { return color; } }
        private Color color;

        public FieldTypes Type { get { return type; } }
        private FieldTypes type;

        public double XPositionPixels { get; set; }
        public double YPositionPixels { get; set; }

        public int Id { get { return id; } }
        private int id;

        public double SizePixels
        {
            get { return sizePixels; }
            set { sizePixels = value > 0 ? value : -value; }
        }
        private double sizePixels;

        public bool Marked { get; set; }

        private GameManager gameManager;

        private Rectangle rectangle;

        public Figure Occupant { get; set; }

        private Brush normalBrush;

        private Figure possibleFutureOccupant = null;

        public Field(int id, FieldTypes type, Color color)
        {
            gameManager = GameManager.Instance;
            this.id = id;
            this.type = type;
            this.color = color;
            Marked = false;
            sizePixels = 100;
            XPositionPixels = 0;
            YPositionPixels = 0;
        }

        public void Mark(Figure possibleFutureOccupant)
        {
            Marked = true;
            rectangle.Fill = new SolidColorBrush(Color.FromArgb((byte)(0.8 * 255), Colors.DarkSalmon.R, 
                                                 Colors.DarkSalmon.G, Colors.DarkSalmon.B));
            this.possibleFutureOccupant = possibleFutureOccupant;
        }

        public void Unmark()
        {
            Marked = false;
            possibleFutureOccupant = null;
            rectangle.Fill = normalBrush;
        }

        public override void Draw()
        {
            if (Type == FieldTypes.Spawn)
            {
                rectangle = CreateRec(color, sizePixels, sizePixels, XPositionPixels, YPositionPixels);
                rectangle.Name = "normal" + id;
            }
            else if (Type == FieldTypes.House)
            {
                rectangle = CreateRec(Darken(color, 0.25f), sizePixels, sizePixels, XPositionPixels, YPositionPixels);
                rectangle.Name = "house" + id;
            }
            else if (Type == FieldTypes.Start)
            {
                rectangle = CreateRec(Darken(color, 0.5f), sizePixels, sizePixels, XPositionPixels, YPositionPixels);
            }
            else
            {
                rectangle = CreateRec(Colors.White, sizePixels, sizePixels, XPositionPixels, YPositionPixels);
                rectangle.Name = "normal" + id;
            }

            gameManager.GameWindow.mainCanvas.Children.Add(rectangle);
            normalBrush = rectangle.Fill;
        }

        public bool IsOccupying(Player player)
        {
            if (Occupant == null) return false;
            if (Occupant.Player == player) return true;

            return false;
        }

        public static Field GetFutureField(Figure figure, int spaceNeeded)
        {
            if (figure.Field.Type == Field.FieldTypes.Start && GameManager.Instance.Dice.LastRolled == 6) return figure.Player.Spawn;

            if (figure.Field.Type != Field.FieldTypes.House && figure.StepsToGo - spaceNeeded < figure.Player.House.FreeFields)
            {
                int houseId = figure.Player.House.FreeFields - (figure.StepsToGo - spaceNeeded) - 1;
                houseId = houseId < 4 ? houseId : 3;
                return figure.Player.House.Fields[houseId];
            }

            if (figure.Field.Type == Field.FieldTypes.House)
            {
                int futureId = figure.Field.Id + spaceNeeded;
                futureId = futureId < 4 ? futureId : 3;
                return figure.Player.House.Fields[futureId];
            }

            return figure.GetFieldInDistance(spaceNeeded);
        }

        public void MoveFigureTo()
        {
            if(Marked && possibleFutureOccupant != null)
            {
                possibleFutureOccupant.Click();
            }
        }

    }
}
