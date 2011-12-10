using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers.AI
{
    public static class Evaluator
    {
        private static readonly int[] _whiteValuesPerPiecePerLevel;
        private static readonly int[] _blackValuesPerPiecePerLevel;

        private const int _normalPiecePrice = 100;
        private const int _kingPiecePrice = 130;

        private const int _randomRange = 5;

        static Random _rand = new Random();

        static Evaluator()
        {
            _whiteValuesPerPiecePerLevel = new int[] { 1000, 80, 60, 40, 30, 20, 10, 0 }; //level 0 = rada A8-B8-C8...
            _blackValuesPerPiecePerLevel = new int[] { 0, 10, 20, 30, 40, 60, 80, 1000 };
        }

        public static int Evaluate(GameBoard board, PlayerColor player) //zatim zcela zakladni funkce, aby pocitac netahl uplne nahodne
        {
            int blackScore = 0;
            int whiteScore = 0;

            for (int i = 0; i < GameBoard.BOARD_SIDE_SIZE; ++i)
            {
                int cnt = board.PieceCountOfPlayerAtLevel(PlayerColor.Black, i);
                blackScore += cnt * _blackValuesPerPiecePerLevel[i]; //cim blize transformaci na damu, tim lip
                cnt = board.PieceCountOfPlayerAtLevel(PlayerColor.White, i);
                whiteScore += cnt * _whiteValuesPerPiecePerLevel[i];
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
