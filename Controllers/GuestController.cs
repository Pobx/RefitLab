using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace RefitLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IpobxApi _ipobxApi;
        public GuestController(IpobxApi ipobxApi)
        {
            _ipobxApi = ipobxApi;
        }

        [HttpGet("discovery")]
        public async Task<ActionResult<string>> disCovery()
        {
            var response = await _ipobxApi.disCovery();
            Console.WriteLine(response);

            return Ok(response);

        }

        [HttpGet("token")]
        public async Task<object> getToken()
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

            var response = await _ipobxApi.getToken(data);
            Console.WriteLine(response);

            return Ok(response);
        }

        [HttpGet("identityJwt")]
        public async Task<ActionResult<object>> identityJwt()
        {
            var clientCertificateHandler = new HttpClientHandler();
            clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(clientCertificateHandler);
            httpClient.BaseAddress = new Uri("https://localhost:6001");

            var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkQwQzQ0ODA3MTlENjFDQTU5NDkwRUYwQ0VCRkJFNkFGIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDc5MzY0NTQsImV4cCI6MTY0NzkzNjc1NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMSIsImF1ZCI6WyJyby5jbGllbnQudG9rZW4iLCJodHRwczovL2xvY2FsaG9zdDo1MDAxL3Jlc291cmNlcyJdLCJjbGllbnRfaWQiOiJyby5jbGllbnQuand0Iiwic3ViIjoiMSIsImF1dGhfdGltZSI6MTY0NzkzNjQ1NCwiaWRwIjoibG9jYWwiLCJqdGkiOiJENjEwMzBCMDUxNjNBQjI5ODRFMUIyQTBCQTg5OTMwNyIsImlhdCI6MTY0NzkzNjQ1NCwic2NvcGUiOlsibGV2ZWwxIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.QQZVHJhf3uO6dkp2vorgcgkJogyEFMyC-aBzbvenCG0BO6EAqf1XOCuFMFKICGIU3qO8RCLBUpCuwtNoWqHuk3cl41XZu4zCXLAZ_5zeCmhgPIS9Z2Kji0B7qTHRpH6TNAZbKJez2hEbJhvmV_RKpbXtoVD802bFisIygQ8Rn35L_EefO663hmYSUT6joFX-r6IlYCmUPW5k6UcdArltiBfm8IYCJ87iNxQpIgulDDm0jYkCnD_mMjrPWGBo6rpkjIaDxxQvYb_oZe8eE7aMYizDzdciqZxPwIHg4gAY-3fCp9sgaZ9EARd_t7tVgUV0JRqWaIXzts-rbSelDJzFJQ";
            var api = RestService.For<IpobxApi2>(httpClient);
            var response = await api.identity(token);
            Console.WriteLine(response);

            return Ok(response);
        }

        [HttpGet("identityToken")]
        public async Task<ActionResult<object>> identityToken()
        {
            var clientCertificateHandler = new HttpClientHandler();
            clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(clientCertificateHandler);
            httpClient.BaseAddress = new Uri("https://localhost:6001");

            var token = "8243958D653502A520D08DCCA9BB38AF66C3475FDD7B4FFFFA6F92980F3039CC";
            var api = RestService.For<IpobxApi2>(httpClient);
            var response = await api.identity(token);
            Console.WriteLine(response);

            return Ok(response);
        }

        [HttpGet("token-by-password-jwt")]
        public async Task<ActionResult> tokenByPasswordJwt()
        {
            var clientCertificateHandler = new HttpClientHandler();
            clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(clientCertificateHandler);

            var options = new TokenClientOptions
            {
                Address = "https://localhost:5001/connect/token",
                ClientId = "ro.client.jwt",
                ClientSecret = "secret1234",
            };

            var tokenClient = new TokenClient(httpClient, options);
            var tokenResponse = await tokenClient.RequestPasswordTokenAsync("pobx", "1234", "level1 offline_access");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return NoContent();
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            return Ok(tokenResponse.Json);
        }

        [HttpGet("token-by-password-token")]
        public async Task<ActionResult> tokenByPasswordToken()
        {
            var clientCertificateHandler = new HttpClientHandler();
            clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(clientCertificateHandler);

            var options = new TokenClientOptions
            {
                Address = "https://localhost:5001/connect/token",
                ClientId = "ro.client.token",
                ClientSecret = "secret1234",
            };

            var tokenClient = new TokenClient(httpClient, options);
            var tokenResponse = await tokenClient.RequestPasswordTokenAsync("pobx", "1234", "level1 openid profile offline_access");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return NoContent();
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            return Ok(tokenResponse.Json);
        }

        [HttpGet("userinfo")]
        public async Task<ActionResult> userInfo()
        {
            var clientCertificateHandler = new HttpClientHandler();
            clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(clientCertificateHandler);
            httpClient.BaseAddress = new Uri("https://localhost:5001");

            var token = "8243958D653502A520D08DCCA9BB38AF66C3475FDD7B4FFFFA6F92980F3039CC";
            var response = await httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = "/connect/userinfo",
                Token = token
            });

            if (response.IsError)
            {
                Console.WriteLine(response.Error);
                return NoContent();
            }

            Console.WriteLine(response.Json);
            Console.WriteLine("\n\n");

            return Ok(response.Json);
        }

        [HttpGet("Revocation")]
        public async Task<object> Revocation()
        {
            var clientCertificateHandler = new HttpClientHandler();
            clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var client = new HttpClient(clientCertificateHandler);
            client.BaseAddress = new Uri("https://localhost:5001");
            var response = await client.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = "/connect/revocation",
                ClientId = "ro.client.token",
                ClientSecret = "secret1234",

                Token = "8243958D653502A520D08DCCA9BB38AF66C3475FDD7B4FFFFA6F92980F3039CC"
            });

            if (response.IsError)
            {
                Console.WriteLine(response.Error);
                return NoContent();
            }

            Console.WriteLine(response.Raw);
            return Ok(response.Raw);
        }

        [HttpGet("RefreshToken")]
        public async Task RefreshToken()
        {
            // var clientCertificateHandler = new HttpClientHandler();
            // clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            // var client = new HttpClient(clientCertificateHandler);
            // client.BaseAddress = new Uri("https://localhost:5005");
            // var request = client.RequestRefreshTokenAsync(new RefreshTokenRequest
            // {
            //     Address = "/connect/token",
            //     ClientId = "client.kma.token",
            //     ClientSecret = "9076b40d-9a81-47df-9cf0-ed219982d8af",
            //     GrantType = GrantTypes.RefreshToken,
            //     RefreshToken = "29660849BEC1F5F5C73D179E6387AEA462132125F13C26364199396ACC3FD6FC"
            // });


            // var tasks = new List<Task<IdentityModel.Client.TokenResponse>>();

            // tasks.Add(request);
            // tasks.Add(request);

            // var response = (await Task.WhenAll(tasks));
            // Console.WriteLine(JsonConvert.SerializeObject(response));
            // return Ok(JsonConvert.SerializeObject(response));

            var clientCertificateHandler = new HttpClientHandler();
            clientCertificateHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var client = new HttpClient(clientCertificateHandler);
            client.BaseAddress = new Uri("https://localhost:5005");
            var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = "/connect/token",
                ClientId = "client.kma.token",
                ClientSecret = "9076b40d-9a81-47df-9cf0-ed219982d8af",
                GrantType = GrantTypes.RefreshToken,
                RefreshToken = "C089A4CADF274B7A7B48E7B2CE442D9AFC1A65B92282916C5FAAA5F443584BAC"
            });

            if (response.IsError)
            {
              Console.WriteLine (response.Error);
              return;
            }

            Console.WriteLine (response.Raw);
        }

        [HttpGet("weather")]
        public async Task<object> weather()
        {
            var response = await _ipobxApi.weather();
            Console.WriteLine(response);

            return Ok(response);
        }

        [HttpGet("WeatherForecast")]
        public async Task WeatherForecast()
        {
            // Create an HttpClientHandler object and set to use default credentials
            HttpClientHandler handler = new HttpClientHandler();

            // Set custom server validation callback
            handler.ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation;

            // Create an HttpClient object
            HttpClient client = new HttpClient(handler);

            // Call asynchronous network methods in a try/catch block to handle exceptions
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://localhost:7090/WeatherForecast");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Read {responseBody.Length} characters");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine($"Message: {e.Message} ");
            }

            // Need to call dispose on the HttpClient and HttpClientHandler objects
            // when done using them, so the app doesn't leak resources
            handler.Dispose();
            client.Dispose();
        }

        private static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors)
        {
            // It is possible inpect the certificate provided by server
            Console.WriteLine($"Requested URI: {requestMessage.RequestUri}");
            Console.WriteLine($"Effective date: {certificate.GetEffectiveDateString()}");
            Console.WriteLine($"Exp date: {certificate.GetExpirationDateString()}");
            Console.WriteLine($"Thumbprint: {certificate.Thumbprint}");
            Console.WriteLine($"FriendlyName: {certificate.FriendlyName}");
            Console.WriteLine($"Issuer: {certificate.Issuer}");
            Console.WriteLine($"Subject: {certificate.Subject}");
            Console.WriteLine($"certificate: {JsonConvert.SerializeObject(certificate)}");

            // Based on the custom logic it is possible to decide whether the client considers certificate valid or not
            Console.WriteLine($"Errors: {sslErrors}");
            return sslErrors == SslPolicyErrors.None;
        }

        [HttpGet("test-3-routes-favorites")]
        public async Task<object> Test3RoutesFavorites()
        {
            var tasks = new List<Task<object>>();

            tasks.Add(_ipobxApi.FavoriteTransfer());
            tasks.Add(_ipobxApi.BillPay());
            tasks.Add(_ipobxApi.Topup());
            var response = (await Task.WhenAll(tasks));

            return Ok(response);
        }

    }

    public interface IpobxApi
    {
        [Get("/api/WeatherForecast/all")]
        Task<object> weather();

        [Post("/connect/token")]
        Task<object> getToken([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> data);

        [Get("/.well-known/openid-configuration")]
        Task<object> disCovery();

        [Get("/WeatherForecast")]
        Task<object> WeatherForecast();

        [Headers("customerId: @RMU9EF0399D7D8E4C", "Accept-Language: en-US", "x-api-key: d0109ab7-fe3a-4b2f-bddb-4c87e4749d2f", "sourceSystem: 1234")]
        [Get("/favorites/me/transfer")]
        Task<object> FavoriteTransfer();

        [Headers("customerId: @RMU9EF0399D7D8E4C", "Accept-Language: en-US", "x-api-key: d0109ab7-fe3a-4b2f-bddb-4c87e4749d2f", "sourceSystem: 1234")]
        [Get("/favorites/me/biller/billpay")]
        Task<object> BillPay();

        [Headers("customerId: @RMU9EF0399D7D8E4C", "Accept-Language: en-US", "x-api-key: d0109ab7-fe3a-4b2f-bddb-4c87e4749d2f", "sourceSystem: 1234")]
        [Get("/favorites/me/biller/topup")]
        Task<object> Topup();

    }

    public interface IpobxApi2
    {
        [Get("/identity")]
        Task<object> identity([Authorize("Bearer")] string token);

        [Get("/identity2")]
        Task<object> identity2([Authorize("introspection")] string token);
    }

    public record PostEntity(int userId, int id, string title, string body);
}
