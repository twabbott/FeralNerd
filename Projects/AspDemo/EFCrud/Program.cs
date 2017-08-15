using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using UniversityRepositoryEF;
using UniversityRepository;

namespace EFCrud
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                ctx.Students.Add(new Student
                {
                    FirstName = "Tom",
                    LastName = "Abbott"
                });

                ctx.Commit();
            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                foreach (Student student in ctx.Students.GetAll())
                {
                    Console.WriteLine("{0} {1}", student.FirstName, student.LastName);
                }
            }
        }
    }
}
