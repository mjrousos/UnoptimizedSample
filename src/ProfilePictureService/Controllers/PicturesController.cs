using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProfilePictureService.Models;
using ProfilePictureService.Services;

namespace ProfilePictureService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly ILogger<PicturesController> _logger;
        private readonly IImageService _imageService;

        public PicturesController(ILogger<PicturesController> logger, IImageService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        // POST api/pictures/{userId}
        /// <summary>
        /// Adds or updates a user's profile picture
        /// </summary>
        /// <param name="userId">The ID of the user to update</param>
        /// <param name="formFile">The user's new profile picture</param>
        /// <returns>The result of the update</returns>
        [HttpPost("{userId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromRoute] string userId, IFormFile formFile)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            if (formFile == default)
            {
                return BadRequest();
            }

            using (var imageStream = formFile.OpenReadStream())
            {
                var bytes = new byte[imageStream.Length];
                await imageStream.ReadAsync(bytes).ConfigureAwait(false);
                await _imageService.StoreImageAsync(userId, bytes).ConfigureAwait(false);
            }

            return CreatedAtAction(nameof(GetAsync), new { userId = userId }, null);
        }

        // GET api/pictures/{userId}
        /// <summary>
        /// Retrieves a user's profile picture
        /// </summary>
        /// <param name="userId">The ID of the user to find a profile picture for</param>
        /// <returns>The user's profile picture as a base 64 string</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            var userImage = await _imageService.GetImageAsync(userId).ConfigureAwait(false);
            if (userImage == default)
            {
                return NotFound();
            }
            else
            {
                return File(userImage, "image/jpeg");
            }
        }

        // DELETE api/pictures/{userId}
        /// <summary>
        /// Delete a user's profile picture
        /// </summary>
        /// <param name="userId">The ID of the user whose profile picture should be deleted</param>
        /// <returns>The result of the delete</returns>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            await _imageService.RemoveImageAsync(userId).ConfigureAwait(false);
            return Ok();
        }
    }
}
