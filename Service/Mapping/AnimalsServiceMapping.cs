using AS.Data.Models;
using AS.Service.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AS.Service.Mapping
{
    public class AnimalsServiceMapping : Profile
    {
        public AnimalsServiceMapping()
        {
            CreateMap<ASAnimals, AnimalsIndexServiceModel > ();
        }
    }
}
