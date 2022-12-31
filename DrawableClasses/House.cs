using MenschAergerDichNicht.GameClasses;

namespace MenschAergerDichNicht.DrawableClasses
{
    public class House : IDrawable
    {

        private Player player;

        public Field[] Fields { get { return fields; } }
        private Field[] fields;

        public int FreeFields { 
            get { return freeFields; }
            set { if(value > 0 && value <=4) freeFields = value; }
        }
        private int freeFields;

        public int occupiedFields
        {
            get { return 4 - freeFields; }
        }

        private GameManager gameManager;

        public House(Player player)
        {
            gameManager = GameManager.Instance;
            this.player = player;
            fields = new Field[4];
            freeFields = 4;
            for (int fieldId = 0; fieldId < Fields.Length; fieldId++)
            {
                Fields[fieldId] = new Field(fieldId, Field.FieldTypes.House, player.Color);
            }
        }

        public void Enter(int id, Figure figure)
        {

            figure.MoveToField(Fields[id]);

            if (id == (FreeFields - 1))
            {
                figure.Finish(freeFields != 0);
                freeFields--;

                if (FreeFields == 0) player.Finish();
            }
        }

        public override void Draw()
        {
            double fSize = gameManager.Board.FieldSizePixels;
            for (int i = 0; i < Fields.Length; i++)
            {
                Field f = Fields[i];
                int multiplicatorX = player.Id % 2 == 0 ? 1 : 0;
                int multiplicatorY = player.Id % 2 == 0 ? 0 : 1;
                int inverter = player.Id <= 1 ? 1 : -1;

                DrawField(f, 
                    player.HouseEntry.XPositionPixels + (fSize * (i + 1) * inverter) * multiplicatorX, 
                    player.HouseEntry.YPositionPixels + (fSize * (i + 1) * inverter) * multiplicatorY, 
                0.9);
            }


        }
    }
}
