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

        public bool IsCapture
        {
            get
            {
                return BoardPosition.GetPositionsBetween(FromField, ToField).Count > 0;
            }
        }

        public bool KingMove
        {
            get
            {
                return BoardPosition.GetPositionsBetween(FromField, ToField).Count > 1;
            }
        }

        public int Length
        {
            get { return 1; }
        }

        public SimpleMove(PlayerColor player, BoardPosition from, BoardPosition to)
        {
            Player = player;
            FromField = from;
            ToField = to;
        }

        public IMove Reverse()
        {
            SimpleMove rev = new SimpleMove(Player, ToField, FromField) { Reversed = true };
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
            return FromField == other.FromField && ToField == other.ToField && KingMove == other.KingMove;
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
