using Microsoft.EntityFrameworkCore;
using MishMashApplication.DTO.Channels;
using MishMashApplication.Models;
using SIS.HTTP.Responses.Contracts;
using SIS.MVC.Attributes;
using System.Linq;

namespace MishMashApplication.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet("/")]
        [HttpGet("/Home/")]
        [HttpGet("/Home/Index")]
        [HttpGet("/Index")]
        public IHttpResponse Index()
        {
            ViewData["ChannelsYour"] = new ChannelDTO[0];
            ViewData["ChannelsSuggested"] = new ChannelDTO[0];
            ViewData["ChannelsSeeOther"] = new ChannelDTO[0];
            ViewData["UserRole"] = string.Empty;
            if (CurentUser is null)
            {
                return this.View();
            }

            User foundUser = db.Users.FirstOrDefault(x => x.Id == CurentUser.Id);
            ViewData["UserRole"] = foundUser.Role.ToString();

            ChannelDTO[] allChannelsDtos = db.Channels.Select(x => new ChannelDTO
            {
                ChannelId = x.Id,
                Name = x.Name,
                Description = x.Description,
                FollowersIds = x.ChannelUsers.Select(cu => cu.UserId).ToArray(),
                Tags = x.ChannelTags.Select(ct => ct.Tag.Name).ToArray(),
                ChannelType=x.Type
            }).ToArray();

            ChannelDTO[] channelsYour = allChannelsDtos.Where(x => x.FollowersIds.Contains(CurentUser.Id)).ToArray();
            string[] allOfMyTags = channelsYour.SelectMany(x => x.Tags).Select(x => x.ToLower()).Distinct().ToArray();

            ChannelDTO[] channelsSuggested = allChannelsDtos.Where(x => !channelsYour.Select(c => c.ChannelId).Contains(x.ChannelId) &&
                                                                   x.Tags.Any(t => allOfMyTags.Contains(t.ToLower()))).ToArray();

            ChannelDTO[] channelsOther = allChannelsDtos.Where(x => !(channelsYour.Contains(x) || channelsSuggested.Contains(x))).ToArray();

            this.ViewData["ChannelsYour"] = channelsYour;
            this.ViewData["ChannelsSuggested"] = channelsSuggested;
            ViewData["ChannelsSeeOther"] = channelsOther;
            return this.View();
        }

    }
}