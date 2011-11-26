using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using GothicCheckers;

namespace GothicCheckers.GUI
{
    /// <summary>
    /// Interaction logic for GameSettingsWindow.xaml
    /// </summary>
    public partial class GameSettingsWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int newLong);

        private const int GW_STYLE = -0x10;
        private const int WS_REMOVE_MAXIMIZEBOX = -0x00010001;

        public GameSettingsWindow(GameManager manager)
        {
            InitializeComponent();
            this.DataContext = manager;
        }

        // "vypnuti" maximize tlacitka
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr wnd = new WindowInteropHelper(this).Handle;
            int winStyle = GetWindowLong(wnd, GW_STYLE);
            winStyle &= WS_REMOVE_MAXIMIZEBOX;
            SetWindowLong(wnd, GW_STYLE, winStyle);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ((GameManager)DataContext).Reset();
            UpdateBindings();
            DialogResult = true;
            Close();
        }

        private void UpdateBindings()
        {
            BindingExpression[] expressions = new BindingExpression[]
            {
                rbPlayer1ControlHuman.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer1ControlComputer.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer1DifficultyEasy.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer1DifficultyNormal.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer1DifficultyHard.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer2ControlHuman.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer2ControlComputer.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer2DifficultyEasy.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer2DifficultyNormal.GetBindingExpression(RadioButton.IsCheckedProperty),
                rbPlayer2DifficultyHard.GetBindingExpression(RadioButton.IsCheckedProperty)
            };

            foreach (var ex in expressions) ex.UpdateSource();
        }
    }
}
