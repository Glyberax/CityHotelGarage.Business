
namespace CityHotelGarage.Business.Repository.Interfaces;

public interface IBaseRepository<T> where T : class
{
    IQueryable<T> GetAll();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    
    /// <param name="query">Sayfalanacak IQueryable (filtering, sorting yapılmış olabilir)</param>
    /// <param name="pageNumber">Sayfa numarası (1, 2, 3...)</param>
    /// <param name="pageSize">Sayfa boyutu (10, 20, 50...)</param>
    /// <returns>Sayfalı veriler ve toplam kayıt sayısı</returns>
    Task<(IEnumerable<T> Data, int TotalCount)> GetPagedAsync(IQueryable<T> query, int pageNumber, int pageSize);
}