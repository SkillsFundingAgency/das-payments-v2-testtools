
namespace SFA.DAS.Payments.MessageResubmission.Function.Models
{
    public class ResubmitCompletionPaymentMessagesRequest
    {
        public string BlobStorageContainerName { get; set; }
        public string FileName { get; set; }
    }
}
