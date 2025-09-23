using RESTful.Entity;
using RESTful.Exceptions;

namespace RESTful.Service.Interface;

public interface IGenericService<T> where T : class, IEntity
{
    /// <summary>
    /// Retrieves all entities from the database
    /// </summary>
    /// <returns>List of all entities</returns>
    /// <exception cref="NotFoundException">Thrown when no entities are found</exception>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Retrieves a single entity by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the entity</param>
    /// <returns>The entity if found, null otherwise</returns>
    /// <exception cref="NotFoundException">Thrown when entity with specified ID is not found</exception>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new entity in the database
    /// </summary>
    /// <param name="entity">The entity to create</param>
    /// <returns>The created entity with generated ID</returns>
    /// <exception cref="ValidationException">Thrown when entity data is invalid</exception>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Updates an existing entity in the database
    /// </summary>
    /// <param name="id">The ID of the entity to update</param>
    /// <param name="entity">The updated entity data</param>
    /// <returns>The updated entity if successful, null if not found</returns>
    /// <exception cref="NotFoundException">Thrown when entity with specified ID is not found</exception>
    /// <exception cref="ValidationException">Thrown when entity data is invalid</exception>
    Task<T?> UpdateAsync(int id, T entity);

    /// <summary>
    /// Deletes an entity from the database
    /// </summary>
    /// <param name="id">The ID of the entity to delete</param>
    /// <returns>The deleted entity if successful, null if not found</returns>
    /// <exception cref="NotFoundException">Thrown when entity with specified ID is not found</exception>
    Task<T?> DeleteAsync(int id);
}