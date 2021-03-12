using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamicDataModelWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FormController : ControllerBase
    {
        private readonly ILogger<FormController> _logger;

        public FormController(ILogger<FormController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 通用表單 Endpoint
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /form/AForm/
        ///     {
        ///        "Name": "twMVC",
        ///        "Number": 42
        ///     }
        /// 
        ///     POST /form/BForm/
        ///     {
        ///        "Title": "讓我們用一種方式來開發吧",
        ///        "Description": "軟體開發沒有銀子彈，但發射子彈總有差不多的射擊指南吧！"
        ///     }
        /// </remarks>
        /// <param name="type">表單類型</param>
        /// <param name="query">要處理的表單資料</param>
        /// <returns>對應不同表單而處理的回應資料</returns>
        [HttpPost]
        [Route("{type}")]
        public IActionResult Data([FromRoute] string type, [FromBody] dynamic query)
        {
            return type switch
            {
                "AForm" => Ok($"{(string)query.Name}#{query.Number}"),
                "BForm" => Ok($"{(string)query.Title} - {(string)query.Description}"),
                _ => Ok("Not support this form."),
            };
        }
    }
}
