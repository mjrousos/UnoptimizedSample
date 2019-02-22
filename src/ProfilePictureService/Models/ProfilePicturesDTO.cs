using System.Collections.Generic;

namespace ProfilePictureService.Models
{
    /// <summary>
    /// Data transfer object for transporting a collection of user profile pictures
    /// </summary>
    public class ProfilePicturesDTO
    {
        /// <summary>
        /// Key/value pairs of user names and profile pictures (encoded as base 64 strings)
        /// </summary>
        public Dictionary<string, string> UserProfilePictures { get; }
    }
}
