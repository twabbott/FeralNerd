using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCrud
{
    public class StudentRepository
    {
        public List<Student> GetStudents()
        {
            StudentDBContext ctx = new StudentDBContext();
            return ctx.Students.ToList();
        }
    }
}
