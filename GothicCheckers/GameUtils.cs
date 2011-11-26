using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GothicCheckers
{
    public static class GameUtils
    {
        public static readonly Regex HelpRegex = new Regex("(help)", RegexOptions.IgnoreCase);
        public static readonly Regex MoveRegex = new Regex("(move)", RegexOptions.IgnoreCase);
        public static readonly Regex BoardPositionRegex = new Regex("[A-H]{1}[1-8]{1}", RegexOptions.IgnoreCase);

        public static PlayerColor OtherPlayer(PlayerColor player)
        {
            switch (player)
            {
                case PlayerColor.None:
                    throw new ArgumentException();
                case PlayerColor.Black:
                    return PlayerColor.White;
                case PlayerColor.White:
                    return PlayerColor.Black;
                default:
                    throw new ArgumentException();
            }
        }

        public static void EnsureSaveDirectory()
        {
            if (!Directory.Exists("Save"))
            {
                Directory.CreateDirectory("Save");
            }
        }
    }
}
