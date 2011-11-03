using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GothicCheckers.AI
{
    public interface IAIEngine : IDisposable
    {
        IMove BestMove { get; }

        event EventHandler BestMoveChosen;

        void ComputeBestMove(GameBoard board, PlayerColor player, int depth);
        void StopThinking();
    }
}
