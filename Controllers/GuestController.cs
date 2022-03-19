using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace RefitLab.Controllers
{
  [Route ("api/[controller]")]
  [ApiController]
  public class GuestController : ControllerBase
  {
    private readonly IpobxApi _ipobxApi;
    public GuestController (IpobxApi ipobxApi)
    {
      _ipobxApi = ipobxApi;
    }

    [HttpGet ("discovery")]
    public async Task<ActionResult<string>> disCovery ()
    {
      var response = await _ipobxApi.disCovery ();
      Console.WriteLine (response);

      return Ok (response);

    }

    [HttpGet ("token")]
    public async Task<object> getToken ()
    {
      // var entity = new ClientCredentialsTokenRequest
      // {
      //   ClientId = "pobx",
      //   ClientSecret = "secret1234",
      //   Scope = "api1"
      // };

      var data = new Dictionary<string, object>
        { { IdentityModel.OidcConstants.TokenRequest.ClientId, "pobx" },
          { IdentityModel.OidcConstants.TokenRequest.ClientSecret, "secret1234" },
          { IdentityModel.OidcConstants.TokenRequest.Scope, "api1" },
          { IdentityModel.OidcConstants.TokenRequest.GrantType, GrantTypes.ClientCredentials },
        };

      var response = await _ipobxApi.getToken (data);
      Console.WriteLine (response);

      return Ok (response);
    }

    [HttpGet ("identity")]
    public async Task<ActionResult<object>> identity ()
    {
      // var httpClient = new CreateHttpClient("https://localhost:6001");
      var clientCertificateHandler = new HttpClientHandler ();
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      var httpClient = new HttpClient (clientCertificateHandler);
      httpClient.BaseAddress = new Uri ("https://localhost:6001");

      var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkQwQzQ0ODA3MTlENjFDQTU5NDkwRUYwQ0VCRkJFNkFGIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDc1OTIwODQsImV4cCI6MTY0NzU5NTY4NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDEvcmVzb3VyY2VzIiwiY2xpZW50X2lkIjoicG9ieCIsImp0aSI6IjgwQTA5OUExNEQ0QjQwQTNCQkJGNTgyNkNCOUQyQUZEIiwiaWF0IjoxNjQ3NTkyMDg0LCJzY29wZSI6WyJhcGkxIl19.MqVgAaqJeliGPXq8nf4JJkuBKZmyVowRDRVFgFw1mGag1keJXjJf7TBZGUX0w-_yzGl6Jim9IEkv6Xt-iSiaxyuk_ihQppQD1MVdKsXfp7B7IV2TJjnRTTq5oy9KjPUi_pA4UXnmOxFhCiAvW8XE0Ut43SmeG-rb7SLbCNg5kOZKYihhvbRzi-M4lQTENkWw8pEbbSHzemYSQl2Y1dCF777gxGbV2xl9RWwxeCyNj1LvvohggIw9F6BpzEBlxGRpJj-ByUZoEC6r0MySfS6BmdGsOs2eXnbTLZT3tmDUwZF5mpM8WundCCv7W5J6URFGk8A05OxJ-nv2uvTZ2oDFBA";
      var api = RestService.For<IpobxApi2> (httpClient);
      var response = await api.identity (token);
      Console.WriteLine (response);

      return Ok (response);

    }
  }

  public interface IpobxApi
  {

    [Post ("/connect/token")]
    Task<object> getToken ([Body (BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

    [Get ("/.well-known/openid-configuration")]
    Task<object> disCovery ();
  }

  public interface IpobxApi2
  {
    [Get ("/identity")]
    Task<object> identity ([Authorize ("Bearer")] string token);
  }

  public record PostEntity (int userId, int id, string title, string body);
}
