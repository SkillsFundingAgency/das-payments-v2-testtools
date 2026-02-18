using SFA.DAS.Payments.EarningEvents.Messages.External.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spike.FM36Tool.Application.DASEarningsBridgeQueue
{
    public interface IDASEarningsBridgeQueueService
    {
        Task SendMessageToServiceBus(CalculateGrowthAndSkillsPayments growthAndSkillsCommand);
    }
}
