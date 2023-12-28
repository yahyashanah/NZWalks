using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext context;
        public SQLWalkRepository(NZWalksDbContext context)
        {
            this.context = context;
        }
        public async Task<Walk> CreateWalkAsync(Walk walk)
        {
            await context.Walks.AddAsync(walk);
            await context.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteWalkAsync(Guid id)
        {
            var walkById = await context.Walks.FirstOrDefaultAsync(x => x.Id == id);

            if (walkById == null)
            {
                return null;
            }

            context.Walks.Remove(walkById);
            await context.SaveChangesAsync();
            return walkById;
        }
        
        public async Task<List<Walk>> GetAllAsync(string? filterOn, string? filterQuery, string? sortBy, bool isAscending, int pageNumber, int pageSize)
        {
            var walks = context.Walks.Include("Difficulty").Include("Region").AsQueryable();

            // Filtering
            if(string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
            }

            // Sorting
            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if(sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
            //return await context.Walks.Include("Difficulty").Include("Region").ToListAsync();
        }

        public async Task<Walk?> GetWalkAsync(Guid id)
        {
            return await context.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var walkDomainModel = await context.Walks.FirstOrDefaultAsync(x => x.Id == id);

            if(walkDomainModel == null)
            {
                return null;
            }

            walkDomainModel.Name = walk.Name;
            walkDomainModel.Description = walk.Description;
            walkDomainModel.LengthInKm = walk.LengthInKm;
            walkDomainModel.WalkImageUrl = walk.WalkImageUrl;
            walkDomainModel.DifficultyId = walk.DifficultyId;
            walkDomainModel.RegionId = walk.RegionId;

            await context.SaveChangesAsync();
            return walkDomainModel;
            
        }
    }
}
