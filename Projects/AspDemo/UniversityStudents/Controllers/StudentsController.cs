using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using UniversityRepository;
using UniversityRepositoryEF;

namespace UniversityStudents.Controllers
{
    public class StudentsController : ApiController
    {
        private int _sleepTimeout = 300;

        // GET: api/Students
        [HttpGet]
        public IHttpActionResult Get()
        {
            Thread.Sleep(_sleepTimeout);

            List<Student> students = null;
            try
            {
                using (IUnitOfWork db = UnitOfWorkFactory.Initialize())
                {
                    // TODO: Add additional query string parameters for pagination, etc.
                    students = db.Students.GetAll().ToList();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }

            return Ok(students);
        }

        // GET: api/Students?search=xxx
        [HttpGet]
        public IHttpActionResult Get(string search)
        {
            Thread.Sleep(_sleepTimeout);

            List<Student> students = new List<Student>();
            try
            {
                using (IUnitOfWork db = UnitOfWorkFactory.Initialize())
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        students = db.Students.Find(s => s.FirstName.ToLower().Contains(search) || s.LastName.ToLower().Contains(search)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }

            return Ok(students);
        }

        // GET: api/Students/5
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            Thread.Sleep(_sleepTimeout);

            Student student = null;
            try
            {
                using (IUnitOfWork db = UnitOfWorkFactory.Initialize())
                {
                    // TODO: Add additional query string parameters for pagination, etc.
                    student = db.Students.Get(id);
                }

            }
            catch (Exception ex)
            {
                return InternalServerError();
            }

            if (student == null)
                return NotFound();

            return Ok(student);
        }


        // POST: api/Students
        [HttpPost]
        public IHttpActionResult Post([FromBody]Student student)
        {
            Thread.Sleep(_sleepTimeout);

            try
            {
                if (student == null)
                    return BadRequest();

                string error;
                if (!__ValidateStudent(student, out error))
                    return BadRequest(error);

                using (IUnitOfWork db = UnitOfWorkFactory.Initialize())
                {
                    student.StudentId = -1;

                    db.Students.Add(student);
                    db.Commit();
                }

                if (student.StudentId < 0)
                    return InternalServerError();

                return Created<Student>(Request.RequestUri + "/" + student.StudentId, student);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        // PUT: api/Students/5
        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody]Student student)
        {
            Thread.Sleep(_sleepTimeout);

            try
            {
                if (student == null || id <= 0)
                    return BadRequest();

                // Validate EVERYTHING before it goes into the DB.
                string error;
                if (!__ValidateStudent(student, out error))
                    return BadRequest(error);

                using (IUnitOfWork db = UnitOfWorkFactory.Initialize())
                {
                    // Make there the ID is good before writing to the DB.
                    Student copy = db.Students.Get(id);
                    if (copy == null)
                        return NotFound();

                    copy.FirstName = student.FirstName;
                    copy.LastName = student.LastName;
                    copy.Major = student.Major;
                    copy.Gender = student.Gender;
                    copy.Gpa = student.Gpa;

                    db.Commit();
                }

            }
            catch (Exception ex)
            {
                return InternalServerError();
            }

            return Ok();
        }

        // DELETE: api/Students/5
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            Thread.Sleep(_sleepTimeout);

            try
            {
                if (id <= 0)
                    return BadRequest();

                using (IUnitOfWork db = UnitOfWorkFactory.Initialize())
                {
                    Student student = db.Students.Remove(id);
                    db.Commit();
                    if (student == null)
                        return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool __ValidateStudent(Student student, out string error)
        {
            error = null;
            if (string.IsNullOrEmpty(student.FirstName))
            {
                error = "First name cannot be blank.";
                return false;
            }

            if (string.IsNullOrEmpty(student.LastName))
            {
                error = "Last name cannot be blank.";
                return false;
            }

            if (string.IsNullOrEmpty(student.Major))
                student.Major = "Undeclared";

            if (!Enum.IsDefined(typeof(Gender), student.Gender))
            {
                error = "Gender must be either Male or Female";
                return false;
            }

            if (student.Gpa< 0 || student.Gpa > 4.0)
            {
                error = "GPA range must be between 0.0 and 4.0";
                return false;
            }

            return true;
        }
    }
}
