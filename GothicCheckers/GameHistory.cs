using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public class GameHistory : ObservableCollection<IMove>
    {
        public const char RIGHT_ARROW_SYMBOL = (char)0x2192;

        public void RemoveLast()
        {
            RemoveAt(Count - 1);
        }
    }
}
