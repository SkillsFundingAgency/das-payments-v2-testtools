namespace Spike.FM36Tool.Application.DASEarningsBridgeQueue
{
    public class DASServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string EndpointName { get; set; }
        public string FailedMessagesQueue { get; set; }
    }
}
