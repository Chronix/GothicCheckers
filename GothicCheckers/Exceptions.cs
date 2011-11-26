using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GothicCheckers
{
    [Serializable]
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException() : base() { }

        public InvalidMoveException(string message)
            : base(message) { }

        public InvalidMoveException(string message, Exception innerException)
            : base(message, innerException) { }

        protected InvalidMoveException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
