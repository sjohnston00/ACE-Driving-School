using ACE_Driving_School.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACE_Driving_School.Controllers
{
    public class PaymentController : Controller
    {
        private ACE_Driving_School_Db_Context context = new ACE_Driving_School_Db_Context();
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PayForBooking(int Booking_Id)
        {
            string Student_Id = User.Identity.GetUserId();
            if (Student_Id == null || Student_Id == "")
            {
                return RedirectToAction("Login", "Account");
            }
            Student student = (Student)context.Users.Find(Student_Id);

            Booking booking = context.Bookings.Where(p => p.Booking_Id == Booking_Id).FirstOrDefault();
            booking.Student = student;

            booking.Lessons = context.Lessons.Where(p => p.Booking_Id == Booking_Id)
                                                                 .Include(p => p.Car)
                                                                 .Include(p => p.Student)
                                                                 .Include(p => p.Instructor)
                                                                 .Include(p => p.Booking)
                                                                 .ToList();

            if (booking.Lessons.Count > booking.Lesson_Amount)
            {
                return RedirectToAction("ChooseLessonAmount", "Booking", new { Error_Message = "Error occured start again"});
            }

            ViewBag.Client_ID = ConfigurationManager.AppSettings["Paypal_ClientID"].ToString();
            return View(booking);
        }
    }
}