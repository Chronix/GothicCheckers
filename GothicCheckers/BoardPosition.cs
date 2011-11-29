using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 0659, 0661 // xyz overrides Equals but not GetHashCode / xyz overrides == or != but not GetHashCode

namespace GothicCheckers
{  
    /// <summary>
    /// Reprezentuje pozici na desce
    /// </summary>
    public struct BoardPosition : IEquatable<BoardPosition>
    {
        static string _letters = "ABCDEFGH";
        static string _numbers = "87654321";
        static string[] _whiteUpgradePositions = { "A8", "B8", "C8", "D8", "E8", "F8", "G8", "H8" };
        static string[] _blackUpgradePositions = { "A1", "B1", "C1", "D1", "E1", "F1", "G1", "H1" };

        /// <summary>
        /// Reprezentuje neplatnou pozici
        /// </summary>
        public static readonly BoardPosition Invalid = new BoardPosition(-1, -1);

        /// <summary>
        /// Souřadnice X této pozice
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Souřadnice Y této pozice
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Vrací "slovní" reprezentaci pozice
        /// </summary>
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

        /// <summary>
        /// Vrací pozici mezi dvěma pozicemi
        /// </summary>
        /// <param name="pos1">První pozice</param>
        /// <param name="pos2">Druhá pozice</param>
        /// <returns>Mezipozici, nebo <see cref="Invalid"/>BoardPosition.Invalid, pokud mezipozice neexistuje</returns>
        public static BoardPosition GetPositionBetween(BoardPosition pos1, BoardPosition pos2)
        {
            BoardPosition mid = new BoardPosition();
            mid.X = pos1.X + (pos2.X - pos1.X) / 2;
            mid.Y = pos1.Y + (pos2.Y - pos1.Y) / 2;

            if (mid == pos1 || mid == pos2) return Invalid;

            return mid;
        }

        /// <summary>
        /// Vrací seznam pozic mezi dvěma pozicemi
        /// </summary>
        /// <param name="pos1">První pozice</param>
        /// <param name="pos2">Druhá pozice</param>
        /// <returns>Mezipozici, nebo <see cref="Invalid"/>BoardPosition.Invalid, pokud mezipozice neexistuje</returns>
        public static IList<BoardPosition> GetPositionsBetween(BoardPosition pos1, BoardPosition pos2)
        {
            List<BoardPosition> positions = new List<BoardPosition>();
            int newX = pos1.X;
            int newY = pos1.Y;

            if (pos2.X > pos1.X)
            {
                if (pos2.Y > pos1.Y)
                {
                    while (newX != pos2.X - 1 && newY != pos2.Y - 1)
                    {
                        ++newX;
                        ++newY;
                        positions.Add(new BoardPosition(newX, newY));
                    }
                }
                else if (pos2.Y == pos1.Y)
                {
                    while (newX != pos2.X - 1)
                    {
                        ++newX;
                        positions.Add(new BoardPosition(newX, newY));
                    }
                }
                else
                {
                    while (newX != pos2.X - 1 && newY != pos2.Y + 1)
                    {
                        ++newX;
                        --newY;
                        positions.Add(new BoardPosition(newX, newY));
                    }
                }
            }
            else if (pos2.X == pos1.X)
            {
                if (pos2.Y > pos1.Y)
                {
                    while (newY != pos2.Y - 1)
                    {
                        ++newY;
                        positions.Add(new BoardPosition(newX, newY));
                    }
                }
                else if (pos2.Y < pos1.Y)
                {
                    while (newY != pos2.Y + 1)
                    {
                        --newY;
                        positions.Add(new BoardPosition(newX, newY));
                    }
                }
            }
            else
            {
                if (pos2.Y > pos1.Y)
                {
                    while (newX != pos2.X + 1 && newY != pos2.Y - 1)
                    {
                        --newX;
                        ++newY;
                        positions.Add(new BoardPosition(newX, newY));
                    }
                }
                else if (pos2.Y == pos1.Y)
                {
                    while (newX != pos2.X + 1)
                    {
                        --newX;
                        positions.Add(new BoardPosition(newX, newY));
                    }
                }
                else
                {
                    while (newX != pos2.X + 1 && newY != pos2.Y + 1)
                    {
                        --newX;
                        --newY;
                        positions.Add(new BoardPosition(newX, newY));
                    }
                }
            }

            return positions;
        }

        /// <summary>
        /// Vrací, zda je daná pozice pozicí, ve které dojde k upgradu obyčejného kamene na dámu
        /// </summary>
        /// <param name="player">Barva hráče, pro kterého je pozice posuzována</param>
        /// <returns></returns>
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
    }
}

#pragma warning restore 0659, 0661
