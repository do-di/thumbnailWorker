using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.Util;
using Microsoft.Extensions.Options;
using System;
using ThumbnailWorker.Config;
using ThumbnailWorker.Infrastructure.Interface;

namespace ThumbnailWorker.Infrastructure
{
    public class StorageProvider : IStorageProvider
    {
        private readonly AmazonS3Client _s3Client;
        private readonly S3Config _s3Config;

        public StorageProvider(IOptions<S3Config> s3Option)
        {
            _s3Config = s3Option.Value;
            var config = new AmazonS3Config
            {
                ServiceURL = _s3Config.EndPoint,
                ForcePathStyle = true
            };
            _s3Client = new AmazonS3Client(
                _s3Config.AccessKey,
                _s3Config.SecretKey,
                config
            );
        }

        public async Task UploadFileAsync(Stream steam, string filePath)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(steam, _s3Config.BucketName, filePath).ConfigureAwait(false);
        }

        public async Task<GetObjectResponse> GetObjectAsync(string filePath)
        {
            return await _s3Client.GetObjectAsync(_s3Config.BucketName, filePath).ConfigureAwait(false);
        }
    }
}
