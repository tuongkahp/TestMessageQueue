using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TestMessageQueue.Api;
using TestMessageQueue.Models;
using TestMessageQueue.Models.Options;
using TestMessageQueue.Services.Interface;

namespace TestingMessageQueue.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestRabbitMQController : ControllerBase
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly ILogger<TestRabbitMQController> _logger;

        public TestRabbitMQController(IServiceWrapper serviceWrapper, ILogger<TestRabbitMQController> logger)
        {
            _serviceWrapper = serviceWrapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to test message queue system");
        }

        [HttpPost("test-concurrent")]
        public async Task<IActionResult> TestConcurrent(int n, int delay, int total)
        {
            if (n < 1 || delay < 0 || total < 0)
            {
                return BadRequest("INVALID_DATA");
            }

            delay *= 1000;

            for (int i = 0; i < total; i++)
            {
                await SendMessageAtTheSameTime(n);
                Thread.Sleep(delay);
            }

            return Ok("DONE");
        }

        private async Task SendMessageAtTheSameTime(int totalMessage)
        {
            await _serviceWrapper.RabbitMQService.SendMessage(totalMessage);
        }
    }
}
