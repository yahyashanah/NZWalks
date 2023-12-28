using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository,
            IMapper mapper) 
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {

            // Get Data From Database - Domail models
            var regionsDomain = await regionRepository.GetAllRegionsAsync();

            // Return DTOs
            return Ok(mapper.Map<List<RegionDto>>(regionsDomain));
        }

        // Get Region By Id 
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Get Region Domain Model From Database
            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            //// Map Region Domain Model to Region DTO
            return Ok(mapper.Map<RegionDto>(regionDomain));
        }

        // POST to create New Region
        [HttpPost]
        [ValidateModel] 
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create(AddRegionRequestDto addRegionRequestDto)
        {
                // Map DTO to Domain Model
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

                // Use Domain Model to create Region
                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                // Map Domain model back to DTO
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
           
        }

        // Update region
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id,AddRegionRequestDto updateRegionRequestDto)
        {
                // Map DTO to Domain Model
                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                // Check if region exit
                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);
                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                // Convert Domain Model to DTO
                return Ok(mapper.Map<RegionDto>(regionDomainModel));

        }

        // Delete Region
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var region = await regionRepository.DeleteAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<RegionDto>(region));
        }
    }
}
