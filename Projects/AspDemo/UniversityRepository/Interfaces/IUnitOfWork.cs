using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityRepository
{
    /// <summary>
    ///     Interface for our unit of work.
    /// </summary>
    public interface IUnitOfWork: IDisposable
    {
        ////////////////////////////////////////////////////
        /// <summary>
        ///     The Student repository
        /// </summary>
        IStudentRepository Students { get; }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Commmits all changes to the data store.
        /// </summary>
        /// <returns></returns>
        int Commit();


        /// <summary>
        ///     Gets the connection string used by this unit of work.  Used
        ///     to clean up the DB during unit testing.
        /// </summary>
        string ConnectionString { get; }
    }
}
