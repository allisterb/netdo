namespace DigitalOcean.Tests.Api;

using DigitalOcean.Api;
using System;

public class SpacesTests : ApiTests
{
    [Fact]
    public async Task CanListBuckets()
    {
        var space = new SpacesClient(spacesEndpoint, spacesKeyId, spacesKeySecret);
        var b = await space.ListBucketsAsync();
        Assert.NotEmpty(b);
    }
}
