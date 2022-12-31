using MenschAergerDichNicht.GameClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MenschAergerDichNicht
{
    public partial class MainWindow : Window
    {


        private ObservableCollection<PlayerData> playerData = new ObservableCollection<PlayerData>();
        public ObservableCollection<PlayerData> PlayerData
        {
            get { return playerData; }
            set { playerData = value; }
        }


        public int NormalModeBoardSizeFields
        {
            get { return (normalModeLongSide * 2 + normalModeShortSide + 1) * 4; }
        }
        public int LargeModeBoardSizeFields
        {
            get { return (largeModeLongSide * 2 + largeModeShortSide + 1) * 4; }
        }
        public int HugeModeBoardSizeFields
        {
            get { return (hugeModeLongSide * 2 + hugeModeShortSide + 1) * 4; }
        }

        private int normalModeLongSide = 4;
        private int normalModeShortSide = 2;

        private int largeModeLongSide = 6;
        private int largeModeShortSide = 2;

        private int hugeModeLongSide = 8;
        private int hugeModeShortSide = 4;

        public MainMenu Menu { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            Menu = new MainMenu(this);
            DataContext = this;
        }
        private void OpenGame(object sender, RoutedEventArgs e)
        {
            GameManager.Instance.PlayerDataSets = Menu.AddBots().ToArray();

            GameWindow gameWindow = new GameWindow();
            gameWindow.main = this;
            gameWindow.Show();

            this.Hide();
        }


        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnSelectMode(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            string tag = (string)radio.Tag;

            if (tag == "large")
            {
                SettingsVariables.longSideFields = largeModeLongSide;
                SettingsVariables.shortSideFields = largeModeShortSide;
            }
            else if (tag == "huge")
            {
                SettingsVariables.longSideFields = hugeModeLongSide;
                SettingsVariables.shortSideFields = hugeModeShortSide;
            }
            else
            {
                SettingsVariables.longSideFields = normalModeLongSide;
                SettingsVariables.shortSideFields = normalModeShortSide;
            }

        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }

        private void OpenAddPlayerWindow(object sender, RoutedEventArgs e)
        {
            AddPlayerWindow addPlayer = new AddPlayerWindow(this,
                new ObservableCollection<Color>(Menu.ChooseColors),
                new ObservableCollection<ImageSource>(Menu.ChooseIcons));
            addPlayer.Show();
            this.Hide();

        }


        private void RemovePlayer(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string name = button.Tag.ToString();

            Menu.RemovePlayer(name);
        }
    }
}