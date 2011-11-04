using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public class GameManager : INotifyPropertyChanged
    {
        private GameReplayState _replayState;
        private GameBoard _board;

        public GameHistory History { get; private set; }
        public GameType GameType { get; set; }
        public PlayerColor CurrentPlayer { get; set; }
        public static GameDifficulty WhiteDifficulty { get; set; }
        public static GameDifficulty BlackDifficulty { get; set; }
        public PlayerControlType WhiteControl { get; set; }
        public PlayerControlType BlackControl { get; set; }

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

            CurrentPlayer = PlayerColor.White;

#if DEBUG
            LastTurnValidMoves = new ObservableCollection<IMove>();
#endif
        }

        public void Reset()
        {
            History.Clear();

            _board = new GameBoard();
            OnPropertyChanged("Board");
        }

        public void DoMove(bool addToHistory, params string[] positions)
        {
            ExceptionProvider.ThrowIfNull(positions, GUI.Localization.ErrorMessages.InvalidMoveNotEnoughPositions);
            ExceptionProvider.ThrowInvalidMoveIf(positions.Length < 2, string.Empty);
            IMove move = null;

            if (positions.Length == 2)
            {
                move = new SimpleMove(_board[positions[0]].Occupation, positions[0], positions[1]);
            }
            else
            {
                move = CompoundMove.FromPositions(_board[positions[0]].Occupation, positions[0], positions.Last(), positions.Where((pos, i) => i > 0 && i < positions.Length - 1).ToArray());
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
        }

        public void PlayHistory()
        {
            foreach (var move in History) _board.DoMove(move, true);
        }

        private void SwapPlayers()
        {
            CurrentPlayer = GameUtils.OtherPlayer(CurrentPlayer);
        }

        private bool IsPlayersTurn(PlayerColor movePlayer)
        {
            return CurrentPlayer == movePlayer;
        }

        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
