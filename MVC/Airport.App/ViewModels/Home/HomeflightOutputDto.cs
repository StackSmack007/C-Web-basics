using System;

namespace Airport.App.ViewModels.Home
{
    public class HomeflightOutputDto
    {
        public int Id { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public string ImgURL { get; set; }
        public DateTime Date { get; set; }
    }
}
