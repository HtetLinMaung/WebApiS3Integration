using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
// using System.IO;
// using System.Threading.Tasks;

namespace WebApiS3Integration.Services
{
    public class S3FileService
    {
        private readonly AmazonS3Client _s3Client;

        public S3FileService(IConfiguration configuration)
        {
            var awsOptions = configuration.GetSection("AWS");
            var accessKeyId = awsOptions["AccessKeyId"];
            var secretAccessKey = awsOptions["SecretAccessKey"];
            var region = awsOptions["Region"];

            _s3Client = new AmazonS3Client(accessKeyId, secretAccessKey, RegionEndpoint.GetBySystemName(region));
        }

        public async Task UploadFileAsync(string bucketName, string key, Stream fileStream)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = fileStream,
                AutoCloseStream = true
            };

            await _s3Client.PutObjectAsync(putRequest);
        }

        public async Task<Stream> DownloadFileAsync(string bucketName, string key)
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(getRequest);
            return response.ResponseStream;
        }

        public async Task DeleteFileAsync(string bucketName, string key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        // Method to generate a signed URL
        public string GetPreSignedURL(string bucketName, string key, int durationInMinutes)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(durationInMinutes)
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }

}