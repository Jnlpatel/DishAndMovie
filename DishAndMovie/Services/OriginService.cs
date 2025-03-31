using DishAndMovie.Data;
using DishAndMovie.Models;
using Microsoft.EntityFrameworkCore;
using DishAndMovie.Interfaces;

namespace DishAndMovie.Services
{
    public class OriginService : IOriginService
    {
        private readonly ApplicationDbContext _context;

        public OriginService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OriginDto>> ListOrigins()
        {
            // Fetch origins from the database
            List<Origin> origins = await _context.Origins.ToListAsync();

            // Convert to OriginDto
            List<OriginDto> originDtos = origins.Select(o => new OriginDto()
            {
                OriginId = o.OriginId,
                OriginCountry = o.OriginCountry
            }).ToList();

            return originDtos;
        }

        public async Task<OriginDto?> FindOrigin(int id)
        {
            // Fetch a single origin by ID
            var origin = await _context.Origins.FindAsync(id);

            // If no origin is found, return null
            if (origin == null)
            {
                return null;
            }

            // Create and return the origin DTO
            OriginDto originDto = new OriginDto()
            {
                OriginId = origin.OriginId,
                OriginCountry = origin.OriginCountry
            };

            return originDto;
        }

        public async Task<ServiceResponse> UpdateOrigin(OriginDto originDto)
        {
            ServiceResponse serviceResponse = new ServiceResponse();

            // Check if the origin exists
            var existingOrigin = await _context.Origins.FindAsync(originDto.OriginId);
            if (existingOrigin == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Origin not found.");
                return serviceResponse;
            }

            // Update origin properties
            existingOrigin.OriginCountry = originDto.OriginCountry;

            // Mark the entity as modified
            _context.Entry(existingOrigin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"An error occurred while updating the origin: {ex.Message}");
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"Unexpected error: {ex.Message}");
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> AddOrigin(OriginDto originDto)
        {
            ServiceResponse response = new ServiceResponse();

            // Create a new Origin entity
            Origin origin = new Origin()
            {
                OriginCountry = originDto.OriginCountry
            };

            // Add the new origin to the database
            _context.Origins.Add(origin);
            await _context.SaveChangesAsync();

            // Return success response
            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = origin.OriginId;
            return response;

        }

        public async Task<ServiceResponse> DeleteOrigin(int id)
        {
            ServiceResponse response = new ServiceResponse();

            // Find the origin by ID
            var origin = await _context.Origins.FindAsync(id);
            if (origin == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Origin not found. Cannot be deleted.");
                return response;
            }

            try
            {
                _context.Origins.Remove(origin);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred while deleting the origin.");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            return response;

        }
    }
}
