using DishAndMovie.Models;

namespace DishAndMovie.Interfaces
{
    public interface IOriginService
    {
        Task<IEnumerable<OriginDto>> ListOrigins(int skip, int perpage);
        Task<OriginDto?> FindOrigin(int id);
        Task<ServiceResponse> UpdateOrigin(OriginDto originDto);
        Task<ServiceResponse> AddOrigin(OriginDto originDto);
        Task<ServiceResponse> DeleteOrigin(int id);
        Task<int> CountOrigins();
    }
}
