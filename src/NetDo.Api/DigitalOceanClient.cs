namespace DigitalOcean.Api;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

public partial class DigitalOceanClient : Runtime 
{
    public DigitalOceanClient(string apikey) : this(ConfigureHttpClient(apikey)) {}
       
    protected static HttpClient ConfigureHttpClient(string apikey)
    {
        var httpClient = new HttpClient();       
        // set the Authorization: Bearer <token> header
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
        return httpClient;        
    }
}
