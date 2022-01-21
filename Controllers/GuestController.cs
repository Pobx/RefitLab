using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RefitLab.Controllers
{
    [Route ("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IPostApi _iPostApi;
        public GuestController (IPostApi iPostApi)
        {
            _iPostApi = iPostApi;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PostEntity>>> getPosts ()
        {
            var response = await _iPostApi.GetPosts ();

            return Ok (response);
        }

        [HttpPost]
        public async Task<ActionResult<PostEntity>> createPost ()
        {
            PostEntity entity = new (1, 0, "Pobx", "Hello Pobx !");
            string token = "1234";

            var response = await _iPostApi.CreatePost (entity, token);
            return Created ("", response);
        }
    }

    public interface IPostApi
    {
        [Get ("/posts")]
        Task<IReadOnlyList<PostEntity>> GetPosts ();

        [Post ("/posts")]
        Task<PostEntity> CreatePost (PostEntity entity, [Authorize ("Bearer")] string token);
    }

    public record PostEntity (int userId, int id, string title, string body);
}
