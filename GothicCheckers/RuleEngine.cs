using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GothicCheckers
{
    public static class RuleEngine
    {
        public const int MAX_IDLE_MOVES = 30;

        public static IEnumerable<BoardPosition> GetPossibleTargetFields(GameBoard board, BoardPosition fromFieldPosition, out bool forcedOnly)
        {
            Dictionary<BoardPosition, bool> targets = new Dictionary<BoardPosition, bool>();
            GameField fromField = board[fromFieldPosition.X, fromFieldPosition.Y];

            if (fromField.Piece == PieceType.Normal)
            {
                int direction = fromField.Occupation == PlayerColor.White ? -1 : 1;
                Tuple<BoardPosition, bool> checkResult;

                if (CheckDirectionNormal(board, fromField, -1, 0, out checkResult)) targets.Add(checkResult.Item1, checkResult.Item2); //pole primo nalevo
                if (CheckDirectionNormal(board, fromField, 1, 0, out checkResult)) targets.Add(checkResult.Item1, checkResult.Item2); //pole primo napravo

                if (CheckDirectionNormal(board, fromField, -1, direction, out checkResult)) targets.Add(checkResult.Item1, checkResult.Item2); //pole nalevo dolu/nahoru
                if (CheckDirectionNormal(board, fromField, 0, direction, out checkResult)) targets.Add(checkResult.Item1, checkResult.Item2);  //pole rovne dolu/nahoru
                if (CheckDirectionNormal(board, fromField, 1, direction, out checkResult)) targets.Add(checkResult.Item1, checkResult.Item2);  //pole napravo dolu/nahoru
            }
            else if (fromField.Piece == PieceType.King)
            {
                CheckDirectionKing(board, fromField, -1, 1, ref targets);
                CheckDirectionKing(board, fromField, 0, 1, ref targets);
                CheckDirectionKing(board, fromField, 1, 1, ref targets);
                CheckDirectionKing(board, fromField, -1, -1, ref targets);
                CheckDirectionKing(board, fromField, 0, -1, ref targets);
                CheckDirectionKing(board, fromField, 1, -1, ref targets);
            }
            
            var result = targets.Where(pair => pair.Value).Select(pair => pair.Key);

            if (result.Count() == 0)
            {
                forcedOnly = false;
                return targets.Keys;
            }

            forcedOnly = true;
            return result;
        }

        public static IEnumerable<IMove> GetAllMovesFromField(GameBoard board, BoardPosition fromFieldPosition, out bool forcedOnly)
        {
            PlayerColor player = board[fromFieldPosition].Occupation;
            List<IMove> moves = new List<IMove>();

            var firstTargets = GetPossibleTargetFields(board, fromFieldPosition, out forcedOnly);            

            if (!forcedOnly)
            {
                moves.AddRange(firstTargets.Select(pos => new SimpleMove(player, fromFieldPosition, pos)));
                return moves;
            }

            JumpTree tree = new JumpTree(fromFieldPosition, board);
            tree.AddTargets(fromFieldPosition, firstTargets);
            ConstructJumpTree(tree);

            var paths = tree.Linearize();

            foreach (var path in paths)
            {
                if (path.Count == 2)
                {
                    moves.Add(new SimpleMove(player, path[0], path[1]));
                }
                else
                {
                    CompoundMove move = CompoundMove.FromPositions(player, path.First(), path.Last(), path.GetRange(1, path.Count - 2).ToArray());
                    moves.Add(move);
                }
            }
            
            return moves;
        }

        public static IEnumerable<IMove> GetAllMovesForPlayer(GameBoard board, PlayerColor player)
        {
            IEnumerable<GameField> fields = board.GetFieldsByPlayer(player);
            List<IMove> forcedMoves = new List<IMove>();
            List<IMove> idleMoves = new List<IMove>();

            bool forcedOnly;

            foreach (GameField f in fields)
            {
                var moves = GetAllMovesFromField(board, f.Position, out forcedOnly);

                if (forcedOnly) forcedMoves.AddRange(moves);
                else idleMoves.AddRange(moves);
            }

            if (forcedMoves.Count > 0) return forcedMoves;
            else return idleMoves;
        }

        public static bool ValidateMove(GameBoard board, IMove move)
        {            
            var moves = GetAllMovesForPlayer(board, move.Player);
            return moves.Contains(move);
        }

        public static bool CheckForGameEnd(GameBoard board, out PlayerColor winner)
        {
            int whitePieces = board.GetPieceCountByOccupation(PlayerColor.White);
            int blackPieces = board.GetPieceCountByOccupation(PlayerColor.Black);

            if (whitePieces == 0)
            {
                winner = PlayerColor.Black;
                return true;
            }
            else if (blackPieces == 0)
            {
                winner = PlayerColor.White;
                return true;
            }
            else if (board.IdleMoves >= MAX_IDLE_MOVES)
            {
                if (whitePieces == blackPieces)
                {
                    winner = PlayerColor.None;
                    return true;
                }

                winner = whitePieces > blackPieces ? PlayerColor.White : PlayerColor.Black;
                return true;
            }

            winner = PlayerColor.None;
            return false;
        }

        private static bool CheckDirectionNormal(GameBoard board, GameField start, int xDir, int yDir, out Tuple<BoardPosition, bool> result)
        {
            GameField target = board[start.Position.X + xDir, start.Position.Y + yDir];

            if (target != null)
            {
                if (target.Empty)
                {
                    result = new Tuple<BoardPosition, bool>(target.Position, false); //volne pole, tah neni povinny
                    return true;
                }
                else if (target.Occupation != start.Occupation)
                {
                    GameField actualTarget = board[target.Position.X + xDir, target.Position.Y + yDir];

                    if (actualTarget != null && actualTarget.Empty)
                    {
                        result = new Tuple<BoardPosition, bool>(actualTarget.Position, true); //preskoceni protivnikova kamene, tah je (jeden z) povinny(ch tahu)
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        private static void CheckDirectionKing(GameBoard board, GameField start, int xDir, int yDir, ref Dictionary<BoardPosition, bool> result)
        {
            GameField target = board[start.Position.X + xDir, start.Position.Y + yDir];
            bool foundEnemy = false;

            while (target != null)
            {
                if (target.Empty)
                {
                    if (foundEnemy)
                    {
                        result.Add(target.Position, true);
                    }
                    else
                    {
                        result.Add(target.Position, false);
                        target = board[target.Position.X + xDir, target.Position.Y + yDir];
                    }

                    continue;
                }

                if (target.Occupation == start.Occupation) break;
                else
                {
                    if (!foundEnemy)
                    {
                        foundEnemy = true; //na dalsim poli v ceste je nepratelsky kamen
                        continue;
                    }
                    else break;
                }
            }
        }

        private static void ConstructJumpTree(JumpTree tree)
        {
            var leaves = tree.GetLeaves();
            if (ProcessLeafGroup(tree, leaves)) ConstructJumpTree(tree);
        }

        private static bool ProcessLeafGroup(JumpTree tree, IEnumerable<JumpTreeNode> leaves)
        {
            bool forced;
            bool added = false;

            foreach (var node in leaves)
            {
                var targets = GetPossibleTargetFields(node.Board, node.Position, out forced);

                if (!forced)
                {
                    continue;
                }

                tree.AddTargets(node.Position, targets);
                added = true;
            }

            return added;
        }
    }
}
