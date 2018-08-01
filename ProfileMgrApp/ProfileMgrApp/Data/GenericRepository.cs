using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileMgrApp.Data
{
    /// <summary>
    ///  An abstract Generic Repository class to provide the base implementation of IGenericRepository interface
    /// </summary>
    public abstract class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
    {
        private DbContext _context;

        public GenericRepository(DbContext context)
        {
            _context = context;
        }

        public virtual void Create(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual TEntity Read(TKey key)
        {
            return _context.Set<TEntity>().Find(key);
        }

        public virtual async Task<TEntity> ReadAsync(TKey key)
        {
            return await _context.Set<TEntity>().FindAsync(key);
        }

        public virtual void Update(TKey key, TEntity entity)
        {
            TEntity exist = _context.Set<TEntity>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
        }

        public virtual async Task UpdateAsync(TKey key, TEntity entity)
        {
            TEntity exist = _context.Set<TEntity>().Find(key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual void Update(TEntity newEntity, TEntity oldEntity)
        {
            _context.Entry(oldEntity).CurrentValues.SetValues(newEntity);
            _context.SaveChanges();
        }

        public virtual async Task UpdateAsync(TEntity newEntity, TEntity oldEntity)
        {
            _context.Entry(oldEntity).CurrentValues.SetValues(newEntity);
            await _context.SaveChangesAsync();
        }

        public virtual void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual ICollection<TEntity> List()
        {
            return _context.Set<TEntity>().ToList();
        }

        public virtual async Task<ICollection<TEntity>> ListAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }
    }
}
