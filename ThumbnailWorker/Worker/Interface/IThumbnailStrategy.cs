namespace ThumbnailWorker.Worker.Interface
{
    public interface IThumbnailStrategy
    {
        string Type { get; }
        Task CreateThumbnailAsync(string request);
        string GenerateThumbnailPath(string originalPath);
    }
}
