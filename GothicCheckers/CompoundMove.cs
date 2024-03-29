﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    /// <summary>
    /// Reprezentuje složený tah, neboli vícenásobný skok.
    /// </summary>
    public sealed class CompoundMove : IMove, ICopyable<CompoundMove>, IEquatable<CompoundMove>
    {
        /// <summary>
        /// Kolekce obsahující jednotlivé "podtahy".
        /// </summary>
        public Collection<SimpleMove> Moves { get; private set; }

        /// <summary>
        /// Výchozí pozice tahu.
        /// </summary>
        public BoardPosition FromField
        {
            get { return Moves[0].FromField; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Konečná pozice tahu.
        /// </summary>
        public BoardPosition ToField
        {
            get { return Moves.Last().ToField; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Hráč, kterému tah patří.
        /// </summary>
        public PlayerColor Player
        {
            get { return Moves[0].Player; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Vrací, zda při tomto tahu došlo k přeskočení nepřátelského kamene. Pro třídu CompoundMove vrací vždy true.
        /// </summary>
        public bool IsCapture
        {
            get { return true; }
            set { return; }
        }

        /// <summary>
        /// Vrací, zda jde o tah dámou
        /// </summary>
        public bool KingMove
        {
            get { return Moves[0].KingMove; }
        }

        /// <summary>
        /// Vrací, zda jde o zpětný tah (při listování historií hry)
        /// </summary>
        public bool Reversed { get; private set; }

        GameField IMove.Capture
        {
            get { return null; }
            set { return; }
        }

        /// <summary>
        /// Vrací, zda při tahu došlo k upgrade kamene na dámu.
        /// </summary>
        public bool IsUpgrade
        {
            get { return Moves.Last().IsUpgrade; }
        }

        public bool CaptureSet
        {
            get
            {
                return Moves.All(sm => sm.CaptureSet);
            }
        }

        /// <summary>
        /// Vrací délku tahu, tedy počet podtahů.
        /// </summary>
        public int Length
        {
            get { return Moves.Count; }
        }

        public CompoundMove()
        {
            Moves = new Collection<SimpleMove>();
        }

        public CompoundMove(PlayerColor player, SimpleMove firstMove)
            : this()
        {
            Player = player;
            AddMove(firstMove);
        }

        /// <summary>
        /// Přidá podtah do složeného tahu.
        /// </summary>
        /// <param name="move"></param>
        public void AddMove(SimpleMove move)
        {
            Moves.Add(move);
        }

        /// <summary>
        /// Vytvoří zpětný tah
        /// </summary>
        /// <returns></returns>
        public IMove Reverse()
        {
            CompoundMove revMove = new CompoundMove();
            revMove.Moves.AddRange(Moves.Select(m => (SimpleMove)m.Reverse()).Reverse());
            revMove.Reversed = true;
            return revMove;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}: ", Player.ToString()[0]);

            foreach (var move in Moves)
            {
                sb.AppendFormat("{0} {1} ", move.FromField.Representation, GameHistory.RIGHT_ARROW_SYMBOL);
            }

            sb.Append(Moves.Last().ToField.Representation);
            sb.Append(" *");

            if (Moves.Last().IsUpgrade) sb.Append(" !");

            return sb.ToString();
        }

        public string GetMidFieldsSaveString()
        {
            string s = string.Empty;

            for (int i = 1; i < Moves.Count; ++i)
            {
                s += Moves[i].FromField.Representation;
            }

            return s;
        }

        public static CompoundMove FromSaveData(PlayerColor player, string from, string to, string through, bool king)
        {
            BoardPosition[] positions = new BoardPosition[through.Length / 2];

            for (int i = 0; i < through.Length / 2; ++i)
            {
                positions[i] = new BoardPosition(through.Substring(i * 2, 2));
            }

            return CompoundMove.FromPositions(player, new BoardPosition(from), new BoardPosition(to), king, positions);
        }

        public static CompoundMove FromPositions(PlayerColor player, BoardPosition from, BoardPosition to, bool king, params BoardPosition[] through)
        {
            CompoundMove cMove = new CompoundMove();

            SimpleMove[] moves = new SimpleMove[through.Length + 1];
            moves[0] = new SimpleMove(player, from, through[0], king, true);

            for (int i = 1; i < through.Length; ++i)
            {
                moves[i] = new SimpleMove(player, through[i - 1], through[i], king, true);
            }

            moves[moves.Length - 1] = new SimpleMove(player, through.Last(), to, king, true);

            var nonUpgradingMoves = moves.TakeWhile(sm => !sm.IsUpgrade);
            cMove.Moves.AddRange(nonUpgradingMoves);

            if (moves.Length > cMove.Moves.Count)
            {
                cMove.Moves.Add(moves[cMove.Moves.Count]); //todo: ujasnit pravidla ohledne pokracovani skoku pri zmene pesak -> dama
            }

            return cMove;
        }

        public static CompoundMove FromPositions(PlayerColor player, string from, string to, bool king, params string[] through)
        {
            BoardPosition[] pos = new BoardPosition[through.Length];

            for (int i = 0; i < pos.Length; ++i)
            {
                pos[i] = new BoardPosition(through[i]);
            }

            return FromPositions(player, from, to, king, pos);
        }

        public CompoundMove Copy()
        {
            CompoundMove copy = new CompoundMove { Player = this.Player, Reversed = this.Reversed };

            foreach (var move in this.Moves)
            {
                copy.Moves.Add(move.Copy());
            }

            return copy;
        }

        public bool Equals(CompoundMove other)
        {
            if (Length != other.Length) return false;

            for (int i = 0; i < Length; ++i)
            {
                if (!Moves[i].Equals(other.Moves[i])) return false;
            }

            return true;
        }

        public bool Equals(IMove other)
        {
            if (other is CompoundMove)
            {
                return Equals(other as CompoundMove);
            }

            return false;
        }
    }
}
