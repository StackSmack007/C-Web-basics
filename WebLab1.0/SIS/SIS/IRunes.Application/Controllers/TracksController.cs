namespace IRunes.Application.Controllers
{
    using IRunes.Application.Enums;
    using IRunes.Infrastructure.Models.Models;
    using Microsoft.EntityFrameworkCore;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using System;
    using System.Linq;

    public class TracksController : BaseController
    {
        public IHttpResponse Create(IHttpRequest request)
        {
            ViewData["albumId"] = request.QueryData["albumId"].ToString();
            return View();
        }

        public IHttpResponse CreateData(IHttpRequest request)
        {
            string songName = request.FormData["name"].ToString();
            string albumId = request.FormData["albumId"].ToString();
            string linkURL = request.FormData["linkURL"].ToString();
            decimal price = decimal.Parse(request.FormData["price"].ToString());

            if (!IsUserLogedIn(request))
            {
                return ResposeErrorMessageAndRedirect("No User loged in. Log in first");
            }

            Track foundTrack = db.Tracks.Include(x => x.TrackAlbums).FirstOrDefault(x => x.Name == songName);

            if (foundTrack is null)
            {
                Track newTrack = new Track() { Name = songName, LinkURL = linkURL, Price = price };
                newTrack.TrackAlbums.Add(new AlbumTrack() { AlbumId = albumId });
                db.Tracks.Add(newTrack);
            }
            else if (foundTrack.TrackAlbums.Any(x => x.AlbumId == albumId) && !request.FormData.ContainsKey("overwriteChoise"))
            {
                return ResposeErrorMessageAndRedirect($"The track with name {songName} is already included in the album please provide choise on how to proceed.", $"/Tracks/Create?albumId={albumId}", "Add Song To Album");
            }
            else if (!request.FormData.ContainsKey("overwriteChoise"))
            {
                return ResposeErrorMessageAndRedirect($"The track with name {songName} is already included in the database please provide choise on how to proceed.", $"/Tracks/Create?albumId={albumId}", "Add Song To Album");
            }
            else
            {
                OverwriteChoise choise = (OverwriteChoise)int.Parse(request.FormData["overwriteChoise"].ToString());
                bool isPresentInThisAlbum = foundTrack.TrackAlbums.Any(x => x.AlbumId == albumId);
                if (choise == OverwriteChoise.MakeNew)
                {
                    Track newTrack = new Track() { Name = songName, LinkURL = linkURL, Price = price };
                    newTrack.TrackAlbums.Add(new AlbumTrack() { AlbumId = albumId });
                    db.Tracks.Add(newTrack);
                }
                if (choise == OverwriteChoise.OverWriteExisting)
                {
                    foundTrack.Price = price;
                    foundTrack.LinkURL = linkURL;
                    if (!isPresentInThisAlbum)
                    {
                        foundTrack.TrackAlbums.Add(new AlbumTrack() { AlbumId = albumId });
                    }
                }
                if (choise == OverwriteChoise.UseExisting && !isPresentInThisAlbum)
                {
                    foundTrack.TrackAlbums.Add(new AlbumTrack() { AlbumId = albumId });
                }
            }
            db.SaveChanges();
            request.FormData.Clear();
            request.QueryData["albumId"] = albumId.ToString();
            return new AlbumsController().Details(request);
        }


        public IHttpResponse Details(IHttpRequest request)
        {
            string trackId = request.QueryData["trackId"].ToString();
            string albumId = request.QueryData["albumId"].ToString();
            if (!IsUserLogedIn(request))
            {
                return ResposeErrorMessageAndRedirect("No User loged in. Log in first", $"/Albums/Details?albumId={albumId}", "Album");
            }

            Track foundtrack = db.Tracks.FirstOrDefault(x => x.Id == trackId);
            if (foundtrack is null)
            {
                return ResposeErrorMessageAndRedirect("Track Not Found", $"/Albums/Details?albumId={albumId}", "Album");
            }
            ViewData["trackName"] = foundtrack.Name;
            ViewData["trackPrice"] = foundtrack.Price.ToString("F2");
            ViewData["trackURL"] = DecodeUrl( foundtrack.LinkURL).Split('/').Last();
            ViewData["albumId"] = albumId;
            return View();
        }
        public IHttpResponse DetachTrackFromAlbum(IHttpRequest request)
        {
            string trackId = request.QueryData["trackId"].ToString();
            string albumId = request.QueryData["albumId"].ToString();
            if (!IsUserLogedIn(request))
            {
                return ResposeErrorMessageAndRedirect("No User loged in. Log in first", $"/Albums/Details?albumId={albumId}", "Album");
            }
            string userId = GetCurrentSessionUserIdandName(request)[0];
            Album foundAlbum = db.Albums.Include(x=>x.AlbumTracks).FirstOrDefault(x=>x.Id==albumId);

            Track foundTrack = db.Tracks.FirstOrDefault(x => x.Id == trackId);
            if (foundAlbum is null)
            {
                return ResposeErrorMessageAndRedirect("No such album in database", $"/Albums/Details?albumId={albumId}", "Album");
            }

            if (foundTrack is null)
            {
                return ResposeErrorMessageAndRedirect("No such track in database", $"/Albums/Details?albumId={albumId}", "Album");
            }

            if (!foundAlbum.AlbumTracks.Any(x=>x.TrackId==trackId))
            {
                return ResposeErrorMessageAndRedirect("This album does not contain the song requested for removal", $"/Albums/Details?albumId={albumId}", "Album");
            }

            if (foundAlbum.UserCreatorID!=userId)
            {
                return ResposeErrorMessageAndRedirect("This user is not authorised to remove tracks from list", $"/Albums/Details?albumId={albumId}", "Album");
            }
            var albumTrack = foundTrack.TrackAlbums.First(x => x.AlbumId == albumId);
            foundTrack.TrackAlbums.Remove(albumTrack);
            db.SaveChanges();
            return new AlbumsController().Details(request);
        }

    }
}
