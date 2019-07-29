using MishMashApplication.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MishMashApplication.DTO.Channels
{
    public class ChannelDTO
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<int> FollowersIds { get; set; }
        public int FollowersCount => FollowersIds.Count;

        public ChannelType ChannelType { get; set; }

        public ChannelDTO()
        {
            Tags = new HashSet<string>();
            FollowersIds = new HashSet<int>();
        }

        public ChannelDTO(string name, string description, string tags, string channelType):this()
        {
            Name = name.Replace("+"," ");
            Description = description.Replace("+", " ");
            Tags = new HashSet<string>(tags.Replace("+", " ").Replace("%2C",",").Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
            ChannelType = Enum.Parse<ChannelType>(channelType);            
        }
    //    public string ChannelString => ChannelType.ToString();
    }
}
