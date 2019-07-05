namespace IRunes.Application.Controllers
{
    using IRunes.Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using System.Linq;
    using System.Text;

    public class AlbumsController : BaseController
    {
        public IHttpResponse All(IHttpRequest request)
        {
            if (!this.IsUserLogedIn(request))
            {
                return ResposeErrorMessageAndRedirect("No user currently logged in");
            }

            string userId = GetCurrentSessionUserIdandName(request)[0];

            var albumsDTOs = db.Albums.Select(x => new { x.Id, x.Name, IsCreatedByThisUser = userId == x.UserCreatorID }).ToArray();
            StringBuilder sb = new StringBuilder();
            foreach (var dto in albumsDTOs.OrderByDescending(x => x.IsCreatedByThisUser))
            {
                if (dto.IsCreatedByThisUser)
                {
                    sb.Append($"<p><a href=\"/Albums/Details?albumId={dto.Id}\" style=\"color:darkgreen\">{dto.Name}</a></p>");
                }
                else
                {
                    sb.Append($"<p><a href=\"/Albums/Details?albumId={dto.Id}\" style=\"color:brown\">{dto.Name}</a></p>");
                }
            }
            ViewData["albumsList"] = sb.ToString();

            return View();

        }

        public IHttpResponse Create(IHttpRequest request)
        {
            if (!this.IsUserLogedIn(request))
            {
                return ResposeErrorMessageAndRedirect("No user currently logged in");
            }
            return View();
        }

        public IHttpResponse CreateData(IHttpRequest request)
        {
            string albumName = request.FormData["name"].ToString();
            string coverUrl = request.FormData["coverURL"].ToString();
            string creatorId = GetCurrentSessionUserIdandName(request)[0];
            Album newAlbum = new Album() { Name = albumName, CoverImgUrl = coverUrl, UserCreatorID = creatorId };

            if (db.Albums.Any(x => x.Name == albumName))
            {
                return ResposeErrorMessageAndRedirect("Album name already exists", "/Albums/Create", "Create Album");
            }
            db.Albums.Add(newAlbum);
            db.SaveChanges();
            return All(request);
        }

        public IHttpResponse Details(IHttpRequest request)
        {
            string albumId = request.QueryData["albumId"].ToString();
            Album foundAlbum = db.Albums.Include(a => a.AlbumTracks)
                                        .ThenInclude(at => at.Track)
                                        .FirstOrDefault(x => x.Id == albumId);
            if (foundAlbum is null)
            {
                return ResposeErrorMessageAndRedirect("Album not found", "/Albums/All", "View All Albums");
            }
            if (!this.IsUserLogedIn(request))
            {
                return ResposeErrorMessageAndRedirect("No user currently logged in");
            }
            string[] currentUserInfo = GetCurrentSessionUserIdandName(request);
            string currentUserId = currentUserInfo[0];
            bool IsAuthorOfAlbum = currentUserId == foundAlbum.UserCreatorID;

            ViewData["albumName"] = foundAlbum.Name;
            ViewData["albumPrice"] = foundAlbum.Price.ToString("F2");
            ViewData["imgURL"] = DecodeUrl(foundAlbum.CoverImgUrl);
            ViewData["creatorName"] = currentUserInfo[1];
            ViewData["addTrackOrNot"] = string.Empty;

            if (IsAuthorOfAlbum)
            {
                ViewData["addTrackOrNot"] = $"<hr/><h2><a href=\"/Tracks/Create?albumId={albumId}\" style=\"color:goldenrod\">Create new track &#x266B</a></h2><hr/>";
            }

            ViewData["tracksList"] = "<p>No songs added yet!</p>";
            if (foundAlbum.AlbumTracks.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<ul>");
                int counter = 0;
                foreach (Track track in foundAlbum.AlbumTracks.Select(x => x.Track))
                {
                    string deleteOption = IsAuthorOfAlbum ? $"  <a href=\"/Tracks/Detach?trackId={track.Id}&albumId={albumId}\" style=\"text-decoration:none\"><span style=\"color:red\"><strong> &#10007 </strong></span> </a>" : "";
                    sb.Append($"<li>{++counter}. <a href=\"/Tracks/Details?trackId={track.Id}&albumId={albumId}\">{track.Name}</a>{deleteOption}</li>");
                }
                sb.Append("</ul>");
                ViewData["tracksList"] = sb.ToString();
            }
            return View();
        }

    }
}