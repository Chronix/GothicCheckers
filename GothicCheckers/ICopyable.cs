using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public interface ICopyable<T>
    {
        T Copy();
    }
}
