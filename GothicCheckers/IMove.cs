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

        bool Forced { get; }
        bool Reversed { get; set; }
        bool UpgradingMove { get; }
        int Length { get; }

        GameField ModifiedField { get; set; }

        IMove Reverse();
    }
}
