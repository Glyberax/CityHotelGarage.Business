
using CityHotelGarage.Business.Operations.DTOs;
using CityHotelGarage.Business.Operations.Results;

namespace CityHotelGarage.Business.Operations.Interfaces;

public interface ICityService
{
    Task<Result<IEnumerable<CityDto>>> GetAllCitiesAsync();
    Task<Result<CityDto>> GetCityByIdAsync(int id);
    Task<Result<CityDto>> CreateCityAsync(CityCreateDto cityDto);
    Task<Result<CityDto>> UpdateCityAsync(int id, CityUpdateDto cityDto);
    Task<Result> DeleteCityAsync(int id);
    
    // 🆕 YENİ - Paging metodu ekleyin
    /// <summary>
    /// Sayfalı şehir listesi getirir (arama, sıralama, cache destekli)
    /// </summary>
    /// <param name="pagingRequest">Sayfalama parametreleri</param>
    /// <returns>Sayfalı şehir listesi</returns>
    Task<Result<PagedResult<CityDto>>> GetPagedCitiesAsync(PagingRequestDto pagingRequest);
}