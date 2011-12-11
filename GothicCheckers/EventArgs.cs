using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public class VisualChangedEventArgs : EventArgs
    {
        public IEnumerable<int> ChangedIndices { get; private set; }

        public VisualChangedEventArgs(IEnumerable<int> changedIndices)
        {
            ChangedIndices = changedIndices;
        }
    }

    public class PlayerEventArgs : EventArgs
    {
        public PlayerColor Player { get; private set; }

        public PlayerEventArgs(PlayerColor player)
        {
            Player = player;
        }
    }

    public class MoveDoneEventArgs : EventArgs
    {
        public IMove Move { get; private set; }
        public bool SuggestingMove { get; private set; }

        public MoveDoneEventArgs(IMove move, bool suggesting)
        {
            Move = move;
            SuggestingMove = suggesting;
        }
    }
}
