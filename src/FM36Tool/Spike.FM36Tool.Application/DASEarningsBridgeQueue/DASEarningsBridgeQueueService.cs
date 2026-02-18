using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.External.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spike.FM36Tool.Application.DASEarningsBridgeQueue
{
    public class DASEarningsBridgeQueueService : IDASEarningsBridgeQueueService
    {
        private readonly ILogger<DASEarningsBridgeQueueService> _logger;
        private readonly IConfiguration _configuration;

        public DASEarningsBridgeQueueService(IConfiguration configuration, ILogger<DASEarningsBridgeQueueService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<int> SendMessageToServiceBus(IEnumerable<CalculateGrowthAndSkillsPayments> growthAndSkillsCommands)
        {
            var endpointConfiguration = new EndpointConfiguration("ClientEndpoint");
            endpointConfiguration.SendOnly();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(
                _configuration.GetConnectionString("DcServiceBusConnectionString"));
            var endpointName = _configuration["EndpointName"];
            endpointConfiguration.Conventions()
                                 .DefiningCommandsAs(type => type == typeof(CalculateGrowthAndSkillsPayments));
            transport.Routing()
                .RouteToEndpoint(typeof(CalculateGrowthAndSkillsPayments), endpointName);

            var endpointInstance = await Endpoint.Start(endpointConfiguration);
            foreach (var growthAndSkillsCommand in growthAndSkillsCommands)
            {
                await endpointInstance.Send(growthAndSkillsCommand);
            }

            await endpointInstance.Stop();
            _logger.LogInformation("Sent {Count} messages", growthAndSkillsCommands.Count());
            return growthAndSkillsCommands.Count();
        }
    }
}
