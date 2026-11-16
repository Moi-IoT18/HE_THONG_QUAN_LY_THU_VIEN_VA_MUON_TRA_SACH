// Họ và tên: Nguyễn Thế Đông
// Mã sinh viên: 23103100187
// Nội dung thực hiện: Repository – lớp trừu tượng hóa thao tác dữ liệu .
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;

public interface IRepository<T> where T : class
{
    IQueryable<T> QueryNoTracking();
    IQueryable<T> Query();
    Task<T?> GetByIdAsync(object id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    public IQueryable<T> QueryNoTracking() => _dbSet.AsNoTracking();
    public IQueryable<T> Query() => _dbSet;
    public async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
    public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public Task AddAsync(T entity)
    {
        _dbSet.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
