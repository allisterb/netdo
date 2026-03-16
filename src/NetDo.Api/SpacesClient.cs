namespace DigitalOcean.Api;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Amazon.S3;
using Amazon.S3.Model;

public class SpacesClient : Runtime
{
    #region Constructors
    public SpacesClient(string endpoint, string accessKeyId, string accessKeySecret)
    {        
        this.s3Client = new AmazonS3Client(accessKeyId, accessKeySecret, new AmazonS3Config { ServiceURL = endpoint });
    }
    #endregion

    #region Methods
    public async Task<string[]> ListBucketsAsync()
    {
        using var op = Begin("Listing buckets");
        var response = await this.s3Client.ListBucketsAsync();
        var buckets = response.Buckets.Select(b => b.BucketName).ToArray();
        op.Complete();
        return buckets;
    }
    
    public async Task<byte[]> GetObjectAsync(string bucketName, string key, string? versionid = null)
    {
        using var op = Begin("Getting object {0} from bucket {1}", key, bucketName);
        var request = new GetObjectRequest()
        {
            BucketName = bucketName,
            Key = key,            
        };
        if (versionid != null)
        {
            request.VersionId = versionid;
        }
        using var response = await this.s3Client.GetObjectAsync(request);
        using var ms = new MemoryStream();
        await response.ResponseStream.CopyToAsync(ms);
        op.Complete();
        return ms.ToArray();
    }

    public async Task PutObjectAsync(string bucketName, string key, byte[] content, string contentType = "application/json")
    {
        using var op = Begin("Putting object {0} in bucket {1}", key, bucketName);
        using var ms = new MemoryStream(content);
        var request = new PutObjectRequest()
        {
            BucketName = bucketName,
            Key = key,
            InputStream = ms,
            ContentType = contentType
        };

        var response = await this.s3Client.PutObjectAsync(request);
        op.Complete();
        
    }
       
    #endregion

    #region Fields
    protected readonly AmazonS3Client s3Client;
    #endregion
}
