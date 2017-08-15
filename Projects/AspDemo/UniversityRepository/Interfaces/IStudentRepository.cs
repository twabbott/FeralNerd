using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityRepository
{
    public interface IStudentRepository: IRepository<Student>
    {
        IEnumerable<Student> GetStudentsBySearch(string searchValue);
    }
}
