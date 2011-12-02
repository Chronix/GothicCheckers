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
        private int _bestValue;
        private IMove _bestMove;
        private readonly object _lock = new object();

        protected override IMove GetBestMove(object oState)
        {
            _bestMove = null;
            AIState state = oState as AIState;
            int value = Compute(state.Board, state.Player, state.Depth, state.Depth);
            Debug.WriteLine("Best Move: {0} Value: {1}", _bestMove.ToString(), value.ToString());
            return _bestMove;
        }

        private int Compute(GameBoard board, PlayerColor currentPlayer, int startDepth, int depth)
        {
            CheckForStop();

            if (depth == 0) return Evaluator.Evaluate(board, currentPlayer);

            var moves = RuleEngine.GetAllMovesForPlayer(board, currentPlayer);

            if (moves.Count() == 0) return Evaluator.Evaluate(board, currentPlayer);

            _bestValue = int.MinValue;
            PlayerColor opponent = GameUtils.OtherPlayer(currentPlayer);
            GameBoard boardCopy = board.Copy();

            foreach (var move in moves)
            {
                boardCopy.DoMove(move, true);
                int moveValue = -Compute(boardCopy, opponent, startDepth, depth - 1);
                IMove revMove = move.Reverse();
                boardCopy.DoMove(revMove, true);

                if (depth == startDepth)
                {
                    Debug.WriteLine("Move: {0} Value: {1}", move.ToString(), moveValue.ToString());

                    if (_bestMove == null || moveValue > _bestValue)
                    {
                        _bestMove = move;
                    }
                }

                _bestValue = Math.Max(_bestValue, moveValue);
            }

            return _bestValue;
        }
    }
}
