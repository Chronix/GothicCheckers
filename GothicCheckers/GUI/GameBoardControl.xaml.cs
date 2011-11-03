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
        private Ellipse[] _pieces;
        private int _pieceSize;
        private int _squareSize;
        private GameManager _manager;

        private bool _haveSelection;
        private List<BoardPosition> _movePositions;

        public GameManager Manager
        {
            get { return _manager; }
            set
            {
                _manager = value;
                _manager.Board.VisualChanged += new EventHandler<VisualChangedEventArgs>(BoardVisualChanged);
                FullRedraw();
            }
        }

        public GameBoardControl()
        {
            InitializeComponent();
            _pieces = new Ellipse[GameBoard.BOARD_PIECE_COUNT];
            _pieceSize = (int)((boardCanvas.Width / GameBoard.BOARD_SIDE_SIZE) - (boardCanvas.Width / 50));
            _squareSize = (int)((boardCanvas.Width / GameBoard.BOARD_SIDE_SIZE));
            _movePositions = new List<BoardPosition>();
        }

        private void boardCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(boardCanvas);
                int pos = ((int)p.Y / _squareSize) * GameBoard.BOARD_SIDE_SIZE + ((int)p.X / _squareSize);
                SelectSquare(pos);
            }
        }

        private void SelectSquare(int pos)
        {
            if (pos < 0 || pos >= GameBoard.BOARD_PIECE_COUNT) return;
            if ((_manager.CurrentPlayer != _manager.Board.PlayerAt(pos) && !_haveSelection) || (_manager.Board.PlayerAt(pos) == PlayerColor.None && !_haveSelection )) return;

            if (_haveSelection)
            {
                squareSelection.Visibility = System.Windows.Visibility.Hidden;

                _movePositions.Add(new BoardPosition(pos));

                try
                {
                    _manager.DoMove(true, _movePositions.Select(p => p.Representation).ToArray());
                }
                catch (InvalidMoveException e)
                {
                    MessageBox.Show(e.Message, "Invalid move!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

                _movePositions.Clear();

                _haveSelection = false;
            }
            else
            {
                DrawSelection(pos);
                _movePositions.Add(new BoardPosition(pos));
                _haveSelection = true;
            }
        }

        private void FullRedraw()
        {
            if (_manager.Board == null) return;

            for (int i = 0; i < GameBoard.BOARD_PIECE_COUNT; ++i)
            {
                _pieces[i] = new Ellipse();
                _pieces[i].RenderTransform = new TranslateTransform();
                _pieces[i].Width = _pieces[i].Height = _pieceSize;

                if (_manager.Board[i].Occupation == PlayerColor.None)
                {
                    _pieces[i].Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    _pieces[i].Visibility = System.Windows.Visibility.Visible;

                    if (_manager.Board[i].Occupation == PlayerColor.Black)
                    {
                        _pieces[i].Style = (Style)FindResource("BlackPieceStyle");
                    }
                    else
                    {
                        _pieces[i].Style = (Style)FindResource("WhitePieceStyle");
                    }
                }

                (_pieces[i].RenderTransform as TranslateTransform).X = (i % GameBoard.BOARD_SIDE_SIZE) * _squareSize + (boardCanvas.Width / 75);
                (_pieces[i].RenderTransform as TranslateTransform).Y = (i / GameBoard.BOARD_SIDE_SIZE) * _squareSize + (boardCanvas.Width / 75);
                pieceCanvas.Children.Add(_pieces[i]);
            }
        }

        private void Redraw(int[] where)
        {
            foreach (int i in where)
            {
                if (_manager.Board[i].Occupation == PlayerColor.None)
                {
                    _pieces[i].Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    _pieces[i].Visibility = System.Windows.Visibility.Visible;

                    if (_manager.Board[i].Occupation == PlayerColor.Black)
                    {
                        _pieces[i].Style = (Style)FindResource("BlackPieceStyle");
                    }
                    else
                    {
                        _pieces[i].Style = (Style)FindResource("WhitePieceStyle");
                    }
                }
            }
        }

        private void DrawSelection(int pos)
        {
            (squareSelection.RenderTransform as TranslateTransform).X = (pos % GameBoard.BOARD_SIDE_SIZE) * _squareSize;
            (squareSelection.RenderTransform as TranslateTransform).Y = (pos / GameBoard.BOARD_SIDE_SIZE) * _squareSize;
            squareSelection.Visibility = System.Windows.Visibility.Visible;
        }

        private void BoardVisualChanged(object sender, VisualChangedEventArgs e)
        {
            Redraw(e.ChangedIndices);            
        }
    }
}
