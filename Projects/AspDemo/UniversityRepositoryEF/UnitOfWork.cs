using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UniversityRepository;

namespace UniversityRepositoryEF
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly UniversityContext _context;

        public UnitOfWork(UniversityContext context)
        {
            _context = context;

            Students = new StudentRepository(_context);
        }

        public IStudentRepository Students { get; private set; }
        
        public int Commit()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public string ConnectionString
        {
            get
            {
                return _context.Database.Connection.ConnectionString;
            }
        }
    }
}
