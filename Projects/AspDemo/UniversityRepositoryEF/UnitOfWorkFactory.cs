using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using UniversityRepository;


namespace UniversityRepositoryEF
{
    public class UnitOfWorkFactory
    {
        public static IUnitOfWork Initialize()
        {
            return new UnitOfWork(new UniversityContext("UniversityDatabase"));
        }

        public static IUnitOfWork Initialize(string connectionStringName)
        {
            return new UnitOfWork(new UniversityContext(connectionStringName));
        }
    }
}
