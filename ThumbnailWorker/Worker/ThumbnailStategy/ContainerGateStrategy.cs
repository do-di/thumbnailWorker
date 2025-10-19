using ThumbnailWorker.Worker.Interface;

namespace ThumbnailWorker.Worker.ThumbnailStategy
{
    public class ContainerGateStrategy : IThumbnailStrategy
    {
        public string Type { get; } = "ContainerGate";
        public Task CreateThumbnailAsync(string request)
        {
            throw new NotImplementedException();
        }

        public string GenerateThumbnailPath(string originalPath)
        {
            throw new NotImplementedException();
        }
    }
}
