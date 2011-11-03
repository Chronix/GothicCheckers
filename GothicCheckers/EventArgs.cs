using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public class VisualChangedEventArgs : EventArgs
    {
        public int[] ChangedIndices { get; private set; }

        public VisualChangedEventArgs(IEnumerable<int> changedIndices)
        {
            ChangedIndices = changedIndices.ToArray();
        }
    }
}
