using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException(string message)
            : base(message) { }
    }
}
