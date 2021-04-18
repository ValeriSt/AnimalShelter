using AS.Service.Models;
using AS.Web.Models.ViewModels.AnimalViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AS.Web.Mapping
{
    public class AnimalsWebMapping : Profile
    {
        public AnimalsWebMapping()
        {
            CreateMap<AnimalsIndexServiceModel, AnimalsIndexViewModel>();
        }
    }
}
