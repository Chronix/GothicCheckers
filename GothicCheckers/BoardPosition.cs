using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

#pragma warning disable 0659, 0661 // xyz overrides Equals but not GetHashCode / xyz overrides == or != but not GetHashCode

namespace GothicCheckers
{  
    public struct BoardPosition : IEquatable<BoardPosition>
    {
        static string _letters = "ABCDEFGH";
        static string _numbers = "87654321";
        static string[] _whiteUpgradePositions = { "A8", "B8", "C8", "D8", "E8", "F8", "G8", "H8" };
        static string[] _blackUpgradePositions = { "A1", "B1", "C1", "D1", "E1", "F1", "G1", "H1" };

        public static readonly BoardPosition Invalid = new BoardPosition(-1, -1);

        public int X { get; set; }
        public int Y { get; set; }

        public string Representation
        {
            get { return _letters[X].ToString() + _numbers[Y].ToString(); }
        }

        public BoardPosition(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }

        public BoardPosition(int rawIndex)
            : this()
        {
            X = rawIndex % GameBoard.BOARD_SIDE_SIZE;
            Y = rawIndex / GameBoard.BOARD_SIDE_SIZE;
        }

        public BoardPosition(string rep)
            : this()
        {
            rep = rep.ToUpper();
            char c = rep[0];
            char n = rep[1];

            X = _letters.IndexOf(c);
            Y = _numbers.IndexOf(n);
        }

        public bool Equals(BoardPosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is BoardPosition) return Equals((BoardPosition)obj);
            else return false;
        }

        public static BoardPosition GetPositionBetween(BoardPosition pos1, BoardPosition pos2)
        {
            BoardPosition mid = new BoardPosition();
            mid.X = pos1.X + (pos2.X - pos1.X) / 2;
            mid.Y = pos1.Y + (pos2.Y - pos1.Y) / 2;

            if (mid == pos1 || mid == pos2) return Invalid;

            return mid;
        }

        public bool IsUpgradingPosition(PlayerColor player)
        {
            string myRep = Representation;

            if (player == PlayerColor.White)
            {
                if (_whiteUpgradePositions.Any(s => s.Equals(myRep))) return true;                
            }
            else if (player == PlayerColor.Black)
            {
                if (_blackUpgradePositions.Any(s => s.Equals(myRep))) return true;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}, Rep: {2}", X, Y, Representation);
        }

        public static bool operator ==(BoardPosition pos1, BoardPosition pos2)
        {
            if (object.ReferenceEquals(pos1, pos2)) return true;

            if ((object)pos1 == null || (object)pos2 == null) return false;

            return pos1.Equals(pos2);
        }

        public static bool operator !=(BoardPosition pos1, BoardPosition pos2)
        {
            return !(pos1 == pos2);
        }

        public static implicit operator BoardPosition(string rep)
        {
            return new BoardPosition(rep);
        }

        public static explicit operator string(BoardPosition pos)
        {
            return pos.Representation;
        }
    }
}

#pragma warning restore 0659, 0661
