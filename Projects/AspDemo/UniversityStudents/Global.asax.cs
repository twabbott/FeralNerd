using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UniversityRepository;
using UniversityRepositoryEF;

namespace UniversityStudents
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            using (IUnitOfWork db = UnitOfWorkFactory.Initialize())
            {
                IList<Student> list = db.Students.GetAll();
                if (list.Count < 5)
                {
                    foreach (Student s in list)
                        db.Students.Remove(s.StudentId);
                    db.Commit();

                    db.Students.Add(new Student
                    {
                        FirstName = "John",
                        LastName = "Williams",
                        Gender = Gender.Male,
                        Major = "Theater",
                        Gpa = 3.8
                    });

                    db.Students.Add(new Student
                    {
                        FirstName = "Phil",
                        LastName = "Johnson",
                        Gender = Gender.Male,
                        Major = "Economics",
                        Gpa = 3.2
                    });

                    db.Students.Add(new Student
                    {
                        FirstName = "Will",
                        LastName = "Carlson",
                        Gender = Gender.Male,
                        Major = "Chinese",
                        Gpa = 3.4
                    });

                    db.Students.Add(new Student
                    {
                        FirstName = "Carla",
                        LastName = "Wilson",
                        Gender = Gender.Female,
                        Major = "Music",
                        Gpa = 3.7
                    });

                    db.Students.Add(new Student
                    {
                        FirstName = "Jessica",
                        LastName = "Phillips",
                        Gender = Gender.Female,
                        Major = "Social Work",
                        Gpa = 3.2
                    });

                    db.Commit();
                }
            }
        }
    }
}
