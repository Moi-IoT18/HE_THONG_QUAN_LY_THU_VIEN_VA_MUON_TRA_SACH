using Microsoft.EntityFrameworkCore;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Remove(T entity);
    Task SaveChangesAsync();
    IQueryable<T> Query();
    IQueryable<T> QueryNoTracking();
}
