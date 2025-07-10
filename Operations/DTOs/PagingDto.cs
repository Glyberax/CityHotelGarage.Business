namespace CityHotelGarage.Business.Operations.DTOs;

/// <summary>
/// API'ye gelen sayfalama parametreleri
/// </summary>
public class PagingRequestDto
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Sayfa numarası (minimum 1)
    /// </summary>
    public int PageNumber 
    { 
        get => _pageNumber; 
        set => _pageNumber = value <= 0 ? 1 : value; 
    }

    /// <summary>
    /// Sayfa boyutu (minimum 1, maksimum 100)
    /// </summary>
    public int PageSize 
    { 
        get => _pageSize; 
        set => _pageSize = value <= 0 ? 10 : value > MaxPageSize ? MaxPageSize : value; 
    }
    
    /// <summary>
    /// Arama terimi (şehir adında aranacak)
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Sıralama alanı (name, population, createdDate)
    /// </summary>
    public string? SortBy { get; set; } = "name";
    
    /// <summary>
    /// Azalan sıralama mı? (false = artan, true = azalan)
    /// </summary>
    public bool SortDescending { get; set; } = false;
    
    /// <summary>
    /// Maksimum sayfa boyutu
    /// </summary>
    public const int MaxPageSize = 100;
}

/// <summary>
/// Response'da dönecek sayfalama bilgileri
/// </summary>
public class PagingInfoDto
{
    /// <summary>
    /// Mevcut sayfa numarası
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// Sayfa boyutu (kaç kayıt)
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Toplam kayıt sayısı
    /// </summary>
    public int TotalRecords { get; set; }
    
    /// <summary>
    /// Toplam sayfa sayısı
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Sonraki sayfa var mı?
    /// </summary>
    public bool HasNextPage { get; set; }
    
    /// <summary>
    /// Önceki sayfa var mı?
    /// </summary>
    public bool HasPreviousPage { get; set; }
    
    /// <summary>
    /// İlk kayıt numarası (gösterim için)
    /// </summary>
    public int FirstRecord { get; set; }
    
    /// <summary>
    /// Son kayıt numarası (gösterim için)
    /// </summary>
    public int LastRecord { get; set; }

    /// <summary>
    /// Constructor - Otomatik hesaplama yapar
    /// </summary>
    public PagingInfoDto(int currentPage, int pageSize, int totalRecords)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = totalRecords > 0 ? (int)Math.Ceiling((double)totalRecords / pageSize) : 0;
        HasNextPage = currentPage < TotalPages;
        HasPreviousPage = currentPage > 1;
        
        // Kayıt aralığı hesaplama
        if (totalRecords > 0)
        {
            FirstRecord = ((currentPage - 1) * pageSize) + 1;
            LastRecord = Math.Min(currentPage * pageSize, totalRecords);
        }
        else
        {
            FirstRecord = 0;
            LastRecord = 0;
        }
    }
}

/// <summary>
/// </summary>
/// <typeparam name="T">Data türü (CityDto, HotelDto, vs.)</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Sayfalı veriler
    /// </summary>
    public IEnumerable<T> Data { get; set; } = [];
    
    /// <summary>
    /// Sayfalama meta bilgileri
    /// </summary>
    public PagingInfoDto Pagination { get; set; }
    
    /// <summary>
    /// İşlem mesajı
    /// </summary>
    public string Message { get; set; } = "";
    
    /// <summary>
    /// İşlem başarılı mı?
    /// </summary>
    public bool IsSuccess { get; set; } = true;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public PagedResult(IEnumerable<T> data, int currentPage, int pageSize, int totalRecords, string message = "")
    {
        Data = data;
        Pagination = new PagingInfoDto(currentPage, pageSize, totalRecords);
        Message = string.IsNullOrEmpty(message) 
            ? $"Sayfa {currentPage} - {Data.Count()} kayıt getirildi." 
            : message;
        IsSuccess = true;
    }
    
    /// <summary>
    /// Boş sayfa için constructor
    /// </summary>
    public static PagedResult<T> Empty(int currentPage, int pageSize, string message = "Kayıt bulunamadı.")
    {
        return new PagedResult<T>([], currentPage, pageSize, 0, message);
    }
}