﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using GraphicsEngine;
using TheGame;
using System.Globalization;

namespace BilardGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
        string player1name = "Player 1";
        string player2name = "Player 2";
        bool isPlayer1Playing = true;
        bool isPlayer2Playing = false;
        public string Player1Name
        {
            get => player1name;
            set
            {
                player1name = value;
                OnPropertyChanged(nameof(Player1Name));
            }
        }
        public string Player2Name
        {
            get => player2name;
            set
            {
                player2name = value;
                OnPropertyChanged(nameof(Player2Name));
            }
        }
        public bool IsPlayer1Playing
        {
            get => isPlayer1Playing;
            set
            {
                isPlayer1Playing = value;
                if (isPlayer1Playing) IsPlayer2Playing = false;
                OnPropertyChanged(nameof(IsPlayer1Playing));
            }
        }
        public bool IsPlayer2Playing
        {
            get => isPlayer2Playing;
            set
            {
                isPlayer2Playing = value;
                if (isPlayer2Playing) isPlayer1Playing = false;
                OnPropertyChanged(nameof(IsPlayer2Playing));
            }
        }
        Resolution res = new Resolution(1200, 800);
        uint[,] colors;
        Game game = new Game(new Resolution(1200, 800));
        BackgroundWorker worker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            img.Source = BitmapExtensions.CreateBitmap(res);
            game = new Game(res);
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (img.Source as WriteableBitmap).FillBitmap(colors);
            game.Update();
            if (game.Player1Playng)
            {
                IsPlayer1Playing = true;
                IsPlayer2Playing = false;
            }
            else
            {
                IsPlayer1Playing = false;
                IsPlayer2Playing = true;
            }
            if(game.IsGameFinished)
            {
                var winner = new Winner(IsPlayer1Playing ? Player1Name : Player2Name);
                winner.Owner = this;
                winner.ShowDialog();
                game = new Game(res);
            }
            if(!game.IsGameFinished) worker.RunWorkerAsync();
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            colors = game.Display();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    game.RotateStickLeft();
                    break;
                case Key.Right:
                    game.RotateStickRigth();
                    break;
                case Key.H:
                    game.HoldStick();
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.H:
                    game.ReleaseStick();
                    break;
            }
        }

        #region Button handlers
        private void StaticCamera_Checked(object sender, RoutedEventArgs e)
        {
            game.StaticCamera();
        }

        private void ActiveCamera_Checked(object sender, RoutedEventArgs e)
        {
            game.ActiveCamera();
        }

        private void TrackingCamera_Checked(object sender, RoutedEventArgs e)
        {
            game.TrackingCamera();
        }

        private void PointLight_Checked(object sender, RoutedEventArgs e)
        {
            game.SwitchPointLight();
        }

        private void TrackingLight_Checked(object sender, RoutedEventArgs e)
        {
            game.SwitchTrackingLight();
        }

        private void ConstantShading_Checked(object sender, RoutedEventArgs e)
        {
            game.ConstantShading();
        }

        private void GouraudShading_Checked(object sender, RoutedEventArgs e)
        {
            game.GouraudShading();
        }

        private void PhongShading_Checked(object sender, RoutedEventArgs e)
        {
            game.PhongShading();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            SettingsMenu.Visibility = Visibility.Visible;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            SettingsMenu.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            game = new Game(res);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Owner = this;
            about.ShowDialog();
        }

        private void Instruction_Click(object sender, RoutedEventArgs e)
        {
            var instruction = new Instruction();
            instruction.Owner = this;
            instruction.ShowDialog();
        }

        private void ChangeNicknames_Click(object sender, RoutedEventArgs e)
        {
            var nicknames = new Nicknames();
            nicknames.Owner = this;
            nicknames.DataContext = this;
            nicknames.ShowDialog();
        }
        #endregion
    }
    class BorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return Brushes.DarkRed;
            else return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
