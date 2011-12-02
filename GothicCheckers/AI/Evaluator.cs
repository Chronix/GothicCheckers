using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers.AI
{
    public static class Evaluator
    {
        private static readonly int[] _valuesPerPiecePerLevel = { 64, 32, 16, 8, 4, 2, 1, 0 }; //z pohledu bileho, level 0 = rada A8-B8-C8...
        private const int _normalPiecePrice = 100;
        private const int _kingPiecePrice = 130;

        private const int _randomRange = 10;

        static Random _rand = new Random();

        public static int Evaluate(GameBoard board, PlayerColor player) //zatim zcela zakladni funkce, aby pocitac netahl uplne nahodne
        {
            int val = 0;

            int white = board.PieceCountOfPlayerByPieceType(PlayerColor.White, PieceType.Normal) * _normalPiecePrice + board.PieceCountOfPlayerByPieceType(PlayerColor.White, PieceType.King) * _kingPiecePrice;
            int black = board.PieceCountOfPlayerByPieceType(PlayerColor.Black, PieceType.King) * _kingPiecePrice + board.PieceCountOfPlayerByPieceType(PlayerColor.Black, PieceType.King) * _kingPiecePrice;

            val += ((black - white) * 200) / (black + white);
            val += black - white;

            return (player == PlayerColor.Black ? val : -val) + _rand.Next(-_randomRange, _randomRange);
        }
    }
}
