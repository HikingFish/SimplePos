using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Domain.Common.ResultPattern
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Forbidden,
        Unexpected,
        None
    }
}
