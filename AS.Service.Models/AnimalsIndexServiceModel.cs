using System;

namespace AS.Service.Models
{
    public class AnimalsIndexServiceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string UserId { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
