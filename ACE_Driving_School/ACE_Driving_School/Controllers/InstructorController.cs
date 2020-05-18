using ACE_Driving_School.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACE_Driving_School.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private ACE_Driving_School_Db_Context context = new ACE_Driving_School_Db_Context();
        public ActionResult InstructorHome()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            return View();
        }
        public ActionResult InstructorAccount()
        {
            string InstructorId = User.Identity.GetUserId();
            Instructor instructor = (Instructor)context.Users.Find(InstructorId);

            return View(instructor);
        }
        [HttpGet]
        public ActionResult EditDetails()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            string InstructorId = User.Identity.GetUserId();
            Instructor instructor = (Instructor)context.Users.Find(InstructorId);
            return View(instructor);
        }
        
        [HttpPost]
        public ActionResult EditDetails(Instructor model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Instructor CurrentDbInstructor = (Instructor)context.Users.Find(model.Id);

            CurrentDbInstructor.Bio = model.Bio;
            CurrentDbInstructor.Experience_Years = model.Experience_Years;
            CurrentDbInstructor.PhoneNumber = model.PhoneNumber;
            CurrentDbInstructor.AddressLine1 = model.AddressLine1;
            CurrentDbInstructor.AddressLine2 = model.AddressLine2;
            CurrentDbInstructor.City = model.City;
            CurrentDbInstructor.Postcode = model.Postcode;
            CurrentDbInstructor.Email = model.Email;

            context.SaveChanges();

            return RedirectToAction("InstructorAccount");
        }

        public ActionResult GeneratePDFreports()
        {
            //first will need to see which report the user wants to print
            //then work out the logic for that reports
            //then make the pdf 
            //try using this video as a refernce https://www.youtube.com/watch?v=vXxNDZpCIkQ

            return View();
        }
        public ActionResult MarkLessonAsComplete(int Lesson_Id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            Lesson lesson = context.Lessons.Find(Lesson_Id);
            lesson.Status = "Complete";

            context.SaveChanges();
            return RedirectToAction("LessonDetails", "Lesson", new { Lesson_Id = Lesson_Id});
        }
        public ActionResult TodaysLessons()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            if (User.IsInRole("Student"))
                return RedirectToAction("Index", "Home", new { Error_Message = "Can't access that page"});

            string Instructor_Id = User.Identity.GetUserId();
            Instructor instructor = (Instructor)context.Users.Find(Instructor_Id);

            
            
            List<Lesson> Instructors_Lessons = context.Lessons.Where(p => p.Instructor_Id == instructor.Id)
                                                         .Include(p => p.Student)
                                                         .Include(p => p.Booking)
                                                         .Include(p => p.Car)
                                                         .ToList();

            List<Lesson> Todays_Lessons = new List<Lesson>();
            foreach (var item in Instructors_Lessons.ToList())
            {
                DateTime LessonDate = item.Date_And_Time.Date;
                if (LessonDate == DateTime.Now.Date)
                {
                    Todays_Lessons.Add(item);
                }
            }
            //get rid of this list
            Instructors_Lessons.Clear();

            return View(Todays_Lessons);
        }
        public ActionResult MarkStudentsAsPassed()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            //Get all the students with the same recent isntructor ID as this instructor and display them
            string Instructor_Id = User.Identity.GetUserId();
            List<Student> Instructors_Students = context.Users.OfType<Student>()
                                                              .Where(p => p.Most_Recent_Instructor_Id == Instructor_Id)
                                                              .Where(p => p.TestDate.HasValue)
                                                              .Where(p => p.hasPassed == false)
                                                              .ToList();
            return View(Instructors_Students);
        }


        public ActionResult MarkStudentAsPassed(string Student_Id)
        {
            Student student = (Student)context.Users.Find(Student_Id);
            student.hasPassed = true;
            student.PassedDate = DateTime.Now.Date;
            context.SaveChanges();
            return RedirectToAction("MarkStudentsAsPassed");
        }
        public List<DateTime> GetInstructorAvailability(DateTime date)
        {
            return new List<DateTime>() { date};
        }

        public void GetCurrentTraffic()
        {
            //get the bishopbrigs current map
            //and display it on the view lesson details page
        }
        
    }
}