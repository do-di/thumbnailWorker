using ThumbnailWorker.Infrastructure;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text.Json;
using ThumbnailWorker.Model;
using ThumbnailWorker.Worker.Interface;
using ThumbnailWorker.Infrastructure.Interface;

namespace ThumbnailWorker.Worker.ThumbnailStategy
{
    public class SealThumbnailStrategy : IThumbnailStrategy
    {
        private readonly IStorageProvider _storageProvider;
        private readonly ApplicationDbContext _context;

        public SealThumbnailStrategy(IStorageProvider storageProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _storageProvider = storageProvider;
            var scope = serviceScopeFactory.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        public string Type { get; } = "SealThumbnail";
        public async Task CreateThumbnailAsync(string request)
        {
            // convert json string to object
            var thumbnailRequest = JsonSerializer.Deserialize<CreateThumbnailRequest>(request);
            ArgumentNullException.ThrowIfNull(thumbnailRequest);

            var image = _context.Images.FirstOrDefault(x => x.Id == thumbnailRequest.ImageId);
            if(image == null)
            {
                // not do any thing
                return;
            }

            // download image file from minio
            var response = await _storageProvider.GetObjectAsync(image.ImagePath).ConfigureAwait(false);
            using var inputStream = response.ResponseStream;

            // resize image
            using var imageOrigin = await Image.LoadAsync(inputStream);
            imageOrigin.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(450, 300),
                Mode = ResizeMode.Max
            }));
            using var outputStream = new MemoryStream();
            await imageOrigin.SaveAsJpegAsync(outputStream);
            outputStream.Position = 0;

            // upload thumbnail to minio
            var thumbnailPath = GenerateThumbnailPath(image.ImagePath);
            await _storageProvider.UploadFileAsync(outputStream, image.ImagePath).ConfigureAwait(false);

            // upload thumbnail info to database
            image.ThumbnailPath = thumbnailPath;
            _context.SaveChanges();
        }

        public string GenerateThumbnailPath(string imagePath)
        {
            return $"thumbnail/{imagePath}";
        }
    }
}
