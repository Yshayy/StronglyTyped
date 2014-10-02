using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseClass
{
    

    public class Optional
    {
        public static Optional<T> Of<T>(T value)
        {
            return new Optional<T>(value);
        }

        public static Optional<T> None<T>()
        {
            return new Optional<T>();
        }
    }
}
