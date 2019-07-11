using System;
using System.Collections.Generic;
using System.Text;

namespace Serko.Expenses.Core.Exceptions
{
    public class MissingTotalValueException : ExpenseException
    {
        public MissingTotalValueException(string message) : base(message)
        {
        }
    }
}
