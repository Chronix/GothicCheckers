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
        
        public bool Reversed { get; set; }

        public GameField ModifiedField { get; set; }

        public bool UpgradingMove
        {
            get 
            {
                if (Reversed) return FromField.IsUpgradingPosition(Player);
                else return ToField.IsUpgradingPosition(Player); 
            }
        }

        public bool Forced
        {
            get { return BoardPosition.GetPositionBetween(FromField, ToField) != BoardPosition.Invalid; }
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
            return new SimpleMove(Player, ToField, FromField) { Reversed = true };
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} {2} {3}", Player.ToString()[0], FromField.Representation, GameHistory.RIGHT_ARROW_SYMBOL, ToField.Representation);
        }

        public SimpleMove Copy()
        {
            return MemberwiseClone() as SimpleMove;
        }

        public bool Equals(SimpleMove other)
        {
            return FromField == other.FromField && ToField == other.ToField;
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
