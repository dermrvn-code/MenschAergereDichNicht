using MenschAergerDichNicht.GameClasses;
using System;

namespace MenschAergerDichNicht.DrawableClasses
{
    public class Board : IDrawable
    {
        public Field[] Fields { get { return fields; } }
        private Field[] fields;

        public double FieldSizePixels { get { return fieldSizePixels; } }
        private double fieldSizePixels;

        public int BoardSizeFields { get { return boardSizeFields; } }
        private int boardSizeFields;

        public int BoardWidthFields { get { return boardWidthFields; } }
        private int boardWidthFields;

        private GameManager gameManager;
        public Board()
        {
            gameManager = GameManager.Instance;
            fields = GenerateFields(SettingsVariables.playerCount);
            boardSizeFields = fields.Length;

            // CALCULATE VALUES IN RELATION TO CANVAS SIZE
            boardWidthFields = SettingsVariables.longSideFields * 2 + SettingsVariables.shortSideFields + 1;
            fieldSizePixels = gameManager.GameWindow.mainCanvas.ActualWidth / (BoardWidthFields + SettingsVariables.bufferFields * 2);
        }


        private Field[] GenerateFields(int players)
        {
            int segmentLengthFields = SettingsVariables.longSideFields * 2 + SettingsVariables.shortSideFields;
            int fieldsArrayLength = segmentLengthFields * players;
            int currentPlayerSegment = 0;

            Field[] fields = new Field[fieldsArrayLength];

            for (int fieldId = 0; fieldId < fieldsArrayLength; fieldId++)
            {
                Player currP = gameManager.Players[currentPlayerSegment];
                Field.FieldTypes type = Field.FieldTypes.Normal;

                // CHECK FOR POSITION OF SPAWN FIELD AND FOR POSITION OF HOUSEENTRY
                if ((fieldId+1 - (SettingsVariables.longSideFields + SettingsVariables.shortSideFields)) % segmentLengthFields == 0)
                {
                    type = Field.FieldTypes.Spawn;
                }
                else if ((fieldId - (SettingsVariables.longSideFields + SettingsVariables.shortSideFields - 1) + SettingsVariables.shortSideFields / 2) % segmentLengthFields == 0)
                {
                    type = Field.FieldTypes.HouseEntry;
                }

                // CALCULATE "OWNER" OF CURRENT SEGMENT
                currentPlayerSegment = (int)Math.Floor((double)fieldId / segmentLengthFields);

                // ADD FIELD TO ARRAY
                fields[fieldId] = new Field(fieldId, type, currP.Color);

                // SET SPECIAL FIELDS TO ITS PLAYER OBJECT
                if (type == Field.FieldTypes.Spawn)
                {
                    currP.Spawn = fields[fieldId];

                }
                else if (type == Field.FieldTypes.HouseEntry)
                {
                    currP.HouseEntry = fields[fieldId];
                }

            }
            return fields;

        }

        public void CleanBoard(Player currentPlayer)
        {
            foreach(Field field in Fields)
            {
                if (!field.Marked) continue;
                field.Unmark();
            }
            foreach(Field field in currentPlayer.House.Fields)
            {
                if (!field.Marked) continue;
                field.Unmark();
            }
        }

        public override void Draw()
        {

            double xOffsetPixels = (SettingsVariables.bufferFields + SettingsVariables.longSideFields) * fieldSizePixels;
            double yOffsetPixels = (SettingsVariables.bufferFields + SettingsVariables.longSideFields 
                                    + SettingsVariables.shortSideFields) * fieldSizePixels;

            int fieldIdCounter = 0;
            for (int arms = 0; arms < 4; arms++)
            {

                // left and right arm
                if (arms % 2 == 0)
                {

                    // SWITCH BETWEEN LEFT AND RIGHT ARM
                    int startDirection = arms == 0 ? -1 : 1;


                    for (int longSide = 0; longSide < SettingsVariables.longSideFields; longSide++)
                    {
                        xOffsetPixels = xOffsetPixels + startDirection * fieldSizePixels;
                        Field f = fields[fieldIdCounter];
                        DrawField(f, xOffsetPixels, yOffsetPixels);
                        fieldIdCounter++;
                    }
                    for (int shortSide = 0; shortSide < SettingsVariables.shortSideFields; shortSide++)
                    {
                        yOffsetPixels = yOffsetPixels + startDirection * fieldSizePixels;
                        Field f = fields[fieldIdCounter];
                        DrawField(f, xOffsetPixels, yOffsetPixels);
                        fieldIdCounter++;
                    }
                    for (int longSide = 0; longSide < SettingsVariables.longSideFields; longSide++)
                    {
                        xOffsetPixels = xOffsetPixels + -1 * startDirection * fieldSizePixels;
                        Field f = fields[fieldIdCounter];
                        DrawField(f, xOffsetPixels, yOffsetPixels);
                        fieldIdCounter++;
                    }


                }
                // top and bottom arm
                else
                {
                    // SWITCH BETWEEN TOP AND BOTTOM ARM
                    int startDirection = arms == 1 ? -1 : 1;


                    for (int ls = 0; ls < SettingsVariables.longSideFields; ls++)
                    {
                        yOffsetPixels = yOffsetPixels + startDirection * fieldSizePixels;
                        Field f = fields[fieldIdCounter];
                        DrawField(f, xOffsetPixels, yOffsetPixels);
                        fieldIdCounter++;
                    }
                    for (int ss = 0; ss < SettingsVariables.shortSideFields; ss++)
                    {
                        xOffsetPixels = xOffsetPixels + -startDirection * fieldSizePixels;
                        Field f = fields[fieldIdCounter];
                        DrawField(f, xOffsetPixels, yOffsetPixels);
                        fieldIdCounter++;
                    }
                    for (int ls = 0; ls < SettingsVariables.longSideFields; ls++)
                    {
                        yOffsetPixels = yOffsetPixels + -startDirection * fieldSizePixels;
                        Field f = fields[fieldIdCounter];
                        DrawField(f, xOffsetPixels, yOffsetPixels);
                        fieldIdCounter++;
                    }
                }
            }


            Player[] players = gameManager.Players;

            // DRAW HOUSES
            foreach (Player player in players)
            {
                player.House.Draw();
            }

            // DRAW STARTS
            foreach (Player player in players)
            {
                player.Start.Draw();
            }
        }

    }
}
