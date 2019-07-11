using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serko.Expenses.Core;
using Serko.Expenses.Core.Calculators;
using Serko.Expenses.Core.Exceptions;

namespace Serko.Expenses.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        IEngine _engine;

        public ExpenseController(IEngine engine, ICalculator calc)
        {
            _engine = engine;
        }

        // POST api/expense
        [HttpPost]
        public ActionResult<IDictionary<string, string>> Post([FromBody] string value)
        {
            try
            {
                return Ok(_engine.ParseAndCalculateGst(value));
            }
            catch (ExpenseException e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}