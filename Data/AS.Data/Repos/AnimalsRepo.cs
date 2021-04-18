using AS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AS.Data.Repos
{
    public class AnimalsRepo
    {
        private readonly ASDbContext aSDbContext;
        public AnimalsRepo(ASDbContext _dbContext)
        {
            this.aSDbContext = _dbContext;
        }
        public List<ASAnimals> GetAllAnimals()
        {
            return this.aSDbContext.ASAnimals.ToList();
        }

        public int CreateAnimal(ASAnimals animals)
        {
            aSDbContext.Add(animals);
            return aSDbContext.SaveChanges();
        }

        public ASAnimals GetAnimalById(string id)
        {
            return aSDbContext.ASAnimals.FirstOrDefault(x => x.Id == id);
        }

        public int UpdateAnimal(string id ,ASAnimals animals)
        {
            var dbAnimals = this.GetAnimalById(id);
            aSDbContext.Entry(dbAnimals).CurrentValues.SetValues(animals);
            return aSDbContext.SaveChanges();
        }
        public int DeleteAnimalById(string id) 
        {
            var animal = this.GetAnimalById(id);
            this.aSDbContext.Remove(animal);
            return aSDbContext.SaveChanges();
        }
    }
}
