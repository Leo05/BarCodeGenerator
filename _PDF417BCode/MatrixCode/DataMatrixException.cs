using System;
using System.Collections.Generic;
using System.Text;


namespace _PDF417BCode.Internal
{
    public class DataMatrixException : Exception
    {
        public DataMatrixException(string message)
            : base(message)
        {
        }

        public DataMatrixException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
