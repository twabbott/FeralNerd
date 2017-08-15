using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityRepository
{
    public enum Gender
    {
        Male,
        Female
    }

    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Major { get; set; }
        public double Gpa { get; set; }

        public Gender Gender { get; set; }
    }
}
