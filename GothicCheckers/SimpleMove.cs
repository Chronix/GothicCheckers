using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public sealed class SimpleMove : IMove, ICopyable<SimpleMove>, IEquatable<SimpleMove>
    {
        public PlayerColor Player { get; set; }
        public BoardPosition FromField { get; set; }
        public BoardPosition ToField { get; set; }
        
        public bool Reversed { get; private set; }

        public GameField Capture { get; set; }

        public bool IsUpgrade
        {
            get 
            {
                if (Reversed) return FromField.IsUpgradingPosition(Player);
                else return ToField.IsUpgradingPosition(Player); 
            }
        }

        public bool IsCapture { get; set; }

        public bool KingMove { get; private set; }

        public int Length
        {
            get { return 1; }
        }

        public SimpleMove(PlayerColor player, BoardPosition from, BoardPosition to, bool kingMove, bool isCapture)
        {
            Player = player;
            FromField = from;
            ToField = to;
            KingMove = kingMove;
            IsCapture = isCapture;
        }

        public IMove Reverse()
        {
            SimpleMove rev = new SimpleMove(Player, ToField, FromField, KingMove, IsCapture) { Reversed = true };
            rev.Capture = Capture != null ? Capture.Copy() : null;
            return rev;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} {2} {3}{4}{5}", Player.ToString()[0], FromField.Representation, GameHistory.RIGHT_ARROW_SYMBOL, ToField.Representation, IsCapture ? " *" : "", IsUpgrade ? " !" : "");
        }

        public SimpleMove Copy()
        {
            SimpleMove move = MemberwiseClone() as SimpleMove;
            move.Capture = Capture != null ? Capture.Copy() : null;
            return move;
        }

        public bool Equals(SimpleMove other)
        {
            return FromField == other.FromField && ToField == other.ToField && KingMove == other.KingMove && IsCapture == other.IsCapture;
        }

        public bool Equals(IMove other)
        {
            if (other is SimpleMove)
            {
                return Equals(other as SimpleMove);
            }

            return false;
        }
    }
}
