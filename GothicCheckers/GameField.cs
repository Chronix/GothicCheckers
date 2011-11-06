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
            return MemberwiseClone() as GameField;
        }

        public override string ToString()
        {
            string s = string.Empty;

            switch (Occupation)
            {
                case PlayerColor.None: s = "0"; break;
                case PlayerColor.White: s = "w"; break;
                case PlayerColor.Black: s = "b"; break;
            }

            if (Piece == PieceType.King) s = s.ToUpper();

            return string.Format("{0} - {1}", Position.Representation, s);
        }
    }
}
