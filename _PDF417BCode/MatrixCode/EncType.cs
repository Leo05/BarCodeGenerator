using System;
using System.Collections.Generic;
using System.Text;


namespace _PDF417BCode.Internal
{
    internal enum EncType
    {
        E_ASCII = 0,
        E_C40,
        E_TEXT,
        E_X12,
        E_EDIFACT,
        E_BINARY,
        E_MAX
    };
}
