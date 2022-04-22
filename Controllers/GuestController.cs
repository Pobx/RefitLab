using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
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

    [HttpGet ("identityJwt")]
    public async Task<ActionResult<object>> identityJwt ()
    {
      var clientCertificateHandler = new HttpClientHandler ();
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      var httpClient = new HttpClient (clientCertificateHandler);
      httpClient.BaseAddress = new Uri ("https://localhost:6001");

      var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkQwQzQ0ODA3MTlENjFDQTU5NDkwRUYwQ0VCRkJFNkFGIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDc5MzY0NTQsImV4cCI6MTY0NzkzNjc1NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6WyJyby5jbGllbnQudG9rZW4iLCJodHRwczovL2xvY2FsaG9zdDo1MDAxL3Jlc291cmNlcyJdLCJjbGllbnRfaWQiOiJyby5jbGllbnQuand0Iiwic3ViIjoiMSIsImF1dGhfdGltZSI6MTY0NzkzNjQ1NCwiaWRwIjoibG9jYWwiLCJqdGkiOiJENjEwMzBCMDUxNjNBQjI5ODRFMUIyQTBCQTg5OTMwNyIsImlhdCI6MTY0NzkzNjQ1NCwic2NvcGUiOlsibGV2ZWwxIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.QQZVHJhf3uO6dkp2vorgcgkJogyEFMyC-aBzbvenCG0BO6EAqf1XOCuFMFKICGIU3qO8RCLBUpCuwtNoWqHuk3cl41XZu4zCXLAZ_5zeCmhgPIS9Z2Kji0B7qTHRpH6TNAZbKJez2hEbJhvmV_RKpbXtoVD802bFisIygQ8Rn35L_EefO663hmYSUT6joFX-r6IlYCmUPW5k6UcdArltiBfm8IYCJ87iNxQpIgulDDm0jYkCnD_mMjrPWGBo6rpkjIaDxxQvYb_oZe8eE7aMYizDzdciqZxPwIHg4gAY-3fCp9sgaZ9EARd_t7tVgUV0JRqWaIXzts-rbSelDJzFJQ";
      var api = RestService.For<IpobxApi2> (httpClient);
      var response = await api.identity (token);
      Console.WriteLine (response);

      return Ok (response);
    }

    [HttpGet ("identityToken")]
    public async Task<ActionResult<object>> identityToken ()
    {
      var clientCertificateHandler = new HttpClientHandler ();
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      var httpClient = new HttpClient (clientCertificateHandler);
      httpClient.BaseAddress = new Uri ("https://localhost:6001");

      var token = "8243958D653502A520D08DCCA9BB38AF66C3475FDD7B4FFFFA6F92980F3039CC";
      var api = RestService.For<IpobxApi2> (httpClient);
      var response = await api.identity (token);
      Console.WriteLine (response);

      return Ok (response);
    }

    [HttpGet ("token-by-password-jwt")]
    public async Task<ActionResult> tokenByPasswordJwt ()
    {
      var clientCertificateHandler = new HttpClientHandler ();
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      var httpClient = new HttpClient (clientCertificateHandler);

      var options = new TokenClientOptions
      {
        Address = "https://localhost:5001/connect/token",
        ClientId = "ro.client.jwt",
        ClientSecret = "secret1234",
      };

      var tokenClient = new TokenClient (httpClient, options);
      var tokenResponse = await tokenClient.RequestPasswordTokenAsync ("pobx", "1234", "level1 offline_access");

      if (tokenResponse.IsError)
      {
        Console.WriteLine (tokenResponse.Error);
        return NoContent ();
      }

      Console.WriteLine (tokenResponse.Json);
      Console.WriteLine ("\n\n");

      return Ok (tokenResponse.Json);
    }

    [HttpGet ("token-by-password-token")]
    public async Task<ActionResult> tokenByPasswordToken ()
    {
      var clientCertificateHandler = new HttpClientHandler ();
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      var httpClient = new HttpClient (clientCertificateHandler);

      var options = new TokenClientOptions
      {
        Address = "https://localhost:5001/connect/token",
        ClientId = "ro.client.token",
        ClientSecret = "secret1234",
      };

      var tokenClient = new TokenClient (httpClient, options);
      var tokenResponse = await tokenClient.RequestPasswordTokenAsync ("pobx", "1234", "level1 openid profile offline_access");

      if (tokenResponse.IsError)
      {
        Console.WriteLine (tokenResponse.Error);
        return NoContent ();
      }

      Console.WriteLine (tokenResponse.Json);
      Console.WriteLine ("\n\n");

      return Ok (tokenResponse.Json);
    }

    [HttpGet ("userinfo")]
    public async Task<ActionResult> userInfo ()
    {
      var clientCertificateHandler = new HttpClientHandler ();
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      var httpClient = new HttpClient (clientCertificateHandler);
      httpClient.BaseAddress = new Uri ("https://localhost:5001");

      var token = "8243958D653502A520D08DCCA9BB38AF66C3475FDD7B4FFFFA6F92980F3039CC";
      var response = await httpClient.GetUserInfoAsync (new UserInfoRequest
      {
        Address = "/connect/userinfo",
          Token = token
      });

      if (response.IsError)
      {
        Console.WriteLine (response.Error);
        return NoContent ();
      }

      Console.WriteLine (response.Json);
      Console.WriteLine ("\n\n");

      return Ok (response.Json);
    }

    [HttpGet ("Revocation")]
    public async Task<object> Revocation ()
    {
      var clientCertificateHandler = new HttpClientHandler ();
      clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
      var client = new HttpClient (clientCertificateHandler);
      client.BaseAddress = new Uri ("https://localhost:5001");
      var response = await client.RevokeTokenAsync (new TokenRevocationRequest
      {
        Address = "/connect/revocation",
          ClientId = "ro.client.token",
          ClientSecret = "secret1234",

          Token = "8243958D653502A520D08DCCA9BB38AF66C3475FDD7B4FFFFA6F92980F3039CC"
      });

      if (response.IsError)
      {
        Console.WriteLine (response.Error);
        return NoContent ();
      }

      Console.WriteLine (response.Raw);
      return Ok (response.Raw);
    }

    [HttpGet ("weather")]
    public async Task<object> weather ()
    {
      var response = await _ipobxApi.weather ();
      Console.WriteLine (response);

      return Ok (response);
    }

  }

  public interface IpobxApi
  {
    [Get ("/api/WeatherForecast/all")]
    Task<object> weather ();

    [Post ("/connect/token")]
    Task<object> getToken ([Body (BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

    [Get ("/.well-known/openid-configuration")]
    Task<object> disCovery ();
  }

  public interface IpobxApi2
  {
    [Get ("/identity")]
    Task<object> identity ([Authorize ("Bearer")] string token);

    [Get ("/identity2")]
    Task<object> identity2 ([Authorize ("introspection")] string token);
  }

  public record PostEntity (int userId, int id, string title, string body);
}
