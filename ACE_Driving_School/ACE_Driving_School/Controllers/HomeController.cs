using ACE_Driving_School.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ACE_Driving_School.Controllers
{
    public class HomeController : Controller
    {
        private ACE_Driving_School_Db_Context context = new ACE_Driving_School_Db_Context();

        /// <summary>
        /// Main Home page, display the 6 most recent passed students
        /// </summary>
        /// <returns>List of Students to the view</returns>
        public ActionResult Index(string Error_Message)
        {
            if (!Error_Message.IsNullOrWhiteSpace())
                ViewBag.ErrorMessage = Error_Message;

            List<Student> Most_Recent_Passed_Students = Get_Most_Recent_Passed_Students();
            
            return View(Most_Recent_Passed_Students);
        }

        /// <summary>
        /// About page, display information about the company
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            

            return View();
        }
        /// <summary>
        /// Contact page, display information on how to find the school and contact information
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// Page where all the instructors are visible and the user can view each one
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAllInstructors()
        {
            var instructors = context.Users.Include(p => p.Roles).OrderBy(p => p.UserName).ToList(); 
            return View(instructors);
        }

        /// <summary>
        /// Page where all the information about the instructor is visible 
        /// </summary>
        /// <param name="Instructor_Username">The username of the instructor so they can be found in the database</param>
        /// <returns>Instructor object to the view</returns>
        public ActionResult InstructorDescription(string Instructor_Username)
        {
            if (Instructor_Username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find the instructor and build and object
            Instructor instructor = (Instructor)context.Users.Where(p => p.UserName == Instructor_Username).ToList().First();
            instructor.Current_Students = Get_Instructor_Most_Recent_Passed_Students(instructor);

            if (instructor == null)
            {
                return HttpNotFound();
            }

            return View(instructor);
        }
        /// <summary>
        /// Function that gets the most recent passed students in the database
        /// </summary>
        /// <returns>A list of all passed students</returns>
        public List<Student> Get_Most_Recent_Passed_Students()
        {
            //get the most recent students from the database that have passed
            List<Student> Most_Recent_Passed_Students = context.Users.OfType<Student>()
                                                        .Where(p => p.hasPassed)
                                                        .OrderBy(p => p.PassedDate).ToList();
            return (Most_Recent_Passed_Students);
        }
        /// <summary>
        /// Function that gets all the most recent passed student of one instructor
        /// </summary>
        /// <param name="instructor">Instructor object, so we can use the ID to match</param>
        /// <returns>A list of student for the passed in instructor</returns>
        public List<Student> Get_Instructor_Most_Recent_Passed_Students(Instructor instructor)
        {
            //---THIS GETS ALL THE STUDENT THAT HAVE PASSED FROM THAT INSTRUCTOR
            List<Student> Instructors_Students = context.Users.OfType<Student>()
                                                  .Where(p => p.Most_Recent_Instructor_Id == instructor.Id)
                                                  .Where(p => p.hasPassed)
                                                  .OrderBy(p => p.PassedDate)
                                                  .ToList();


            //NOW WE GET THE FIRST 6 STUDENTS
            //List<Student> Instructor_First_6_Students = (List<Student>)Instructors_Students.Take(6);
            //this also doesnt throw an expection when there arent enough students in the list
            return Instructors_Students;
            
        }
    }
}