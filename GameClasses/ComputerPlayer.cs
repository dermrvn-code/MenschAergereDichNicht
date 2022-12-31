using MenschAergerDichNicht.DrawableClasses;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace MenschAergerDichNicht.GameClasses
{
    class ComputerPlayer : Player
    {
        GameManager gameManager;


        public ComputerPlayer(int id, ImageSource icon, Color color) : base(id, color, GameManager.Instance.ComputerNames[id], icon, true)
        {
            gameManager = GameManager.Instance;
        }


        public void MoveFigure(Figure figure)
        {
            figure.Click();
        }

        public void ChooseBest(List<Figure> moveableFigures)
        {
            Dictionary<Figure, int> points = CalculatePoints(moveableFigures);

            Figure bestFigure = GetFigureWithBestScore(points);
            if (bestFigure == null) bestFigure = moveableFigures.First();

            MoveFigure(bestFigure);
        }

        public Dictionary<Figure, int> CalculatePoints(List<Figure> moveableFigures)
        {

            Dictionary<Figure, int> points = new Dictionary<Figure, int>();
            moveableFigures.ForEach(figure => points.TryAdd(figure, 0));


            int furthestPoints = 8;

            Figure furthestFigure = moveableFigures.First();
            int leastStepsToGo = 1000;


            foreach (Figure figure in moveableFigures)
            {
                Field futureField = Field.GetFutureField(figure, gameManager.Dice.LastRolled);
                points[figure] += PointsCalculation(moveableFigures, figure, futureField);

                if (figure.StepsToGo > 0 && figure.StepsToGo < leastStepsToGo)
                {
                    leastStepsToGo = figure.StepsToGo;
                    furthestFigure = figure;
                }
            }
            points[furthestFigure] += furthestPoints;


            return points;
        }

        public int PointsWhenHitableInRange(Field futureField, int hitableInRange)
        {
            int points = 0;
            for (int fieldsBack = 1; fieldsBack < 6; fieldsBack++)
            {
                int fieldId = futureField.Id - 1;
                if (fieldId < 0) fieldId = gameManager.Board.Fields.Length + fieldId;

                if (!futureField.IsOccupying(this))
                {
                    if (futureField.Occupant != null)
                    {
                        points += hitableInRange;
                    }
                }
            }
            return points;
        }

        public int PointsCalculation(List<Figure> moveableFigures, Figure figure, Field futureField)
        {
            int hitPoints = 25;
            int spawnPoints = 15;
            int standingOnSpawnPoints = 10;

            int goingToSpawn = -8;
            int hitableInRange = -3;

            int points = 0;
            if (figure.Field.Type == Field.FieldTypes.Start)
            {
                points += spawnPoints;
            }

            if (!futureField.IsOccupying(this))
            {
                if (futureField.Occupant != null)
                {
                    points += hitPoints;
                }
            }

            if (figure.Field.Type == Field.FieldTypes.Spawn)
            {
                points += standingOnSpawnPoints;
            }

            if (futureField.Type == Field.FieldTypes.Spawn && figure.Field.Type != Field.FieldTypes.Start)
            {
                points += goingToSpawn;
            }

            if (figure.Field.Type != Field.FieldTypes.Start && figure.Field.Type != Field.FieldTypes.House)
            {
                if (futureField.Type != Field.FieldTypes.House)
                {
                    points += PointsWhenHitableInRange(futureField, hitableInRange);
                }
            }

            return points;
        }

        public Figure GetFigureWithBestScore(Dictionary<Figure, int> scores)
        {
            Figure bestFigure = scores.First().Key;
            int bestScore = 0;

            foreach (var figureScorePair in scores)
            {
                Figure figure = figureScorePair.Key;
                int score = figureScorePair.Value;

                if (score >= bestScore)
                {
                    bestScore = score;
                    bestFigure = figure;
                }
            }
            return bestFigure;
        }
    }
}
