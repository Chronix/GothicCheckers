using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GothicCheckers.GUI
{
    /// <summary>
    /// Interaction logic for GameBoardPanel.xaml
    /// </summary>
    public partial class GameBoardPanel : UserControl
    {
        private static readonly int[] _darkFields = { 1, 3, 5, 7, 8, 10, 12, 14, 17, 19, 21, 23, 24, 26, 28, 30, 33, 35, 37, 39, 40, 42, 44, 46, 49, 51, 53, 55, 56, 58, 60, 62 };

        private GameManager _manager;
        private GameBoardUnit[] _units;

        private List<int> _activeSelections;
        private List<BoardPosition> _movePositions;

        public GameManager Manager
        {
            get { return _manager; }
            set
            {
                _manager = value;
                Initialize();
            }
        }

        public GameBoardPanel()
        {
            InitializeComponent();
            _units = new GameBoardUnit[GameBoard.BOARD_PIECE_COUNT];
            _activeSelections = new List<int>();
            _movePositions = new List<BoardPosition>();
        }

        public void ClearSelections()
        {
            _activeSelections.ForEach(i => _units[i].HideSelectionRect());
            _activeSelections.Clear();
            _movePositions.Clear();
        }

        public void FullRedraw()
        {
            Redraw(Enumerable.Range(0, 64));
        }

        public void Redraw(IEnumerable<int> where)
        {
            foreach (int i in where)
            {
                RedrawUnit(i);
            }
        }

        private void RedrawUnit(int index)
        {
            GameField field = _manager.Board[index];

            switch (field.Occupation)
            {
                case PlayerColor.Black: _units[index].UnitColor = Brushes.Black; break;
                case PlayerColor.White: _units[index].UnitColor = Brushes.White; break;
                case PlayerColor.None: _units[index].HidePieces(); _units[index].HideSelectionRect(); return;
            }
            
            if (field.Piece == PieceType.King)
            {
                _units[index].ShowKingPiece();
            }
            else
            {
                _units[index].ShowNormalPiece();
            }

            _units[index].HideSelectionRect();
        }

        private void Initialize()
        {
            _manager.Board.VisualChanged += new EventHandler<VisualChangedEventArgs>(Board_VisualChanged);

            for (int i = 0; i < GameBoard.BOARD_PIECE_COUNT; ++i)
            {
                _units[i] = InitUnit(i);
                UnitGrid.Children.Add(_units[i]);
            }
        }

        private GameBoardUnit InitUnit(int index)
        {
            GameBoardUnit unit = new GameBoardUnit();
            unit.UnitIndex = index;
            unit.Background = GetUnitBackground(index);

            switch (_manager.Board[index].Occupation)
            {
                case PlayerColor.Black: unit.UnitColor = Brushes.Black; break;
                case PlayerColor.White: unit.UnitColor = Brushes.White; break;
                default: unit.HidePieces(); break;
            }

            unit.MouseDown += GameUnit_MouseDown;
            return unit;
        }

        private Brush GetUnitBackground(int index)
        {
            if (_darkFields.Contains(index)) return (Brush)FindResource("BlackSquareBackgroundBrush");
            else return (Brush)FindResource("WhiteSquareBackgroundBrush");
        }

        private void SelectUnit(GameBoardUnit unit)
        {
            if (_activeSelections.Count == 0)
            {
                if (_manager.Board[unit.UnitIndex].Occupation == PlayerColor.None) return;
                unit.ShowSelectionRect(true);
                _activeSelections.Add(unit.UnitIndex);
                _movePositions.Add(new BoardPosition(unit.UnitIndex));
            }
            else
            {
                if (_manager.Board[unit.UnitIndex].Occupation != PlayerColor.None) return;
                _movePositions.Add(new BoardPosition(unit.UnitIndex));
                DoCurrentMove();
            }
        }

        private void SelectUnitMulti(GameBoardUnit unit)
        {
            if (_activeSelections.Count == 0)
            {
                if (_manager.Board[unit.UnitIndex].Occupation == PlayerColor.None) return;
                unit.ShowSelectionRect(true);
                _activeSelections.Add(unit.UnitIndex);
                _movePositions.Add(new BoardPosition(unit.UnitIndex));
            }
            else
            {
                if (_manager.Board[unit.UnitIndex].Occupation != PlayerColor.None) return;
                unit.ShowSelectionRect(true);
                _activeSelections.Add(unit.UnitIndex);
                _movePositions.Add(new BoardPosition(unit.UnitIndex));
            }
        }

        private void DoCurrentMove()
        {
            string[] positions = _movePositions.Select(pos => pos.Representation).Distinct().ToArray();

            try
            {
                _manager.DoMove(true, positions);
            }
            catch (InvalidMoveException e)
            {
                if (!string.IsNullOrEmpty(e.Message)) MessageBox.Show(e.Message, "Invalid move!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            ClearSelections();
        }

        private void GameUnit_MouseDown(object sender, MouseEventArgs args)
        {
            GameBoardUnit unit = args.Source as GameBoardUnit;

            if (args.LeftButton == MouseButtonState.Pressed)
            {
                SelectUnit(unit);
            }
            else if (args.RightButton == MouseButtonState.Pressed)
            {
                SelectUnitMulti(unit);
            }
        }

        private void Board_VisualChanged(object sender, VisualChangedEventArgs e)
        {
            Redraw(e.ChangedIndices);
        }
    }
}
