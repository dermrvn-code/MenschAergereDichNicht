using MenschAergerDichNicht.GameClasses;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace MenschAergerDichNicht.DrawableClasses
{
    public abstract class IDrawable
    {
        public abstract void Draw();


        public Rectangle CreateRec(Color color, double width, double height, double x, double y)
        {
            Rectangle rect;
            rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(color);
            rect.StrokeThickness = width / 25;
            rect.Fill = new SolidColorBrush(Color.FromArgb((byte)(0.1 * 255), color.R, color.G, color.B));
            rect.Width = width;
            rect.Height = height;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

            return rect;
        }

        public Ellipse CreateEllipse(Color color, double width, double x, double y, string name)
        {
            Ellipse ell;
            ell = new Ellipse();
            ell.Fill = new SolidColorBrush(color);
            ell.StrokeThickness = 2;
            ell.Width = width;
            ell.Height = width;
            ell.Name = name;
            Canvas.SetLeft(ell, x);
            Canvas.SetTop(ell, y);

            return ell;
        }


        public TextBlock CreateText(string text, Color color, int fontSize, bool bold, double x, double y)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.FontSize = fontSize;
            textBlock.FontWeight = (bold) ? FontWeights.Bold : FontWeights.Normal;
            textBlock.Foreground = new SolidColorBrush(color);
            textBlock.FontFamily = (FontFamily)textBlock.FindResource("Nulshock");

            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);

            return textBlock;
        }



        public void DrawField(Field f, double x, double y, double scale = 1)
        {
            GameManager gm = GameManager.Instance;
            double offset = gm.Board.FieldSizePixels * (1 - scale) / 2;
            f.XPositionPixels = x + offset;
            f.YPositionPixels = y + offset;
            f.SizePixels = gm.Board.FieldSizePixels * scale;
            f.Draw();
        }

        public Color Darken(Color color, float factor)
        {
            factor = 1 - factor;

            return Color.FromRgb((byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));
        }

    }
}
