namespace DigitalOcean.Api;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.S3;

public class SpacesClient : Runtime
{
    #region Constructors
    public SpacesClient(string endpoint, string accessKeyId, string accessKeySecret)
    {        
        this.s3Client = new AmazonS3Client(accessKeyId, accessKeySecret, new AmazonS3Config { ServiceURL = endpoint });
    }
    #endregion

    #region Methods
    public string[] ListBuckets()
    {        
        var response = this.s3Client.ListBucketsAsync().GetAwaiter().GetResult();
        var buckets = response.Buckets.Select(b => b.BucketName).ToArray();       
        return buckets;
    }
    #endregion

    #region Fields
    protected readonly AmazonS3Client s3Client;
    #endregion
}
