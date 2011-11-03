using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers.AI
{
    public static class Evaluator
    {
        static Random _rand = new Random();

        public static int Evaluate(GameBoard board, PlayerColor player)
        {
            return _rand.Next();
        }
    }
}
