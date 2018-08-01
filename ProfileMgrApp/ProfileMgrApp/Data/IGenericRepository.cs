using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileMgrApp.Data
{
    /// <summary>
    /// Generic Repository Interface for CRUD operations, it provides abstraction for Data Access Layer 
    /// </summary>
    public interface IGenericRepository<TEntity, TKey> where TEntity : class  
    {
        void Create(TEntity entity);
        Task CreateAsync(TEntity entity);

        TEntity Read(TKey id);
        Task<TEntity> ReadAsync(TKey id);

        void Update(TKey key, TEntity entity);
        Task UpdateAsync(TKey key, TEntity entity);

        void Update(TEntity newEntity, TEntity oldEntity);
        Task UpdateAsync(TEntity newEntity, TEntity oldEntity);

        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);

        ICollection<TEntity> List();
        Task<ICollection<TEntity>> ListAsync();
    }

}
