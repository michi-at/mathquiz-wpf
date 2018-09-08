namespace MathQuizWPF
{
    using System.Windows;
    using WinForms = System.Windows.Forms;
    using Drawing = System.Drawing;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class App : Application
    {
        private static readonly string[] menuItemNames = { "Play", "Stop", "Difficulty Level", "Exit" };
        private void InitializeIcon()
        {
            WinForms.ContextMenu contextMenu = new WinForms.ContextMenu();

            // Initialize contextMenu

            for (int i = 0; i < menuItemNames.Length; ++i)
            {
                contextMenu.MenuItems.Add(menuItemNames[i], MenuItemClick);
                if (menuItemNames[i] == "Difficulty Level")
                {
                    string[] difficultyLevels = Enum.GetNames(typeof(DifficultyLevel));
                    foreach(string dl in difficultyLevels)
                    {
                        contextMenu.MenuItems[i].MenuItems.Add(dl, DifficultyLevelChanged);
                        if (dl == Enum.GetName(typeof(DifficultyLevel), (DifficultyLevel)0))
                            contextMenu.MenuItems[i].MenuItems[contextMenu.MenuItems[i].MenuItems.Count - 1].Checked = true;
                    }
                }
            }

            // Initialize icon

            icon = new WinForms.NotifyIcon
            {
                Text = appName,
                Visible = true,
                ContextMenu = contextMenu
            };
            IconLookup();
            icon.DoubleClick += new EventHandler(IconDoubleClick);
        }

        private void IconDoubleClick(object sender, EventArgs e)
        {
            mainWindow.Show();
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            switch((sender as WinForms.MenuItem).Text)
            {
                case "Play":
                    {
                        viewModel.IsPlaying = true;
                        break;
                    }
                case "Stop":
                    {
                        viewModel.IsPlaying = false;
                        break;
                    }
                case "Exit":
                    {
                        this.Shutdown();
                        break;
                    }
            }
        }

        private void DifficultyLevelChanged(object sender, EventArgs e)
        {
            DifficultyLevel difficultyLevel = (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), (sender as WinForms.MenuItem).Text);
            switch(difficultyLevel) {
                default:
                    {
                        (sender as WinForms.MenuItem).Checked = true;
                        foreach(WinForms.MenuItem item in icon.ContextMenu.MenuItems[2].MenuItems)
                        {
                            if (item.Index != (sender as WinForms.MenuItem).Index)
                            {
                                item.Checked = false;
                            }
                        }
                        viewModel.GameMode = difficultyLevel;
                        break;
                    }
            }
        }

        private void IconLookup()
        {
            string directoryRoot = Directory.GetCurrentDirectory();
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryRoot);

            FileStream iconFile;
            string fileName = FindIcon(directoryInfo);
            if (fileName == "") icon.Icon = Utils.IconExtractor.Extract(Environment.ExpandEnvironmentVariables(@"%systemroot%\system32\shell32.dll"), 0, true);
            else
            {
                iconFile = new FileStream(fileName, FileMode.Open);
                icon.Icon = new Drawing.Icon(iconFile);
                iconFile.Close();
                mainWindow.Icon = new BitmapImage(new Uri(fileName));
            }
        }

        private string FindIcon(DirectoryInfo directory)
        {
            string fileName = CheckFiles(directory);
            if (fileName == "")
            {
                foreach (var dir in directory.GetDirectories())
                {
                    fileName = CheckFiles(dir);
                }
            }

            return fileName;
        }

        private string CheckFiles(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
            {
                if (file.Name == "icon.ico")
                {
                    return file.FullName;
                }
            }
            return "";
        }
    }
}
