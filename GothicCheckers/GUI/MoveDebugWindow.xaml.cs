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
using System.Windows.Shapes;

namespace GothicCheckers.GUI
{
    /// <summary>
    /// Interaction logic for MoveDebugWindow.xaml
    /// </summary>
    public partial class MoveDebugWindow : Window
    {
        private GameManager manager;

        public MoveDebugWindow(GameManager manager)
        {
            InitializeComponent();
            this.manager = manager;
            this.DataContext = this.manager;
        }
    }
}
