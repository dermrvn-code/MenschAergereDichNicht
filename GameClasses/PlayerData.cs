using System.Windows;
using System.Windows.Media;

namespace MenschAergerDichNicht.GameClasses
{
    public class PlayerData
    {
        public string Name { get; set; }
        public Color Color { get; }
        public bool AutoRoll { get; set; }
        public double Opacity
        {
            get
            {
                return (AutoRoll) ? 1 : 0.1;
            }
        }
        public ImageSource Icon { get; set; }

        public PlayerData(string name, Color color, bool autoRoll, ImageSource icon)
        {
            Name = name;
            Color = color;
            Icon = icon;
            AutoRoll = autoRoll;
        }

    }
}
