using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RESTful.Context;
using RESTful.Entity;
using RESTful.Exceptions;
using RESTful.Service.Interface;

namespace RESTful.Service.Implementation;

public class GenericService<T> : IGenericService<T> where T : class, IEntity
{
    private readonly BackendDbContext _context;
    private readonly ICachingService _cachingService;
    private readonly ILogger<GenericService<T>> _logger;
    private readonly DbSet<T> _dbSet;
    private readonly string _entityName;
    
    public GenericService(BackendDbContext context, ICachingService cachingService, ILogger<GenericService<T>> logger)
    {
        _context = context;
        _cachingService = cachingService;
        _logger = logger;
        _dbSet = _context.Set<T>();
        _entityName = typeof(T).Name;
    }

    public async Task<List<T>> GetAllAsync()
    {
        var key = $"{_entityName}_All";

        var cached = await _cachingService.GetAsync<List<T>>(key);
        if (cached != null)
            return cached;
        
        _logger.LogInformation("[DB QUERY] All {EntityName}", _entityName);
        
        var entities = await _dbSet.AsNoTracking().ToListAsync();

        if (!entities.Any())
        {
            throw new NotFoundException($"No {_entityName} found.");
        }

        // Apply caching
        await _cachingService.SetAsync(key, entities);

        return entities;
    }
    
    public async Task<T?> GetByIdAsync(int id)
    {
        var key = $"{_entityName}_{id}";

        var cached = await _cachingService.GetAsync<T>(key);
        if (cached != null)
            return cached;

        _logger.LogInformation("[DB QUERY] {EntityName} {Id}", _entityName, id);
        var entity = await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
            throw new NotFoundException($"{_entityName} with ID {id} not found.");

        // Apply caching
        await _cachingService.SetAsync(key, entity);
        
        return entity;
    }
    
    public async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();

        await _cachingService.SetAsync($"{_entityName}_{entity.Id}", entity);
        await _cachingService.RemoveAsync($"{_entityName}_All");

        return entity;
    }
    
    public async Task<T?> UpdateAsync(int id, T updatedEntity)
    {
        var existing = await _dbSet.FindAsync(id);
        if (existing == null)
            throw new NotFoundException($"{_entityName} with ID {id} not found.");

        _context.Entry(existing).CurrentValues.SetValues(updatedEntity); // Replace with mapper
        await _context.SaveChangesAsync();

        var refreshed = await _dbSet.AsNoTracking().FirstAsync(e => e.Id == id);

        await _cachingService.SetAsync($"{_entityName}_{id}", refreshed);
        await _cachingService.RemoveAsync($"{_entityName}_All");

        return refreshed;
    }
    
    public async Task<T?> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
            throw new NotFoundException($"{_entityName} with ID {id} not found.");

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();

        await _cachingService.RemoveAsync($"{_entityName}_{id}");
        await _cachingService.RemoveAsync($"{_entityName}_All");

        return entity;
    }
}