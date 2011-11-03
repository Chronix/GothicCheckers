using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public class GameField : ICopyable<GameField>
    {
        public PlayerColor Occupation { get; set; }
        public PieceType Piece { get; set; }

        public BoardPosition Position { get; set; }

        public bool Empty
        {
            get { return Occupation == PlayerColor.None; }
        }

        public GameField Copy()
        {
            return new GameField { Occupation = this.Occupation, Piece = this.Piece, Position = this.Position };
        }
    }
}
