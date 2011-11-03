using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DebugConsole.xaml
    /// </summary>
    public partial class DebugConsole : Window
    {
        private class ConsoleWriter : TextWriter
        {
            private DebugConsole _console;

            public ConsoleWriter(DebugConsole console)
            {
                _console = console;
            }

            public override Encoding Encoding
            {
                get { return Encoding.Default; }
            }

            public override void WriteLine()
            {
                _console.WriteLine();
            }

            public override void WriteLine(string value)
            {
                _console.WriteLine(value);
            }
        }

        private static string[] ValidCommands = { "newgame", "move", "autoprint" };

        private GameManager _manager;
        private bool _autoPrint = true;
        private TextWriter _out;

        public TextWriter Out
        {
            get { return _out; }
        }

        public DebugConsole(GameManager manager)
        {
            InitializeComponent();
            _manager = manager;
            _out = new ConsoleWriter(this);
            WriteLine("Gothic Checkers v" + GameManager.Version);
            WriteLine("For help, type help in the command line!");
            WriteLine();

            MoveDebugWindow w = new MoveDebugWindow(manager);
            w.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                string command = txtCommand.Text;
                txtCommand.Text = string.Empty;
                ParseCommand(command);
            }
        }

        private void ParseCommand(string command)
        {
            if (command.ToLower().Trim() == "printboard")
            {
                PrintGameBoard();
            }
            else if (GameUtils.HelpRegex.IsMatch(command))
            {
                PrintHelp(GameUtils.HelpRegex.Replace(command, "").Trim());
            }
            else if (GameUtils.MoveRegex.IsMatch(command))
            {
                DoMove(command);
            }
            
        }

        private void PrintHelp(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                WriteLine("Gothic Checkers " + Localization.ConsoleHelpStrings.Help_Help);
                WriteLine(Localization.ConsoleHelpStrings.Help_CommandList);
                WriteLine("newgame - " + Localization.ConsoleHelpStrings.Help_CommandList_NewGame);
                WriteLine("move - " + Localization.ConsoleHelpStrings.Help_CommandList_Move);
                WriteLine("autoprint <on/off> - " + Localization.ConsoleHelpStrings.Help_CommandList_AutoPrint);
                WriteLine(Localization.ConsoleHelpStrings.Help_CommandList_MoreHelp);
                WriteLine();
                WriteLine();
            }
            else
            {
                string lowarg = arg.ToLower();
                if (ValidCommands.Contains(lowarg))
                {
                    switch (lowarg)
                    {
                        case "newgame":
                            {
                                WriteLine("newgame:\r" + Localization.ConsoleHelpStrings.Help_CommandDetail_NewGame);
                                WriteLine(Localization.ConsoleHelpStrings.Help_CommandDetail_NewGame_Example1);
                                WriteLine(Localization.ConsoleHelpStrings.Help_CommandDetail_NewGame_Example2);
                            } break;
                        case "move":
                            {
                                WriteLine("move:\r" + Localization.ConsoleHelpStrings.Help_CommandDetail_Move);
                            } break;
                    }
                }
            }
        }

        private void DoMove(string move)
        {
            var positions = GameUtils.BoardPositionRegex.Matches(move);

            try
            {
                _manager.DoMove(true, positions.Cast<Match>().Select(match => match.Value).ToArray());

                if (_autoPrint) PrintGameBoard();
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }
        }

        private void PrintGameBoard()
        {
            string ascii = _manager.Board.AsAsciiPicture();
            Paragraph p = new Paragraph();
            p.Inlines.Add(ascii);
            p.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            p.LineHeight = 8;
            txtConsole.Document.Blocks.Add(p);
            WriteLine();
            WriteLine();
            txtConsole.ScrollToEnd();
        }

        private void WriteLine()
        {
            txtConsole.AppendText("\r");
            txtConsole.ScrollToEnd();
        }

        private void WriteLine(string line)
        {
            txtConsole.AppendText(line);
            WriteLine();
        }
    }
}
