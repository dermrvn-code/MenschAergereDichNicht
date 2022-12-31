using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace MenschAergerDichNicht.GameClasses
{
    public class MainMenu
    {

        public List<Color> ChooseColors { get { return chooseColors; } }
        private List<Color> chooseColors;
        public List<ImageSource> ChooseIcons { get { return chooseIcons; } }
        private List<ImageSource> chooseIcons;

        MainWindow main;

        public MainMenu(MainWindow main)
        {
            this.main = main;
            InitMainMenu();
        }

        public void InitMainMenu()
        {
            chooseColors = SettingsVariables.colors.ToList();
            chooseIcons = new List<ImageSource>();
            for (int iconId = 1; iconId <= 8; iconId++)
            {                
                chooseIcons.Add(Player.GetPlayerIcon(iconId));
            }

            foreach (var player in main.PlayerData)
            {
                chooseColors.Remove(player.Color);

                var iconLoop = new List<ImageSource>(chooseIcons);

                foreach(var icon in iconLoop)
                {
                    if (Player.GetIconId(icon) != Player.GetIconId(player.Icon)) continue;
                    chooseIcons.Remove(icon);
                }
            }
        }

        public List<PlayerData> AddBots()
        {
            var playerDataWithBots = new List<PlayerData>(main.PlayerData);
            int botCount = 4 - main.PlayerData.Count();

            for (int botId = 0; botId < botCount; botId++)
            {
                playerDataWithBots.Add(new PlayerData("", PickRandomColor(), true, PickRandomIcon()));
            }
            return playerDataWithBots;
        }

        private Color PickRandomColor()
        {
            Random rand = new Random();
            int colorIndex = rand.Next(0, chooseColors.Count());
            Color returnColor = chooseColors[colorIndex];
            chooseColors.Remove(returnColor);

            return returnColor;
        }

        private ImageSource PickRandomIcon()
        {
            Random rand = new Random();
            int iconIndex = rand.Next(0, chooseIcons.Count());
            ImageSource returnIcon = chooseIcons[iconIndex];
            chooseIcons.Remove(returnIcon);

            return returnIcon;
        }


        public void AddPlayer(string name, Color color, bool autoroll, ImageSource icon)
        {
            PlayerData newPlayerSet = new PlayerData(name, color, autoroll, icon);
            chooseColors.Remove(color);
            chooseIcons.Remove(icon);
            main.PlayerData.Add(newPlayerSet);
        }


        public void RemovePlayer(string name)
        {
            if (name == null) return;

            foreach (var player in main.PlayerData)
            {
                if (player.Name != name) continue;

                chooseColors.Add(player.Color);
                chooseIcons.Add(player.Icon);

                main.PlayerData.Remove(player);
                break;

            }
        }



    }
}
