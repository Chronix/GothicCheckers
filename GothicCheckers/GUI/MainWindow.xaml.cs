﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Win32;

namespace GothicCheckers.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameManager _manager;
        private DebugConsole _debugConsole; 

        public GameManager GameManager
        {
            get { return _manager; }
        }

        public MainWindow()
        {
            _manager = new GameManager();
            this.DataContext = _manager;

            InitializeComponent();

            _manager.GameEnded += new EventHandler<PlayerEventArgs>(_manager_GameEnded);
            _manager.PlayersSwapped += new EventHandler<PlayerEventArgs>(_manager_PlayersSwapped);
            _manager.MoveDone += new EventHandler<MoveDoneEventArgs>(_manager_MoveDone);

            SaveLoadManager.OldFormatLoading += new EventHandler(SaveLoadManager_OldFormatLoading);

            MainGameBoard.Manager = _manager;

            try
            {
                GameUtils.EnsureSaveDirectory();
            }
            catch
            {
                MessageBox.Show(Localization.ErrorMessages.SaveDirectoryError, Localization.ErrorMessages.CaptionError, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region MAIN MENU COMMAND HANDLERS
        private void Command_Execute_New(object sender, ExecutedRoutedEventArgs args)
        {
            GameSettingsWindow gsw = new GameSettingsWindow(_manager);
            gsw.Owner = this;
            gsw.ShowDialog();
            SetCurrentPlayerStatusText(PlayerColor.White);
            MainGameBoard.FullRedraw();
            _manager.StartGame();
        }

        private void Command_Execute_Load(object sender, ExecutedRoutedEventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Saved games (.xml)|*.xml";
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save");

            if (ofd.ShowDialog().Value)
            {
                _manager.Reset();

                try
                {
                    SaveLoadManager.LoadGame(ofd.FileName, ref _manager);
                    
                }
                catch
                {
                    MessageBox.Show(Localization.ErrorMessages.InvalidSaveFile, Localization.ErrorMessages.CaptionError, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _manager.PlayHistory();
                MainGameBoard.FullRedraw();
                lbHistory.ScrollIntoView(lbHistory.Items[lbHistory.Items.Count - 1]);
            }
        }

        private void Command_Execute_Save(object sender, ExecutedRoutedEventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Saved Games (.xml)|*.xml";
            sfd.AddExtension = true;
            sfd.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save");

            if (sfd.ShowDialog().Value)
            {
                SaveLoadManager.SaveGame(sfd.FileName, _manager);
            }
        }

        private void Command_Execute_Exit(object sender, ExecutedRoutedEventArgs args)
        {
            Environment.Exit(0);
        }

        private void Command_CanExecute_ShowDebugConsole(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = _debugConsole == null || (_debugConsole != null && _debugConsole.Visibility == System.Windows.Visibility.Hidden);
        }

        private void Command_Execute_ShowDebugConsole(object sender, ExecutedRoutedEventArgs args)
        {
            if (_debugConsole == null)
            {
                _debugConsole = new DebugConsole(_manager);
            }

            _debugConsole.Show();
        }

        private void Command_Execute_LanguageSwitch(object sender, ExecutedRoutedEventArgs args)
        {
            string newCulture = ((MenuItem)args.OriginalSource).Tag.ToString();
        }

        private void Command_Execute_ShowGameSettings(object sender, ExecutedRoutedEventArgs args)
        {
            GameSettingsWindow gsw = new GameSettingsWindow(_manager);
            gsw.Owner = this;
            gsw.ShowDialog();
        }

        private void Command_Execute_SwitchPlayers(object sender, ExecutedRoutedEventArgs args)
        {
            
        }

        private void Command_Execute_UndoTurn(object sender, ExecutedRoutedEventArgs args)
        {

        }

        private void Command_Execute_RedoTurn(object sender, ExecutedRoutedEventArgs args)
        {

        }

        private void Command_Execute_Help(object sender, ExecutedRoutedEventArgs args)
        {
            string commParam = args.Parameter.ToString();

            if (commParam == "suggest")
            {
                _manager.SuggestMove();
            }
        }
        #endregion

        #region OTHER UI ELEMENTS COMMAND HANDLERS
        private void Command_Execute_TogglePauseMode(object sender, ExecutedRoutedEventArgs args)
        {
            _manager.GameIsPaused = !_manager.GameIsPaused;
        }

        private void Command_CanExecute_TogglePauseMode(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = _manager.GameCanBePaused;
        }

        private void Command_Execute_MoveInHistory(object sender, ExecutedRoutedEventArgs args)
        {
            
        }

        private void Command_CanExecute_MoveInHistory(object sender, CanExecuteRoutedEventArgs args)
        {
            if (args.Parameter == null) return;

            if (int.Parse(args.Parameter.ToString()) == -1)
            {
                if (lbHistory.SelectedIndex == 0 || lbHistory.SelectedIndex == -1)
                {
                    args.CanExecute = false;
                }
                else
                {
                    args.CanExecute = true;
                }
            }
            else
            {
                if (lbHistory.SelectedIndex == lbHistory.Items.Count - 1)
                {
                    args.CanExecute = false;
                }
                else
                {
                    args.CanExecute = true;
                }
            }
        }

        private void Command_Execute_ToggleReplayMode(object sender, ExecutedRoutedEventArgs args)
        {

        }

        private void Command_CanExecute_ToggleReplayMode(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = _manager.ReplayState != GameReplayState.NotAvailable;
        }

        private void Command_Execute_StopGameReplay(object sender, ExecutedRoutedEventArgs args)
        {

        }

        private void Command_CanExecute_StopGameReplay(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = _manager.ReplayState == GameReplayState.Playing || _manager.ReplayState == GameReplayState.Paused;
        }
        #endregion

        #region EVENT HANDLERS
        private void HistoryItem_Click(object sender, RoutedEventArgs args)
        {
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) MainGameBoard.ClearSelections();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainGameBoard.FullRedraw();
        }

        private void _manager_GameEnded(object sender, PlayerEventArgs e)
        {
            if (e.Player == PlayerColor.None) MessageBox.Show(Localization.MainWindowStrings.MainWindow_GameEnded_Draw, Localization.MainWindowStrings.MainWindow_Draw, MessageBoxButton.OK, MessageBoxImage.Information);
            else MessageBox.Show(string.Format(Localization.MainWindowStrings.MainWIndow_GameEnded_Win, e.Player == PlayerColor.White ? Localization.MainWindowStrings.MainWindow_Player_White : Localization.MainWindowStrings.MainWindow_Player_Black), Localization.MainWindowStrings.MainWindow_Victory, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void _manager_PlayersSwapped(object sender, PlayerEventArgs e)
        {
            SetCurrentPlayerStatusText(e.Player);
        }

        private void _manager_MoveDone(object sender, MoveDoneEventArgs e)
        {
            if (e.SuggestingMove)
            {
                MessageBox.Show(string.Format(Localization.MainWindowStrings.MainWindow_BestMoveText, e.Move.ToString()), Localization.MainWindowStrings.MainWindow_BestMoveCaption, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                lbHistory.ScrollIntoView(lbHistory.Items[lbHistory.Items.Count - 1]);
            }
        }

        private void SaveLoadManager_OldFormatLoading(object sender, EventArgs e)
        {
            MessageBox.Show(Localization.ErrorMessages.SaveFormatWarning, Localization.ErrorMessages.CaptionWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void SetCurrentPlayerStatusText(PlayerColor player)
        {
            switch (player)
            {
                case PlayerColor.White: tbCurrentPlayer.Text = Localization.MainWindowStrings.MainWindow_Status_CurrentPlayer + " " + Localization.MainWindowStrings.MainWindow_Player_White; break;
                case PlayerColor.Black: tbCurrentPlayer.Text = Localization.MainWindowStrings.MainWindow_Status_CurrentPlayer + " " + Localization.MainWindowStrings.MainWindow_Player_Black; break;
            }
        }

        private void SuggestMove()
        {
            
        }
    }
}
