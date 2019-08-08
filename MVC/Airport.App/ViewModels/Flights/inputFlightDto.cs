using System;
using System.Globalization;

namespace Airport.Infrastructure.Models.Models
{
    public class inputFlightDto 
    {
        public inputFlightDto(string origin, string destination, string date, string time, string image,int isPublic):this(origin,destination,date,time,image)
        {
            Origin = origin;
            Destination = destination;
            Date = DateTime.ParseExact(date + "|" + time, "yyyy-MM-dd|H:mm", CultureInfo.InvariantCulture);
            ImgURL = image;
            IsPublic = isPublic==1?true:false;
        }

        public inputFlightDto(string origin, string destination, string date, string time, string image)
        {
            Origin = origin;
            Destination = destination;
            Date = DateTime.ParseExact(date + "|" + time, "yyyy-MM-dd|H:mm", CultureInfo.InvariantCulture);
            ImgURL = image;
            IsPublic =  false;
        }
        public string Origin { get; set; }
        public string Destination { get; set; }
      public  DateTime Date { get; set; }
        public string ImgURL { get; set; }
        public bool IsPublic { get; set; }    
    }
}