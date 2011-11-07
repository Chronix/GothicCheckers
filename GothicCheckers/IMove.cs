using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public interface IMove : IEquatable<IMove>
    {
        PlayerColor Player { get; }
        BoardPosition FromField { get; }
        BoardPosition ToField { get; }

        bool IsCapture { get; set; }
        bool KingMove { get; }        
        bool Reversed { get; }
        bool IsUpgrade { get; }
        int Length { get; }

        GameField Capture { get; set; }

        IMove Reverse();
    }
}
