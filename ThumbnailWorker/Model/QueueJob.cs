namespace ThumbnailWorker.Model
{
    public class QueueJob
    {
        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}
