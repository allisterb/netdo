namespace DigitalOcean.Api;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

public partial class DigitalOceanClient : Runtime 
{
    public DigitalOceanClient(string apikey) : this(ConfigureHttpClient(apikey)) {}

    public DigitalOceanClient() :
        this(Environment.GetEnvironmentVariable("DIGITALOCEAN_API_TOKEN") 
            ?? throw new ArgumentNullException("The DIGITALOCEAN_API_TOKEN environment variable is not set.")) {}
    
    protected static HttpClient ConfigureHttpClient(string apikey)
    {
        var httpClient = new HttpClient();       
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        return httpClient;        
    }
}
