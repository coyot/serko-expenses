using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.Exceptions
{
    class InvalidInputException : ExpenseException
    {
        public InvalidInputException(string message) : base(message)
        {
        }
    }
}
