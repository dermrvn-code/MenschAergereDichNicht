using MenschAergerDichNicht.GameClasses;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MenschAergerDichNicht
{
    public partial class AddPlayerWindow : Window
    {

        private ObservableCollection<Color> pickableColors = new ObservableCollection<Color>();
        public ObservableCollection<Color> PickableColors
        {
            get { return pickableColors; }
            set { pickableColors = value; }
        }


        private ObservableCollection<ImageSource> pickableIcons = new ObservableCollection<ImageSource>();
        public ObservableCollection<ImageSource> PickableIcons
        {
            get { return pickableIcons; }
            set { pickableIcons = value; }
        }

        private bool autoroll = false;
        private Color pickedColor = Colors.Transparent;
        private ImageSource pickedIcon;
        private string playerName;

        private MainWindow main;

        public AddPlayerWindow(MainWindow main, ObservableCollection<Color> colors, ObservableCollection<ImageSource> icons)
        {
            InitializeComponent();

            this.main = main;

            PickableColors = colors;
            PickableIcons = icons;

            DataContext = this;
        }


        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
            main.Show();
        }

        private void PickColor(object sender, RoutedEventArgs e)
        {
            RadioButton button = (RadioButton)sender;
            var element = (SolidColorBrush)button.FindName("brush");
            pickedColor = element.Color;
        }

        private void PickIcon(object sender, RoutedEventArgs e)
        {
            RadioButton button = (RadioButton)sender;
            var element = (ImageBrush)button.FindName("icon");
            pickedIcon = element.ImageSource;
        }

        private void PickAutoroll(object sender, RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            autoroll = check.IsChecked == true;
        }

        private void NameEnter(object sender, KeyEventArgs e)
        {
            TextBox box = (TextBox)sender;
            playerName = box.Text;
        }

        private void AddPlayerClick(object sender, RoutedEventArgs e)
        {
            bool notAllFilled = false;
            if (pickedColor == Colors.Transparent) notAllFilled = true;
            if (pickedIcon == null) notAllFilled = true;
            if (playerName == null || playerName == "") notAllFilled = true;

            if (notAllFilled)
            {
                MessageBox.Show("Bitte wähle einen Namen, Farbe und ein Icon");
                return;
            }

            main.Menu.AddPlayer(playerName, pickedColor, autoroll, pickedIcon);

            this.Close();
            main.Show();


        }
    }
}
