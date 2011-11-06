using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers.AI
{
    public static class Evaluator
    {
        private static readonly int[] _valuesPerPiecePerLevel = { 64, 32, 16, 8, 4, 2, 1, 0 }; //z pohledu bileho, level 0 = rada A8-B8-C8...
        private const int _normalPiecePrice = 50;
        private const int _kingPiecePrice = 500;

        public const int N = 10;

        static Random _rand = new Random();

        public static int Evaluate(GameBoard board, PlayerColor player) //zatim zcela zakladni funkce, aby pocitac netahl uplne nahodne
        {
            int val = 0;

            for (int i = 0; i < GameBoard.BOARD_SIDE_SIZE; ++i)
            {
                val += board.PieceCountOfPlayerAtLevel(PlayerColor.White, i) * _valuesPerPiecePerLevel[i]; //cim blize transformaci na damu, tim lip
            }

            val += board.PieceCountOfPlayerByPieceType(PlayerColor.White, PieceType.Normal) * _normalPiecePrice;
            val += board.PieceCountOfPlayerByPieceType(PlayerColor.White, PieceType.King) * _kingPiecePrice;

            val *= N * _rand.Next(N);

            if (player == PlayerColor.White) return val;
            else return -val;
        }
    }
}
