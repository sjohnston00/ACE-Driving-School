using ACE_Driving_School.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
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

        public ActionResult ViewUpcomingLesson()
        {

            return View();
        }

        public List<DateTime> GetInstructorAvailability(DateTime date)
        {
            return new List<DateTime>() { date};
        }

        public void GetCurrentTraffic()
        {

        }
        
    }
}