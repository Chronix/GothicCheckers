using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public class GameHistoryItem
    {
        public int ID { get; set; }
        public IMove Move { get; private set; }

        public GameHistoryItem(IMove move)
        {
            Move = move;
        }

        public override string ToString()
        {
            return Move.ToString();
        }

        public override bool Equals(object obj)
        {
            GameHistoryItem item = obj as GameHistoryItem;

            if (item == null) return false;

            return Move.Equals(item.Move) && ID == item.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class GameHistory : ObservableCollection<GameHistoryItem>
    {
        public const char RIGHT_ARROW_SYMBOL = (char)0x2192;
        private static int _nextID = 0;

        public GameHistory()
        {
            EnsureFirst();            
        }

        public void RemoveLast()
        {
            if (Count == 1) return;
            else RemoveAt(Count - 1);
        }

        public void Refresh()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            EnsureFirst();
        }

        protected override void RemoveItem(int index)
        {
            if (index == 0) return;
            else base.RemoveItem(index);
        }

        protected override void InsertItem(int index, GameHistoryItem item)
        {
            if (index == 0) return;
            else
            {
                item.ID = _nextID++;
                base.InsertItem(index, item);
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (oldIndex == 0 || newIndex == 0) return;
            else base.MoveItem(oldIndex, newIndex);
        }

        protected override void SetItem(int index, GameHistoryItem item)
        {
            if (index == 0) return;
            else
            {
                item.ID = _nextID++;
                base.SetItem(index, item);
            }
        }

        private void EnsureFirst()
        {
            if (Count == 0)
            {
                this.Items.Add(new GameHistoryItem(InitialGameState.Instance) { ID = _nextID++ });
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.Items[0], 0));
            }
        }

        private class InitialGameState : IMove
        {
            #region IMOVE
            PlayerColor IMove.Player
            {
                get { throw new NotImplementedException(); }
            }

            BoardPosition IMove.FromField
            {
                get { throw new NotImplementedException(); }
            }

            BoardPosition IMove.ToField
            {
                get { throw new NotImplementedException(); }
            }

            bool IMove.IsCapture
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            bool IMove.Reversed
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            bool IMove.IsUpgrade
            {
                get { throw new NotImplementedException(); }
            }

            int IMove.Length
            {
                get { throw new NotImplementedException(); }
            }

            GameField IMove.Capture
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            IMove IMove.Reverse()
            {
                throw new NotImplementedException();
            }

            bool IEquatable<IMove>.Equals(IMove other)
            {
                return false;
            }

            bool IMove.KingMove
            {
                get { throw new NotImplementedException(); }
            }

            bool IMove.CaptureSet
            {
                get { throw new NotImplementedException(); }
            }
            #endregion

            public static readonly InitialGameState Instance = new InitialGameState();

            private InitialGameState() { }

            public override string ToString()
            {
                return GUI.Localization.MainWindowStrings.GameHistory_InitialState;
            }
        }
    }
}
