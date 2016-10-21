using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcuseManager
{
    class InvalidValueException : Exception
    {
        public InvalidValueException()
        {

        }

        public InvalidValueException(string reason) : base(reason)
        {

        }
    }
}
