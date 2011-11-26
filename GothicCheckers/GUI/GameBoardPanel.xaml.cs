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
        private bool _selecting;

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
        }

        public void FullRedraw()
        {
        }

        private void Initialize()
        {
            for (int i = 0; i < GameBoard.BOARD_PIECE_COUNT; ++i)
            {
                _units[i] = InitUnit(i);
                UnitGrid.Children.Add(_units[i]);
            }
        }

        private GameBoardUnit InitUnit(int index)
        {
            GameBoardUnit unit = new GameBoardUnit();
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

        private void GameUnit_MouseDown(object sender, MouseEventArgs args)
        {
            GameBoardUnit unit = args.Source as GameBoardUnit;

            if (args.LeftButton == MouseButtonState.Pressed)
            {
                if (!_selecting)
                {
                    unit.ShowSelectionRect();
                    _selecting = true;
                }
                else
                {
                    _selecting = false;
                    unit.HideSelectionRect();
                }
            }
        }
    }
}
