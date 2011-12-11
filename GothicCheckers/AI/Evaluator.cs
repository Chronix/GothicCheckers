using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers.AI
{
    public static class Evaluator
    {
        private static readonly int[] _whitePieceValuesPerLevel = { 1000, 80, 60, 40, 30, 20, 10, 0 };
        private static readonly int[] _blackPieceValuesPerLevel = { 0, 10, 20, 30, 40, 60, 80, 1000 };

        private const int _normalPiecePrice = 100;
        private const int _kingPiecePrice = 130;

        private const int _randomRange = 5;

        private static Random _rand = new Random();

        public static int Evaluate(GameBoard board, PlayerColor player)
        {
            int blackScore = 0;
            int whiteScore = 0;

            for (int i = 0; i < GameBoard.BOARD_SIDE_SIZE; ++i)
            {
                blackScore += board.PieceCountOfPlayerAtLevel(PlayerColor.Black, i) * _blackPieceValuesPerLevel[i]; //cim blize transformaci na damu, tim lip
                whiteScore += board.PieceCountOfPlayerAtLevel(PlayerColor.White, i) * _whitePieceValuesPerLevel[i];
            }

            whiteScore += board.PieceCountOfPlayerByPieceType(PlayerColor.White, PieceType.Normal) * _normalPiecePrice;
            whiteScore += board.PieceCountOfPlayerByPieceType(PlayerColor.White, PieceType.King) * _kingPiecePrice;

            blackScore += board.PieceCountOfPlayerByPieceType(PlayerColor.Black, PieceType.Normal) * _normalPiecePrice;
            blackScore += board.PieceCountOfPlayerByPieceType(PlayerColor.Black, PieceType.King) * _kingPiecePrice;

            int score = (blackScore - whiteScore) * 200;
            score += _rand.Next(-_randomRange, _randomRange);

            return player == PlayerColor.Black ? score : -score;
        }
    }
}
