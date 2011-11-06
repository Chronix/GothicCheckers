using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GothicCheckers.AI
{
    public class MinimaxAB : AIEngine
    {
        private const int INFINITY = 10000000;
        private const int LOT = 8000 * Evaluator.N;

        protected override IMove GetBestMove(object oState)
        {
            CTS.Token.ThrowIfCancellationRequested();
            AIState state = oState as AIState;

            int bestIndex = 0;
            int value = 0;
            int alpha = -INFINITY;

            List<IMove> moves = new List<IMove>(RuleEngine.GetAllMovesForPlayer(state.Board, state.Player));
            CTS.Token.ThrowIfCancellationRequested();

            if (moves.Count == 1) return moves[0];

            GameBoard workBoard = state.Board.Copy();

            for (int i = 0; i < moves.Count; ++i)
            {
                CTS.Token.ThrowIfCancellationRequested();
                workBoard.DoMove(moves[i], true);
                PlayerColor nextPlayer = GameUtils.OtherPlayer(state.Player);
                value = -Minimax(workBoard, nextPlayer, state.Depth, alpha, Winning(-alpha));
                value = Losing(value);
                workBoard.DoMove(moves[i].Reverse(), true);

                if (value > alpha)
                {
                    alpha = value;
                    bestIndex = i;
                }
            }

            return moves[bestIndex];
        }

        private int Minimax(GameBoard board, PlayerColor player, int depth, int alpha, int beta)
        {
            CTS.Token.ThrowIfCancellationRequested();

            int value = 0;

            PlayerColor winner;
            if (RuleEngine.CheckForGameEnd(board, out winner))
            {
                return int.MinValue; // vyhral ten, co tahl naposledy, ne aktualni hrac
            }

            if (depth == 0)
            {
                return Evaluator.Evaluate(board, player);
            }

            var moves = RuleEngine.GetAllMovesForPlayer(board, player);

            foreach (IMove move in moves)
            {
                CTS.Token.ThrowIfCancellationRequested();
                board.DoMove(move, true);
                PlayerColor nextPlayer = GameUtils.OtherPlayer(player);
                value = -Minimax(board, nextPlayer, depth - 1, -beta, -alpha);
                value = Losing(value);
                board.DoMove(move.Reverse(), true);

                if (value > alpha)
                {
                    alpha = value;

                    if (value >= beta)
                    {
                        return beta;
                    }
                }
            }

            return alpha;
        }

        private int Winning(int val)
        {
            if (val > LOT) return val + Evaluator.N;
            if (val < -LOT) return val - Evaluator.N;
            return val;
        }

        private int Losing(int val)
        {
            if (val > LOT) return val - Evaluator.N;
            if (val < -LOT) return val + Evaluator.N;
            return val;
        }
    }
}
