using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.Exceptions
{
    public class ExpenseException : Exception
    {
        public ExpenseException(string message) : base(message)
        {
        }
    }
}
