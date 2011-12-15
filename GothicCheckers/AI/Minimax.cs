using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GothicCheckers.AI
{
    public class Minimax : AIEngine
    {
        protected override IMove GetBestMove(object oState)
        {
            IMove bestMove = null;
            AIState state = oState as AIState;
#if PERFTEST
            Stopwatch sw = Stopwatch.StartNew();
#endif
            int value = Compute(ref bestMove, state.Board, state.Player, state.Depth, state.Depth);
#if PERFTEST
            sw.Stop();
            Trace.WriteLine(string.Format("Total computation time: {0} ms", sw.ElapsedMilliseconds));
#endif
            Trace.WriteLine(string.Format("Best Move: {0} Value: {1}", bestMove.ToString(), value.ToString()));
            return bestMove;
        }

        private int Compute(ref IMove bestMove, GameBoard board, PlayerColor currentPlayer, int startDepth, int depth)
        {
            CheckForStop();

            if (depth == 0) return Evaluator.Evaluate(board, currentPlayer);

            var moves = RuleEngine.GetAllMovesForPlayer(board, currentPlayer);
            
            CheckForStop();

            if (moves.Count() == 0) return Evaluator.Evaluate(board, currentPlayer);

            int bestValue = int.MinValue;
            PlayerColor opponent = GameUtils.OtherPlayer(currentPlayer);
            GameBoard boardCopy = board.Copy();

            CheckForStop();

            foreach (var move in moves)
            {
                CheckForStop();
                boardCopy.DoMove(move, true);
                int moveValue = -Compute(ref bestMove, boardCopy, opponent, startDepth, depth - 1);
                IMove revMove = move.Reverse();
                boardCopy.DoMove(revMove, true);
                CheckForStop();

                if (depth == startDepth)
                {
                    Trace.WriteLine(string.Format("Move: {0} Value: {1}", move.ToString(), moveValue.ToString()));

                    if (bestMove == null || moveValue > bestValue)
                    {
                        bestMove = move;
                    }
                }

                bestValue = Math.Max(bestValue, moveValue);
            }

            return bestValue;
        }
    }
}
