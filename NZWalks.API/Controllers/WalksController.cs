using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // /api/walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }
        // Create Walk
        // POST: /api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto) 
        {
            
              // Map Dto To Domain Moddel
              var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

              await walkRepository.CreateWalkAsync(walkDomainModel);

              // Map Domain model to DTO

             return Ok(mapper.Map<WalkDto>(walkDomainModel));
            
        }


        // GET Walks
        // GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var walksDomainModel = await walkRepository.GetAllAsync(filterOn ,filterQuery ,sortBy ,isAscending ?? true ,pageNumber ,pageSize);

            // Map Domain Model to DTO
            return Ok(mapper.Map<List<WalkDto>>(walksDomainModel));
        }

        // GET Walks By id
        // Get: /api/walks/id
        [HttpGet]
        [Route("id")]
        public async Task<IActionResult> GetWalkById(Guid id)
        {
            var walksDomainModel = await walkRepository.GetWalkAsync(id);

            if(walksDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain Model to Dto
            return Ok(mapper.Map<WalkDto>(walksDomainModel));
        }

        // Update Walk
        [HttpPut]
        [Route("id")]
        [ValidateModel]
        public async Task<IActionResult> updateWalk(Guid id,UpdateWalkRequestDto updateWalkRequestDto)
        {

                // Map Dto to Domain Model
                var wlakDomainModel = mapper.Map<Walk>(updateWalkRequestDto);

                wlakDomainModel = await walkRepository.UpdateAsync(id, wlakDomainModel);

                if (wlakDomainModel == null)
                {
                    return NotFound();
                }

                // Map Domain Model to Dto
                return Ok(mapper.Map<WalkDto>(wlakDomainModel));

        }

        // Remove Walk By Id
        [HttpDelete]
        [Route("id")]
        public async Task<IActionResult> DeleteWalk(Guid id)
        {
            var walkDomainModel = await walkRepository.DeleteWalkAsync(id);

            if(walkDomainModel == null)
            {
                return NotFound();
            }

            // Map Domain Model To Dto
            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }


    }
}
