using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityRepository;

namespace UniversityRepositoryEF
{
    class UniversityContext : DbContext
    {
        public UniversityContext(string connectionStringName) :
            base(connectionStringName)
        { }

        public DbSet<Student> Students { get; set; }
    }
}
