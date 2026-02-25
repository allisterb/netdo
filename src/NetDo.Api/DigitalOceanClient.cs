namespace DigitalOcean.Api;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

public partial class DigitalOceanClient : Runtime 
{
    public DigitalOceanClient(string apikey) : this(GetHttpClient(apikey)) {}
       
    protected static HttpClient GetHttpClient(string apikey)
    {
        // create and configure HttpClient
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.digitalocean.com/v2/")
        };

        // set the Authorization: Bearer <token> header
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

        return httpClient;        
    }
}
