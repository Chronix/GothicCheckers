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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GothicCheckers.GUI
{
    /// <summary>
    /// Interaction logic for GameBoardUnit.xaml
    /// </summary>
    public partial class GameBoardUnit : UserControl
    {
        public static readonly DependencyProperty UnitColorProperty = DependencyProperty.Register("UnitColor", typeof(Brush), typeof(GameBoardUnit), new FrameworkPropertyMetadata(Brushes.Black));

        private bool _isMulti = false;

        public GameBoardUnit()
        {
            InitializeComponent();
            SelectionRect.Fill = (Brush)FindResource("SquareSelectionBrush");
        }

        public Brush UnitColor
        {
            get { return (Brush)GetValue(UnitColorProperty); }
            set { SetValue(UnitColorProperty, value); }
        }

        public int UnitIndex { get; set; }

        public void SetNormalPieceVisibility(Visibility visibility)
        {
            NormalPiece.Visibility = visibility;
        }

        public void SetKingPieceVisibility(Visibility visibility)
        {
            KingPiece.Visibility = visibility;
        }

        public void ShowNormalPiece()
        {
            SetNormalPieceVisibility(System.Windows.Visibility.Visible);
            SetKingPieceVisibility(System.Windows.Visibility.Hidden);
        }

        public void ShowKingPiece()
        {
            SetKingPieceVisibility(System.Windows.Visibility.Visible);
            SetNormalPieceVisibility(System.Windows.Visibility.Hidden);
        }

        public void HidePieces()
        {
            SetNormalPieceVisibility(System.Windows.Visibility.Hidden);
            SetKingPieceVisibility(System.Windows.Visibility.Hidden);
        }

        public void ShowSelectionRect(bool multi = false)
        {
            if (multi)
            {
                if (!_isMulti)
                {
                    SelectionRect.Fill = (Brush)FindResource("SquareMultiSelectionBrush");
                    _isMulti = true;
                }                
            }
            else
            {
                if (_isMulti)
                {
                    SelectionRect.Fill = (Brush)FindResource("SquareSelectionBrush");
                    _isMulti = false;
                }
            }

            SelectionRect.Visibility = System.Windows.Visibility.Visible;
        }

        public void HideSelectionRect()
        {
            SelectionRect.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
