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
    /// Interaction logic for GameBoardControl.xaml
    /// </summary>
    public partial class GameBoardControl : UserControl
    {
        private GameManager _manager;

        private Ellipse[] _normalPieces;
        private Rectangle[] _kingPieces;
        private Rectangle[] _selectionRects;
        private int _pieceSize;
        private int _squareSize;

        private bool _haveSelection;
        private List<BoardPosition> _movePositions;
        private List<int> _activeSelections;

        public GameManager Manager
        {
            get { return _manager; }
            set
            {
                _manager = value;
                _manager.Board.VisualChanged += new EventHandler<VisualChangedEventArgs>(BoardVisualChanged);
                Initialize();
            }
        }

        public GameBoardControl()
        {
            InitializeComponent();
            _normalPieces = new Ellipse[GameBoard.BOARD_PIECE_COUNT];
            _kingPieces = new Rectangle[GameBoard.BOARD_PIECE_COUNT];
            _selectionRects = new Rectangle[GameBoard.BOARD_PIECE_COUNT];
            _pieceSize = (int)((boardCanvas.Width / GameBoard.BOARD_SIDE_SIZE) - (boardCanvas.Width / 50));
            _squareSize = (int)((boardCanvas.Width / GameBoard.BOARD_SIDE_SIZE));
            _movePositions = new List<BoardPosition>();
            _activeSelections = new List<int>();
        }

        private void boardCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(boardCanvas);
            int pos = ((int)p.Y / _squareSize) * GameBoard.BOARD_SIDE_SIZE + ((int)p.X / _squareSize);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectSquare(pos, false);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                SelectSquare(pos, true);
            }
        }

        private void SelectSquare(int pos, bool multi)
        {
            if (pos < 0 || pos >= GameBoard.BOARD_PIECE_COUNT) return;
            if (_manager.Board.PlayerAt(pos) == PlayerColor.None && !_haveSelection)
            {
                if (!multi && _selectionRects[pos].Visibility != System.Windows.Visibility.Visible) return;
            }

            if (_haveSelection)
            {
                _selectionRects.OnIndices(rect => rect.Visibility = System.Windows.Visibility.Hidden, _activeSelections);
                _activeSelections.Clear();

                _movePositions.Add(new BoardPosition(pos));

                try
                {
                    string[] positions = _movePositions.Select(p => p.Representation).Distinct().ToArray();

                    _manager.DoMove(true, positions);
                }
                catch (InvalidMoveException e)
                {
                    if (!string.IsNullOrEmpty(e.Message)) MessageBox.Show(e.Message, "Invalid move!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                _movePositions.Clear();

                _haveSelection = false;
            }
            else
            {
                DrawSelection(pos, multi);
                _movePositions.Add(new BoardPosition(pos));
                _haveSelection = !multi;
            }
        }

        private void Initialize()
        {
            if (_manager.Board == null) return;

            for (int i = 0; i < GameBoard.BOARD_PIECE_COUNT; ++i)
            {
                _normalPieces[i] = new Ellipse();
                _normalPieces[i].RenderTransform = new TranslateTransform();
                _normalPieces[i].Width = _normalPieces[i].Height = _pieceSize;
                
                _kingPieces[i] = new Rectangle();
                _kingPieces[i].RenderTransform = new TranslateTransform();
                _kingPieces[i].Width = _kingPieces[i].Height = _pieceSize;
                _kingPieces[i].Visibility = System.Windows.Visibility.Hidden;

                _selectionRects[i] = new Rectangle();
                _selectionRects[i].RenderTransform = new TranslateTransform();
                _selectionRects[i].Width = _selectionRects[i].Height = _squareSize;
                _selectionRects[i].Visibility = System.Windows.Visibility.Hidden;

                if (_manager.Board[i].Occupation == PlayerColor.None)
                {
                    _normalPieces[i].Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    _normalPieces[i].Visibility = System.Windows.Visibility.Visible;

                    if (_manager.Board[i].Occupation == PlayerColor.Black)
                    {
                        _normalPieces[i].Style = (Style)FindResource("BlackPieceStyle");
                    }
                    else
                    {
                        _normalPieces[i].Style = (Style)FindResource("WhitePieceStyle");
                    }
                }

                ((TranslateTransform)_normalPieces[i].RenderTransform).X = (i % GameBoard.BOARD_SIDE_SIZE) * _squareSize + (boardCanvas.Width / 75);
                ((TranslateTransform)_normalPieces[i].RenderTransform).Y = (i / GameBoard.BOARD_SIDE_SIZE) * _squareSize + (boardCanvas.Width / 75);

                ((TranslateTransform)_kingPieces[i].RenderTransform).X = (i % GameBoard.BOARD_SIDE_SIZE) * _squareSize + (boardCanvas.Width / 75);
                ((TranslateTransform)_kingPieces[i].RenderTransform).Y = (i / GameBoard.BOARD_SIDE_SIZE) * _squareSize + (boardCanvas.Width / 75);

                ((TranslateTransform)_selectionRects[i].RenderTransform).X = (i % GameBoard.BOARD_SIDE_SIZE) * _squareSize;
                ((TranslateTransform)_selectionRects[i].RenderTransform).Y = (i / GameBoard.BOARD_SIDE_SIZE) * _squareSize;

                pieceCanvas.Children.Add(_normalPieces[i]);
                pieceCanvas.Children.Add(_kingPieces[i]);
                boardCanvas.Children.Add(_selectionRects[i]);
            }
        }

        public void FullRedraw()
        {
            Redraw(Enumerable.Range(0, GameBoard.BOARD_PIECE_COUNT));
        }

        public void Redraw(IEnumerable<int> where)
        {
            foreach (int i in where)
            {
                if (_manager.Board[i].Occupation == PlayerColor.None)
                {
                    _normalPieces[i].Visibility = System.Windows.Visibility.Hidden;
                    _kingPieces[i].Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    if (_manager.Board[i].Piece == PieceType.Normal)
                    {
                        _normalPieces[i].Visibility = System.Windows.Visibility.Visible;

                        if (_manager.Board[i].Occupation == PlayerColor.Black)
                        {
                            _normalPieces[i].Style = (Style)FindResource("BlackPieceStyle");
                        }
                        else
                        {
                            _normalPieces[i].Style = (Style)FindResource("WhitePieceStyle");
                        }
                    }
                    else if (_manager.Board[i].Piece == PieceType.King)
                    {
                        _normalPieces[i].Visibility = System.Windows.Visibility.Hidden;
                        _kingPieces[i].Visibility = System.Windows.Visibility.Visible;

                        if (_manager.Board[i].Occupation == PlayerColor.Black)
                        {
                            _kingPieces[i].Style = (Style)FindResource("BlackPieceStyle");
                        }
                        else
                        {
                            _kingPieces[i].Style = (Style)FindResource("WhitePieceStyle");
                        }
                    }
                }
            }
        }

        private void DrawSelection(int pos, bool multi)
        {
            if (!multi) _selectionRects[pos].Fill = (DrawingBrush)FindResource("SquareSelectionBrush");
            else _selectionRects[pos].Fill = (DrawingBrush)FindResource("SquareMultiSelectionBrush");

            _selectionRects[pos].Visibility = System.Windows.Visibility.Visible;
            _activeSelections.Add(pos);
        }

        private void BoardVisualChanged(object sender, VisualChangedEventArgs e)
        {
            Redraw(e.ChangedIndices);            
        }
    }
}
