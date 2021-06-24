using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeStorePayments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten"
        };

        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;

        public PaymentsController(ILogger<PaymentsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<ActionResult> Create()
        {
            string connectionString = _configuration.GetValue<string>("Payments:Settings:ServiceBusConnectionString");
            string queueName = _configuration.GetValue<string>("Payments:Settings:OrdersQueueName");

            ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            using ServiceBusMessageBatch batch = await sender.CreateMessageBatchAsync();

            ServiceBusMessage message = new ServiceBusMessage("create order");

            if (!batch.TryAddMessage(message))
                throw new Exception("unable to send message to azure service bus");

            try
            {
                await sender.SendMessagesAsync(batch);
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            return Ok();
        }
    }
}
