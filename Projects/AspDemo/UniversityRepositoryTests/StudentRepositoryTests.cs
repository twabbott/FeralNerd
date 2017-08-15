using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UniversityRepository;
using UniversityRepositoryEF;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;

namespace UniversityRepositoryTests
{
    [TestClass]
    public class StudentRepositoryTests
    {
        private string _connectionstring = null;

        [TestMethod]
        public void AddAndFetchOneStudent()
        {
            int id;
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                _connectionstring = ctx.ConnectionString;
                id = __CreateOneTestStudent(ctx, "Tom", "Abbott", Gender.Male, "Computer Science", 3.5);
            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                Student student = ctx.Students.Get(id);

                __ValidateStudent(student, "Tom", "Abbott", Gender.Male, "Computer Science", 3.5);
                Assert.IsTrue(student.StudentId != -1, "StudentId should not be -1 after commit.");
            }
        }

        [TestMethod]
        public void AddAndFetchThreeStudents()
        {
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                _connectionstring = ctx.ConnectionString;
                __CreateOneTestStudent(ctx, "Tom", "Abbott", Gender.Male, "Computer Science", 3.5);
                __CreateOneTestStudent(ctx, "Jenna", "Johnson", Gender.Female, "Chemistry", 3.8);
                __CreateOneTestStudent(ctx, "Pete", "Patrick", Gender.Male, "English", 3.0);
            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                var list = ctx.Students.GetAll();
                Assert.IsTrue(list.Count == 3, "There should be three students in the table");

                __ValidateStudent(list[0], "Tom", "Abbott", Gender.Male, "Computer Science", 3.5);
                __ValidateStudent(list[1], "Jenna", "Johnson", Gender.Female, "Chemistry", 3.8);
                __ValidateStudent(list[2], "Pete", "Patrick", Gender.Male, "English", 3.0);
            }
        }

        [TestMethod]
        public void DeleteStudent()
        {
            // Create a new student
            int id;
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                _connectionstring = ctx.ConnectionString;
                id = __CreateOneTestStudent(ctx, "Tom", "Abbott", Gender.Male, "Computer Science", 3.5);
            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                Student student = ctx.Students.Remove(id);
                Assert.IsTrue(student != null, "Should return matching student after removing.");
                ctx.Commit();
            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                IList<Student> list = ctx.Students.GetAll();
                Assert.IsTrue(list.Count == 0, "The count should be 0");
            }
        }

        [TestMethod]
        public void DeleteStudentThatDoesNotExist()
        {
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                _connectionstring = ctx.ConnectionString;
                Student student = ctx.Students.Remove(1234);
                Assert.IsNull(student, "Should return null if no student matches the ID.");
            }
        }

        [TestMethod]
        public void FetchAStudentThatDoesNotExist()
        {
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                _connectionstring = ctx.ConnectionString;
                Student student = ctx.Students.Get(123456);

                Assert.IsNull(student, "Get() should return null for bad id.");
            }
        }

        [TestMethod]
        public void UpdateAStudent()
        {
            int id;
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                _connectionstring = ctx.ConnectionString;
                id = __CreateOneTestStudent(ctx, "Tom", "Abbott", Gender.Male, "Computer Science", 3.5);
            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                Student student = ctx.Students.Get(id);
                student.Gpa = 4;
                student.Major = "English";

                ctx.Commit();
            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                Student student = ctx.Students.Get(id);

                __ValidateStudent(student, "Tom", "Abbott", Gender.Male, "English", 4);
            }
        }

        [TestMethod]
        public void SearchForStudents_FoundTwo()
        {
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                _connectionstring = ctx.ConnectionString;
                __CreateOneTestStudent(ctx, "Thomas", "Abbott", Gender.Male, "Computer Science", 3.5);
                __CreateOneTestStudent(ctx, "Jenna", "Johnson", Gender.Female, "Chemistry", 3.8);
                __CreateOneTestStudent(ctx, "Pete", "Patrick", Gender.Male, "English", 3.0);
                __CreateOneTestStudent(ctx, "Mary", "Thomson", Gender.Male, "General Studies", 3.2);

            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                string searchValue = "thom";
                var list = ctx.Students.Find(student => student.FirstName.ToLower().Contains(searchValue) || student.LastName.ToLower().Contains(searchValue));

                Assert.IsTrue(list.Count == 2, "Two records should match");

                __ValidateStudent(list[0], "Thomas", "Abbott", Gender.Male, "Computer Science", 3.5);
                __ValidateStudent(list[1], "Mary", "Thomson", Gender.Male, "General Studies", 3.2);
            }
        }

        [TestMethod]
        public void SearchForStudents_FoundNone()
        {
            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                _connectionstring = ctx.ConnectionString;
                __CreateOneTestStudent(ctx, "Thomas", "Abbott", Gender.Male, "Computer Science", 3.5);
                __CreateOneTestStudent(ctx, "Jenna", "Johnson", Gender.Female, "Chemistry", 3.8);
                __CreateOneTestStudent(ctx, "Pete", "Patrick", Gender.Male, "English", 3.0);
                __CreateOneTestStudent(ctx, "Mary", "Thomson", Gender.Male, "General Studies", 3.2);

            }

            using (IUnitOfWork ctx = UnitOfWorkFactory.Initialize())
            {
                string searchValue = "zed";
                var list = ctx.Students.Find(student => student.FirstName.ToLower().Contains(searchValue) || student.LastName.ToLower().Contains(searchValue));

                Assert.IsTrue(list.Count == 0, "Zero records should match");
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = _connectionstring;
                conn.Open();

                string text = "DELETE FROM Students";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    int count = cmd.ExecuteNonQuery();
                }
            }
        }

        private int __CreateOneTestStudent(IUnitOfWork ctx, string firstName, string lastName, Gender gender, string major, double gpa)
        {
            Student newStudent = new Student
            {
                StudentId = -1,
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                Major = major,
                Gpa = gpa
            };

            ctx.Students.Add(newStudent);
            ctx.Commit();

            return newStudent.StudentId;
        }

        private void __ValidateStudent(Student student, string firstName, string lastName, Gender gender, string major, double gpa)
        {
            Assert.IsTrue(student.FirstName == firstName, "First name shoudl be \"{0}\"", firstName);
            Assert.IsTrue(student.LastName == lastName, "Last name shoudl be \"{0}\"", lastName);
            Assert.IsTrue(student.Gender == gender, "Gender should be {0}.", gender);
            Assert.IsTrue(student.Gpa == gpa, "GPA should be {0}", gpa);
        }
    }
}
