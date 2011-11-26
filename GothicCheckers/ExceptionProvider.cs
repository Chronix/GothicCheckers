using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GothicCheckers
{
    public static class ExceptionProvider
    {
        public static void ThrowIfNull(object o, string message = null)
        {
            if (o == null)
            {
                if (string.IsNullOrEmpty(message)) throw new ArgumentNullException();
                else throw new ArgumentNullException(message);
            }
        }

        public static void ThrowInvalidMoveIf(bool condition, string message)
        {
            if (condition) throw new InvalidMoveException(message);
        }

        public static void ThrowIf<T>(bool condition, string message = null) where T : Exception
        {
            if (condition)
            {
                ConstructorInfo ctor = typeof(T).GetConstructor(new Type[] { typeof(string) });
                throw ctor.Invoke(new object[] { message }) as T;
            }
        }
    }
}
