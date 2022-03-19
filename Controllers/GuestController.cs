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

      var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkQwQzQ0ODA3MTlENjFDQTU5NDkwRUYwQ0VCRkJFNkFGIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDc2ODEwMzksImV4cCI6MTY0NzY4NDYzOSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDEvcmVzb3VyY2VzIiwiY2xpZW50X2lkIjoicm8uY2xpZW50Iiwic3ViIjoiMSIsImF1dGhfdGltZSI6MTY0NzY4MTAzOSwiaWRwIjoibG9jYWwiLCJqdGkiOiI2RUYzQUU1ODBGMzI3Q0VBMTk0QjU3QTZDNUMxNjY3NSIsImlhdCI6MTY0NzY4MTAzOSwic2NvcGUiOlsibGV2ZWwxIl0sImFtciI6WyJwd2QiXX0.yFi7qB2kmG6QPKIfSRfvit_r3BpKJyCu1mq_0oJNPumorHBQzqiLocfLCkTINThYlOLaPCZm0zXMZZ5judMK3sxDlx5JSRbrXTnqwGItPPSON3cmzAcibPxwvVr4x_-4adAlPZYdF5_UL6yXdoV7zPxuekajhTl5S8UBK_HQnDtcn0excsgbLkAy_dUDXHyBLJlUJ_K6FSslT6dL2m_Q6GsWLAJjMSEmD3pFE6onWGb5B_C83m0jKBz0vosfh-hSVUGK5Ne2GQCNNS92a5lBoAm80hh-V4UR11vcTuhCh81VQSLQF4ISMyt-2rQvIDoDsndIa08X_PMTFnRE7f6zNQ";
      var api = RestService.For<IpobxApi2> (httpClient);
      var response = await api.identity (token);
      Console.WriteLine (response);

      return Ok (response);
    }

    [HttpGet ("token-by-password")]
    public async Task<ActionResult> tokenByPassword ()
    {
      var clientCertificateHandler = new HttpClientHandler ();
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      var httpClient = new HttpClient (clientCertificateHandler);

      var options = new TokenClientOptions
      {
        Address = "https://localhost:5001/connect/token",
        ClientId = "ro.client",
        ClientSecret = "secret1234",
      };

      var tokenClient = new TokenClient (httpClient, options);
      var tokenResponse = await tokenClient.RequestPasswordTokenAsync ("pobx", "1234", "level1");

      if (tokenResponse.IsError)
      {
        Console.WriteLine (tokenResponse.Error);
        return NoContent ();
      }

      Console.WriteLine (tokenResponse.Json);
      Console.WriteLine ("\n\n");

      return Ok (tokenResponse.Json);
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
