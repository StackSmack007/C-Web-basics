namespace MishMashApplication.Controllers
{
    using Microsoft.EntityFrameworkCore;
    using MishMashApplication.DTO.Channels;
    using MishMashApplication.Models;
    using SIS.HTTP.Responses.Contracts;
    using SIS.MVC.Attributes;
    using System.Linq;

    public class ChannelsController : BaseController
    {
        [HttpGet("/Channels/Details")]
        public IHttpResponse Details(int id)
        {
            var channel = Db.Channels.Where(x => x.Id == id).Select(x => new ChannelDTO()
            {
                Name = x.Name,
                Description = x.Description,
                FollowersIds = x.ChannelUsers.Select(cu => cu.UserId).ToArray(),
                ChannelType = x.Type,
                Tags = x.ChannelTags.Select(t => t.Tag.Name).ToArray()
            }).FirstOrDefault();
            if (channel is null) return ControllerError("Unfound channel in database");

            ViewData["Channel"] = channel;
            ViewData["Tags"] = string.Join(", ", channel.Tags);
            return View();
        }

        public IHttpResponse Create()
        {
            if (!IsAdmin())
            {
                return this.ControllerError($"Only Admins can add channels", "/Home/Index", "Home");
            }
            return View();
        }

        [HttpPost()]
        public IHttpResponse Create(ChannelDTO newChannel)
        {
            if (Db.Channels.Any(x => x.Name == newChannel.Name))
            {
                return this.ControllerError($"Channel with name {newChannel.Name} already exists", "/Home/Index", "Home");
            }
            Channel channel = new Channel
            {
                Name = newChannel.Name,
                Description = newChannel.Description,
                Type = newChannel.ChannelType,
            };
            var tagsInDb = Db.Tags.Select(x => new { x.Id, x.Name }).ToList();
            foreach (string tagName in newChannel.Tags)
            {
                if (!tagsInDb.Select(t => t.Name.ToLower()).Contains(tagName.ToLower()))
                {
                    channel.ChannelTags.Add(new ChannelTag { Channel = channel, Tag = new Tag { Name = tagName } });
                    tagsInDb.Add(new { Id = -1, Name = tagName });
                    continue;
                }
                int tagId = tagsInDb.First(x => x.Name.ToLower() == tagName.ToLower()).Id;
                if (tagId == -1) continue;
                channel.ChannelTags.Add(new ChannelTag { Channel = channel, TagId = tagId });
            }
            Db.Channels.Add(channel);
            Db.SaveChanges();
            return this.ControllerSuccess($"Channel with name {newChannel.Name} was successfully added in Database", "/Home/Index", "Home");
        }

        public IHttpResponse MyChannels()
        {
            if (CurentUser is null)
            {
                return this.ControllerError($"No logged user", "/Home/Index", "Home");
            }

            ChannelDTO[] channels = Db.Channels.Where(x => x.ChannelUsers.Any(cu => cu.UserId == CurentUser.Id))
                .Select(x => new ChannelDTO
                {
                    ChannelId=x.Id,
                    Name = x.Name,
                    FollowersIds = x.ChannelUsers.Select(cu => cu.UserId).ToArray(),
                    ChannelType = x.Type,
                }).ToArray();
            ViewData["Channels"] = channels;
            return View();
        }

        [HttpPost()]
        public IHttpResponse Unfollow(int id)
        {
            if (CurentUser is null)
            {
                return this.ControllerError($"No logged user", "/Home/Index", "Home");
            }

            User foundUser = Db.Users.Include(x=>x.UserChannels).FirstOrDefault(x => x.Id == CurentUser.Id);
            ChannelUser channelUser = foundUser.UserChannels.FirstOrDefault(x => x.ChannelId == id);
            if (channelUser is null)
            {
                return this.ControllerError($"Channel not followed", "/Home/Index", "Home");
            }
            foundUser.UserChannels.Remove(channelUser);
            Db.SaveChanges();
            return RedirectResult("/Channels/MyChannels");
        }

        [HttpGet()]
        public IHttpResponse Follow(int id)
        {
            if (CurentUser is null)
            {
                return this.ControllerError($"No logged user", "/Home/Index", "Home");
            }

            if (!Db.Channels.Any(x=>x.Id==id))
            {
                return this.ControllerError($"Non existin channel", "/Home/Index", "Home");
            }

            User foundUser = Db.Users.Include(x=>x.UserChannels).FirstOrDefault(x => x.Id == CurentUser.Id);
            if (foundUser.UserChannels.Any(x=>x.ChannelId==id))
            {
                return this.ControllerError($"This channel is folowed already", "/Home/Index", "Home");
            }

            foundUser.UserChannels.Add(new ChannelUser { ChannelId = id });
            Db.SaveChanges();
            return RedirectResult("/Home/Index");
        }
    }
}