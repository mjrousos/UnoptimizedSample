using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfilePictureService.Models;

namespace ProfilePictureService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        // GET api/pictures/search
        /// <summary>
        /// Initiates a new profile picture search for user IDs provided in the query string
        /// </summary>
        /// <param name="userIds">User IDs to retrieve profile pictures for</param>
        /// <returns>A profile picture collection object containing the requested users' profile pictures</returns>
        [HttpGet("search")]
        public Task<ActionResult<ProfilePicturesDTO>> Search([FromQuery(Name = "userId")] IEnumerable<string> userIds)
        {
            throw new NotImplementedException();
        }

        // POST api/pictures/{userId}
        /// <summary>
        /// Adds or updates a user's profile picture
        /// </summary>
        /// <param name="userId">The ID of the user to update</param>
        /// <param name="formFile">The user's new profile picture</param>
        /// <returns>The result of the update</returns>
        [HttpPost("{userId}")]
        public Task<ActionResult> Post([FromRoute] string userId, [FromBody] IFormFile formFile)
        {
            throw new NotImplementedException();
        }

        // GET api/pictures/{userId}
        /// <summary>
        /// Retrieves a user's profile picture
        /// </summary>
        /// <param name="userId">The ID of the user to find a profile picture for</param>
        /// <returns>The user's profile picture as a base 64 string</returns>
        [HttpGet("{userId}")]
        public Task<ActionResult<string>> Get(string userId)
        {
            throw new NotImplementedException();
        }

        // DELETE api/pictures/{userId}
        /// <summary>
        /// Delete a user's profile picture
        /// </summary>
        /// <param name="userId">The ID of the user whose profile picture should be deleted</param>
        /// <returns>The result of the delete</returns>
        [HttpDelete("{userId}")]
        public Task<ActionResult> Delete(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
