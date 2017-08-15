using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UniversityRepository;

namespace UniversityRepositoryEF
{
    class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(DbContext context):
            base(context)
        {
        }


        public IEnumerable<Student> GetStudentsBySearch(string searchValue)
        {
            throw new NotImplementedException();
        }
    }
}
