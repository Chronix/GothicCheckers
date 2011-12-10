using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace GothicCheckers
{
    public class GameBoard : ICopyable<GameBoard>
    {
        public const int BOARD_SIDE_SIZE = 8;
        public const int BOARD_PIECE_COUNT = BOARD_SIDE_SIZE * BOARD_SIDE_SIZE;
        private GameField[] _fields;

        public event EventHandler<VisualChangedEventArgs> VisualChanged;

        public GameBoard(bool init = true)
        {
            IdleMoves = 0;
            if (init) Reset();
        }

        public GameField this[int rawIndex]
        {
            get
            {
                if (rawIndex < 0 || rawIndex >= _fields.Length) return null;

                return _fields[rawIndex];
            }
        }

        public GameField this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x > BOARD_SIDE_SIZE - 1 || y > BOARD_SIDE_SIZE - 1) return null;

                int actualIndex = BOARD_SIDE_SIZE * y + x;

                return _fields[actualIndex];
            }
        }

        public GameField this[BoardPosition position]
        {
            get { return this[position.X, position.Y]; }
        }

        public GameField this[string rep]
        {
            get { return this[new BoardPosition(rep)]; }
        }

        public int IdleMoves { get; set; }

        public void Reset()
        {
            _fields = new GameField[BOARD_PIECE_COUNT];

            GameField[][] tempFields = new GameField[BOARD_SIDE_SIZE][];

            for (int i = 0; i < BOARD_SIDE_SIZE; ++i)
            {
                tempFields[i] = new GameField[BOARD_SIDE_SIZE];

                for (int j = 0; j < BOARD_SIDE_SIZE; ++j)
                {
                    tempFields[i][j] = new GameField();
                    tempFields[i][j].Position = new BoardPosition(j, i);
                }

                switch (i)
                {
                    case 0: for (int j = 0; j < BOARD_SIDE_SIZE; ++j) { tempFields[i][j].Occupation = PlayerColor.Black; tempFields[i][j].Piece = PieceType.Normal; } break;
                    case 1: goto case 0;
                    case 6: for (int j = 0; j < BOARD_SIDE_SIZE; ++j) { tempFields[i][j].Occupation = PlayerColor.White; tempFields[i][j].Piece = PieceType.Normal; } break;
                    case 7: goto case 6;
                    default: break;
                }
            }

            int index = 0;

            for (int i = 0; i < BOARD_SIDE_SIZE; ++i)
            {
                for (int j = 0; j < BOARD_SIDE_SIZE; ++j)
                {
                    _fields[index++] = tempFields[i][j];
                }
            }

            IdleMoves = 0;
        }

        public int GetPieceCountByOccupation(PlayerColor playerColor)
        {
            return _fields.Where(f => f.Occupation == playerColor).Count();
        }

        public int PieceCountOfPlayerAtLevel(PlayerColor player, int level)
        {
            int startIndex = level * BOARD_SIDE_SIZE;
            int endIndex = startIndex + BOARD_SIDE_SIZE;
            int count = 0;

            for (; startIndex < endIndex; ++startIndex)
            {
                if (this[startIndex].Occupation == player) count++;
            }

            return count;
        }

        public int PieceCountOfPlayerByPieceType(PlayerColor player, PieceType type)
        {
            return _fields.Where(f => f.Occupation == player).Where(f => f.Piece == type).Count();
        }

        public PlayerColor PlayerAt(BoardPosition pos)
        {
            return this[pos].Occupation;
        }

        public PlayerColor PlayerAt(int index)
        {
            return this[index].Occupation;
        }

        public IEnumerable<GameField> GetFieldsByPlayer(PlayerColor playerColor)
        {
            List<GameField> fields = new List<GameField>();

            foreach (GameField f in _fields)
            {
                if (f.Occupation == playerColor) fields.Add(f);
            }

            return fields;
        }

        public GameBoard Copy()
        {
            GameBoard newBoard = new GameBoard(false);
            newBoard.IdleMoves = IdleMoves;
            newBoard._fields = new GameField[_fields.Length];

            for (int i = 0; i < _fields.Length; ++i)
            {
                newBoard._fields[i] = _fields[i].Copy();
            }

            return newBoard;
        }

        public string AsAsciiPicture()
        {           
            StringBuilder sb = new StringBuilder();
            string numbers = "12345678";
            

            using (StringWriter sw = new StringWriter(sb))
            {
                sw.WriteLine(" +-+-+-+-+-+-+-+-+");
                for (int i = BOARD_SIDE_SIZE - 1; i >= 0; --i)
                {
                    sw.Write(numbers[i]);
                    sw.Write("|");
                    for (int j = 0; j < BOARD_SIDE_SIZE; ++j)
                    {
                        var field = _fields[BOARD_SIDE_SIZE * i + j];

                        switch (field.Occupation)
                        {
                            case PlayerColor.Black: sw.Write(field.Piece == PieceType.King ? "B" : "b"); break;
                            case PlayerColor.White: sw.Write(field.Piece == PieceType.King ? "W" : "w"); break;
                            default: sw.Write(" "); break;
                        }

                        sw.Write("|");
                    }
                    sw.WriteLine();
                    sw.WriteLine(" +-+-+-+-+-+-+-+-+");
                }

                sw.WriteLine("  A B C D E F G H");
            }

            return sb.ToString();
        }

        public void DoMove(IMove move, bool suppressRedraw = false)
        {
            if (!move.CaptureSet && !move.Reversed) SetCapture(move);

            IEnumerable<int> changedIndices = null;

            if (move is SimpleMove) changedIndices = DoSimpleMove((SimpleMove)move);
            else changedIndices = DoCompoundMove((CompoundMove)move);

            if (!move.IsCapture)
            {
                if (move.Reversed) IdleMoves--;
                else IdleMoves++;
            }

            if (move.IsUpgrade)
            {
                if (move.Reversed) this[move.FromField].Piece = PieceType.Normal;
                else this[move.ToField].Piece = PieceType.King;
            }

            if (!suppressRedraw) OnVisualChanged(changedIndices);
        }

        public static int TrueIndexOf(BoardPosition bp)
        {
            return BOARD_SIDE_SIZE * bp.Y + bp.X;
        }

        public static IEnumerable<int> TrueIndicesOf(IEnumerable<BoardPosition> bps)
        {
            return bps.Select(bp => TrueIndexOf(bp));
        }

        public void SetCapture(IMove move)
        {
            if (move is SimpleMove) SetCaptureSimple((SimpleMove)move);
            else SetCaptureCompound((CompoundMove)move);
        }

        private void SetCaptureSimple(SimpleMove move)
        {
            IList<BoardPosition> mid = BoardPosition.GetPositionsBetween(move.FromField, move.ToField);
            move.Capture = mid.Select(bp => this[bp]).Where(f => f.Occupation == GameUtils.OtherPlayer(move.Player)).SingleOrDefault();

            if (move.Capture != null)
            {
                move.Capture = move.Capture.Copy();
                move.IsCapture = true;
            }

            move.CaptureSet = true;
        }

        private void SetCaptureCompound(CompoundMove move)
        {
            foreach (var sm in move.Moves) SetCaptureSimple(sm);
        }

        private IEnumerable<int> DoSimpleMove(SimpleMove move)
        {
            PieceType pType = this[move.FromField].Piece;

            this[move.FromField].Occupation = PlayerColor.None;
            this[move.FromField].Piece = PieceType.None;

            List<int> changedIndices = new List<int> { TrueIndexOf(move.FromField), TrueIndexOf(move.ToField) };

            if (move.IsCapture)
            {
                if (move.Reversed)
                {
                    IList<BoardPosition> mid = BoardPosition.GetPositionsBetween(move.FromField, move.ToField);
                    this[move.Capture.Position].Occupation = move.Capture.Occupation;
                    this[move.Capture.Position].Piece = move.Capture.Piece;
                    changedIndices.AddRange(TrueIndicesOf(mid));
                }
                else
                {
                    IList<BoardPosition> mid = BoardPosition.GetPositionsBetween(move.FromField, move.ToField);
                    this[move.Capture.Position].Occupation = PlayerColor.None;
                    this[move.Capture.Position].Piece = PieceType.None;
                    changedIndices.AddRange(TrueIndicesOf(mid));
                    IdleMoves = 0;
                }
            }

            this[move.ToField].Occupation = move.Player;
            this[move.ToField].Piece = pType;

            return changedIndices;
        }

        private IEnumerable<int> DoCompoundMove(CompoundMove move)
        {
            List<int> changedIndices = new List<int>();

            foreach (var simpleMove in move.Moves)
            {
                changedIndices.AddRange(DoSimpleMove(simpleMove));
            }

            return changedIndices.Distinct();
        }

        private void OnVisualChanged(IEnumerable<int> indices)
        {
            if (VisualChanged != null) VisualChanged(this, new VisualChangedEventArgs(indices));
        }
    }
}
