﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

using GothicCheckers.AI;

namespace GothicCheckers
{
    public class GameManager : INotifyPropertyChanged
    {
        private GameReplayState _replayState;
        private GameBoard _board;
        private IAIEngine _aiEngine;

        public GameHistory History { get; private set; }
        public GameType GameType { get; set; }
        public PlayerColor CurrentPlayer { get; set; }
        public static GameDifficulty WhiteDifficulty { get; set; }
        public static GameDifficulty BlackDifficulty { get; set; }
        public PlayerControlType WhiteControl { get; set; }
        public PlayerControlType BlackControl { get; set; }

        public event EventHandler<PlayerEventArgs> GameEnded;
        public event EventHandler<PlayerEventArgs> PlayersSwapped;

#if DEBUG
        public ObservableCollection<IMove> LastTurnValidMoves { get; private set; }
#endif

        public GameReplayState ReplayState
        {
            get { return _replayState; }
            set
            {
                if (_replayState != value)
                {
                    _replayState = value;
                    OnPropertyChanged("ReplayState");
                }
            }
        }

        public GameBoard Board
        {
            get { return _board; }
        }

        public bool GameCanBePaused { get; set; }
        public bool GameIsPaused { get; set; }

        public static string Version
        {
            get { return "1.0"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameManager()
        {
            History = new GameHistory();
            _board = new GameBoard();
            _aiEngine = new MinimaxAB();

            _aiEngine.BestMoveChosen += new EventHandler(_aiEngine_BestMoveChosen);

            CurrentPlayer = PlayerColor.White;

#if DEBUG
            LastTurnValidMoves = new ObservableCollection<IMove>();
#endif
        }

        public void Reset()
        {
            History.Clear();
            _board.Reset();
            CurrentPlayer = PlayerColor.White;
        }

        public void DoMove(bool addToHistory, params string[] positions)
        {
            ExceptionProvider.ThrowIfNull(positions, GUI.Localization.ErrorMessages.InvalidMoveNotEnoughPositions);
            ExceptionProvider.ThrowInvalidMoveIf(positions.Length < 2, string.Empty);
            IMove move = null;

            if (positions.Length == 2)
            {
                move = new SimpleMove(_board[positions[0]].Occupation, positions[0], positions[1], _board[positions[0]].Piece == PieceType.King, BoardPosition.GetPositionBetween(positions[0], positions[1]) != BoardPosition.Invalid);
            }
            else
            {
                move = CompoundMove.FromPositions(_board[positions[0]].Occupation, positions[0], positions.Last(), _board[positions[0]].Piece == PieceType.King, positions.Where((pos, i) => i > 0 && i < positions.Length - 1).ToArray());
            }

            ExceptionProvider.ThrowInvalidMoveIf(!IsPlayersTurn(move.Player), GUI.Localization.ErrorMessages.WaitYourTurn);

#if DEBUG
            LastTurnValidMoves.Clear();
            LastTurnValidMoves.AddRange(RuleEngine.GetAllMovesForPlayer(_board, move.Player));
#endif

            ExceptionProvider.ThrowInvalidMoveIf(!RuleEngine.ValidateMove(_board, move), GUI.Localization.ErrorMessages.InvalidMoveGeneric);

            _board.DoMove(move);
            SwapPlayers();

            if (addToHistory)
            {
                History.Add(move);
            }

            Play();
        }

        public void PlayHistory()
        {
            for (int i = 1; i < History.Count; ++i) _board.DoMove(History[i], true);
        }

        public void StartGame()
        {
            if (WhiteControl == PlayerControlType.Human && BlackControl == PlayerControlType.Human) GameType = GothicCheckers.GameType.DoubleHuman;
            else if ((WhiteControl == PlayerControlType.Human && BlackControl == PlayerControlType.Computer) ||
                     (WhiteControl == PlayerControlType.Computer && BlackControl == PlayerControlType.Human)) GameType = GothicCheckers.GameType.HumanAI;
            else GameType = GothicCheckers.GameType.DoubleAI;

            Play();
        }

        public void Play()
        {
            PlayerColor winner;
            if (RuleEngine.CheckForGameEnd(_board, out winner)) OnGameEnded(winner);

            if (CurrentPlayer == PlayerColor.White && WhiteControl == PlayerControlType.Computer)
            {
                _aiEngine.ComputeBestMove(_board, CurrentPlayer, (int)WhiteDifficulty);
            }
            else if (CurrentPlayer == PlayerColor.Black && BlackControl == PlayerControlType.Computer)
            {
                _aiEngine.ComputeBestMove(_board, CurrentPlayer, (int)BlackDifficulty);
            }
        }

        private void _aiEngine_BestMoveChosen(object sender, EventArgs e)
        {
            _board.DoMove(_aiEngine.BestMove);
            History.Add(_aiEngine.BestMove);
            SwapPlayers();
            Play();
        }

        private void SwapPlayers()
        {
            CurrentPlayer = GameUtils.OtherPlayer(CurrentPlayer);
            OnPlayersSwapped(CurrentPlayer);
        }

        private bool IsPlayersTurn(PlayerColor movePlayer)
        {
            return CurrentPlayer == movePlayer;
        }

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private void OnGameEnded(PlayerColor winner)
        {
            if (GameEnded != null) GameEnded(this, new PlayerEventArgs(winner));
        }

        private void OnPlayersSwapped(PlayerColor newPlayer)
        {
            if (PlayersSwapped != null) PlayersSwapped(this, new PlayerEventArgs(newPlayer));
        }
    }
}
