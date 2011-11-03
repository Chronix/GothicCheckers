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
        protected override IMove GetBestMove(object oState)
        {
            CTS.Token.ThrowIfCancellationRequested();
            AIState state = oState as AIState;

            int bestIndex = 0;
            int value = 0;
            int alpha = int.MinValue;

            List<IMove> moves = new List<IMove>(RuleEngine.GetAllMovesForPlayer(state.Board, state.Player));
            CTS.Token.ThrowIfCancellationRequested();

            if (moves.Count == 1) return moves[0];

            GameBoard workBoard = state.Board.Copy();

            for (int i = 0; i < moves.Count; ++i)
            {
                CTS.Token.ThrowIfCancellationRequested();
                workBoard.DoMove(moves[i]);
                PlayerColor nextPlayer = GameUtils.OtherPlayer(state.Player);
                value = -Minimax(workBoard, nextPlayer, state.Depth, int.MinValue, -alpha);
                workBoard.DoMove(moves[i].Reverse());

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

            var moves = RuleEngine.GetAllMovesForPlayer(board, player);

            foreach (IMove move in moves)
            {
                CTS.Token.ThrowIfCancellationRequested();
                board.DoMove(move);
                PlayerColor nextPlayer = GameUtils.OtherPlayer(player);
                value = -Minimax(board, nextPlayer, depth - 1, -beta, -alpha);
                board.DoMove(move.Reverse());

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
    }
}
