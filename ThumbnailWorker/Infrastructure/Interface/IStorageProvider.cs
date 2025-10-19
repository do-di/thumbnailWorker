using Amazon.S3.Model;

namespace ThumbnailWorker.Infrastructure.Interface
{
    public interface IStorageProvider
    {
        public Task UploadFileAsync(Stream steam, string filePath);

        public Task<GetObjectResponse> GetObjectAsync(string filePath);
    }
}
