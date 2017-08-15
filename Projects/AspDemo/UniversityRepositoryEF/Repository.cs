using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;

using UniversityRepository;

namespace UniversityRepositoryEF
{
    /// <summary>
    ///     Provides a base implementation for a generic EF repository.  This
    ///     class can be extended to add new methods for specific kinds of
    ///     searches.
    /// </summary>
    /// 
    /// <typeparam name="TEntity">
    ///     Data object type that this repository will work with.
    /// </typeparam>
    class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Constructor
        /// </summary>
        /// 
        /// <param name="context">
        ///     The EF DbContext you want to use.
        /// </param>
        public Repository(DbContext context)
        {
            Context = context;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Adds a record.
        /// </summary>
        /// 
        /// <param name="entity">
        ///     Data object containing the new record.
        /// </param>
        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Returns a list of matching records based on search criteria.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A lamda expression for the search criteria.
        /// </param>
        /// 
        /// <returns>
        ///     A list of matching items, or an empty list if the search did not
        ///     match any items.
        /// </returns>
        public IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).ToList();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets a single record by its id.  Returns null if there is no 
        ///     such record.
        /// </summary>
        /// 
        /// <param name="id">
        ///     Id of the record to retrieve.
        /// </param>
        /// 
        /// <returns>
        ///     The record to retrieve, or a null reference.
        /// </returns>
        public TEntity Get(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Returns all records in the repository
        /// </summary>
        /// 
        /// <returns>
        ///     A list of all the records.
        /// </returns>
        public IList<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Removes an item from the repository.
        /// </summary>
        /// 
        /// <param name="entity">
        ///     Matching record that you want removed from the repository.
        /// </param>
        public TEntity Remove(int id)
        {
            TEntity item = Get(id);
            if (item == null)
                return null;

            TEntity x = Context.Set<TEntity>().Remove(item);
            return x;
        }
    }
}
