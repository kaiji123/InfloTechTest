
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Data;


public interface IDataContext
    {
        /// <summary>
        /// Get a list of items
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;

        /// <summary>
        /// Create a new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateAsync<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Update an existing item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Delete an item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
}

