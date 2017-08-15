using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityRepository
{
    ////////////////////////////////////////////////////
    /// <summary>
    ///     Represents a generic repository
    /// </summary>
    /// 
    /// https://www.youtube.com/watch?v=rtXpYpZdOzM
    /// 
    /// <typeparam name="TEntity">
    ///     Data type you want your repository to work with.
    /// </typeparam>
    public interface IRepository<TEntity> where TEntity: class
    {
        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets a single record from the repository by its id.
        /// </summary>
        /// 
        /// <param name="id">
        ///     The id of the item you want to get.
        /// </param>
        /// 
        /// <returns>
        ///     Returns the item you wanted to fetch, or null.
        /// </returns>
        TEntity Get(int id);


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets everything in the collection.
        /// </summary>
        /// 
        /// <returns>
        ///     Everything in the collection.
        /// </returns>
        IList<TEntity> GetAll();


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets a specific set of items based on search criteria.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A lamda expression for the item you want to find.
        /// </param>
        /// 
        /// <returns>
        ///     The items that you searched for, or an empty collection.
        /// </returns>
        IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate);


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Adds an item to the repository.
        /// </summary>
        /// 
        /// <param name="entity">
        ///     The item you want to add.
        /// </param>
        void Add(TEntity entity);


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Removes an item from the repository.
        /// </summary>
        /// 
        /// <param name="entity">
        ///     The matching item you want removed.
        /// </param>
        /// 
        /// <returns>
        ///     Returns true if the student existed, false otherwise.
        /// </returns>
        TEntity Remove(int id);
    }
}
