using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers.AI
{
    public class AIState
    {
        public GameBoard Board { get; set; }
        public PlayerColor Player { get; set; }
        public int Depth { get; set; }

        public AIState(GameBoard board, PlayerColor player, int depth)
        {
            Board = board;
            Player = player;
            Depth = depth;
        }
    }
}
