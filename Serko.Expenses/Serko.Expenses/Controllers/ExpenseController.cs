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
        ICalculator _calc;

        public ExpenseController(IEngine engine, ICalculator calc)
        {
            _engine = engine;
            _calc = calc;
        }

        // GET api/expense
        [HttpGet]
        public ActionResult<IDictionary<string, string>> Get(string value)
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

        // POST api/expense
        [HttpPost]
        public ActionResult<IDictionary<string, string>> Post([FromBody] string value)
        {
            return Ok(_engine.ParseAndCalculateGst(value));
        }
    }
}