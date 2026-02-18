using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus;
using SFA.DAS.Payments.EarningEvents.Messages.External.Commands;
using System.Threading.Tasks;

namespace Spike.FM36Tool.Application.DASEarningsBridgeQueue
{
    public class DASEarningsBridgeQueueService : IDASEarningsBridgeQueueService
    {
        private readonly ILogger<DASEarningsBridgeQueueService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DASServiceBusSettings _settings;

        public DASEarningsBridgeQueueService(IConfiguration configuration, 
            ILogger<DASEarningsBridgeQueueService> logger, 
            IOptions<DASServiceBusSettings> options)
        {
            _configuration = configuration;
            _logger = logger;
            _settings = options.Value;
        }

        public async Task SendMessageToServiceBus(CalculateGrowthAndSkillsPayments growthAndSkillsCommand)
        {
            var endpointConfiguration = new EndpointConfiguration("ClientEndpoint");
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
            endpointConfiguration.SendOnly();
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(_settings.ConnectionString);
            var endpointName = _settings.EndpointName;
            endpointConfiguration.Conventions()
                                 .DefiningCommandsAs(type => type == typeof(CalculateGrowthAndSkillsPayments));
            transport.Routing()
                .RouteToEndpoint(typeof(CalculateGrowthAndSkillsPayments), endpointName);

            var endpointInstance = await Endpoint.Start(endpointConfiguration);
            
            await endpointInstance.Send(growthAndSkillsCommand);

            await endpointInstance.Stop();
        }
    }
}
