using Microsoft.EntityFrameworkCore;
using QuanLyThuVien_UNETI01_DHTI17A4HN.Data;

namespace QuanLyThuVien_UNETI01_DHTI17A4HN.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext db)
    {
        _db = db;
        _dbSet = db.Set<T>();
    }

    public Task<T?> GetByIdAsync(int id) => _dbSet.FindAsync(id).AsTask();

    public Task AddAsync(T entity) => _dbSet.AddAsync(entity).AsTask();

    public void Remove(T entity) => _dbSet.Remove(entity);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();

    public IQueryable<T> Query() => _dbSet.AsQueryable();
}
