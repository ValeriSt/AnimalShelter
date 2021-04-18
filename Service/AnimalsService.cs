using AS.Data.Models;
using AS.Data.Repos;
using AS.Service.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service
{
    public class AnimalsService
    {
        private readonly AnimalsRepo animalsRepo;
        private readonly IMapper autoMapper;
        private readonly UserRepo userRepo;

        public AnimalsService(AnimalsRepo animalsRepo, IMapper autoMapper, UserRepo userRepo)
        {
            this.animalsRepo = animalsRepo;
            this.autoMapper = autoMapper;
            this.userRepo = userRepo;
        } 
        public async Task<List<AnimalsIndexServiceModel>> IndexAnimals(ClaimsPrincipal userClaims)
        {
            var animals = this.animalsRepo.GetAllAnimals().Select(autoMapper.Map<AnimalsIndexServiceModel>).ToList();

            foreach (var animal in animals)
            {
                animal.IsAuthorized = await IsAuthorized(animal.Id, userClaims);
            }
            return animals;
        }
        public async Task CreateAnimal(ASAnimals animals, ClaimsPrincipal userClaims)
        {
            var user = await userRepo.GetUserAsync(userClaims);
            animals.UserId = user.Id;
            animals.Id = Guid.NewGuid().ToString();
            animalsRepo.CreateAnimal(animals);
        } 
        private async Task<bool> IsAuthorized(string animalId, ClaimsPrincipal userClaims)
        {
            var user = await userRepo.GetUserAsync(userClaims);
            var animals = this.animalsRepo.GetAnimalById(animalId);
            return await userRepo.IsInAdminAsync(user) || animals.UserId == user.Id;
        }
    }
}
