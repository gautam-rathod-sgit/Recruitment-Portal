﻿using RecruitmentPortal.Core.Entities.Base;
using RecruitmentPortal.Core.Specification.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Repository.Base
{
    public interface IRepository<T> where T : Entity
    {
        //generics for specification
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        string includeString = null,
                                        bool disableTracking = true);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        List<Expression<Func<T, object>>> includes = null,
                                        bool disableTracking = true);
        Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec);

        Task<IReadOnlyList<T>> GetAllAsync();






        Task<IQueryable<T>> getAll();
        Task<T> getById(int id);

        Task<T> Add(T Entity);
        Task Delete(T Entity);
        Task Update(T Entity);
    }
}
