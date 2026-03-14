namespace DigitalOcean.Api;

using System;
using System.Collections.Generic;
using System.Text;

using Amazon.S3;
public class SpacesClient : Runtime
{
    public SpacesClient(string bucketName)
    {
        this.bucketName = bucketName;
    }

    #region Fields
    protected readonly string bucketName;
    #endregion
}
