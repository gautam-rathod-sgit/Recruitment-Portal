using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Core.Entities.Base;
using RecruitmentPortal.Core.Repository.Base;
using RecruitmentPortal.Core.Specification.Base;
using RecruitmentPortal.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Infrastructure.Repository.Base
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        //creating DbContext class instance 
        RecruitmentPortalDbContext _DbContext;

        //Creating constructor
        public Repository(RecruitmentPortalDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        //adding the generics
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _DbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetallAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }


        //used this method for returning IQeurable
        public IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_DbContext.Set<T>().AsQueryable(), spec);
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _DbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        {
            IQueryable<T> query = _DbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _DbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }







        public async Task<T> Add(T Entity)
        {
            _DbContext.Set<T>().Add(Entity);
            await _DbContext.SaveChangesAsync();
            return Entity;

        }

        public async Task Delete(T Entity)
        {
            _DbContext.Set<T>().Remove(Entity);
            await _DbContext.SaveChangesAsync();
        }

        public async Task<IQueryable<T>> getAll()
        {
            return _DbContext.Set<T>();
        }

        public async Task<T> getById(int id)
        {
            return await _DbContext.Set<T>().FindAsync(id);
        }

        public async Task Update(T Entity)
        {
           
                _DbContext.Set<T>().Update(Entity);
                await _DbContext.SaveChangesAsync();
          
        }
    }
}
