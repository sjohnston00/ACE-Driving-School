using ACE_Driving_School.Models;
using ACE_Driving_School.View_Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ACE_Driving_School.Controllers
{
    public class BookingController : Controller
    {
        private ACE_Driving_School_Db_Context context = new ACE_Driving_School_Db_Context();
        /// <summary>
        /// Page to view all the bookings in the database, for a user or an admin
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAllBookings()
        {
            if (User.IsInRole("Student"))
            {
                Student student = GetLoggedInStudent();
                if (student == null)
                    return HttpNotFound("No student found");

                List <Booking> Students_Bookings = context.Bookings.Where(p => p.Student_Id == student.Id)
                                                                    .Where(p => p.Payment_Status == "Complete")
                                                                    .Include(p => p.Student)
                                                                    .ToList();
                //Add the lessons to the bookings
                foreach (var item in Students_Bookings)
                {
                    item.Lessons = context.Lessons.Where(p => p.Booking_Id == item.Booking_Id)
                                                        .Include(p => p.Student)
                                                        .Include(p => p.Instructor)
                                                        .Include(p => p.Car)
                                                        .ToList();
                }
                return View(Students_Bookings);
            }
            else if (User.IsInRole("Admin") || User.IsInRole("Instructor"))
            {
                List<Booking> All_Bookings = context.Bookings.Include(p => p.Student).ToList();
                return View(All_Bookings);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        /// <summary>
        /// Partial Page for a user to choose the amount of lesson they want
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChooseLessonAmount(string Error_Message)
        {
            if (!Error_Message.IsNullOrWhiteSpace())
            {
                ViewBag.ErrorMessage = $"{Error_Message}";
            }
            LessonAmounts_ViewModel LessonAmounts = new LessonAmounts_ViewModel();
            return View(LessonAmounts);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseLessonAmount(LessonAmounts_ViewModel model)
        {
            return RedirectToAction("CreateBooking", new { Chossen_Amount = model.chossen_Amount});
            
        }
        /// <summary>
        /// Saving the users booking to the database and all the lessons
        /// </summary>
        /// <param name="booking"></param>
        /// <returns></returns>
        public ActionResult CreateBooking(int Chossen_Amount)
        {
            if (ModelState.IsValid)
            {
                    Student student = GetLoggedInStudent();
                    if (student == null)
                        return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.AbsoluteUri });

                    if (student.EmailConfirmed == false)
                        return RedirectToAction("Index", "Manage");

                Booking booking = new Booking()
                {
                    Date_and_Time = DateTime.Now,
                    Lesson_Amount = Chossen_Amount,
                    Payment_Status = "Incomplete",
                    Price = 0, //set to 0 when a new booking, and update once lessons have all be added
                    Student = student,
                    Student_Id = student.Id,
                };
                context.Bookings.Add(booking);
                context.SaveChanges();
                return RedirectToAction("CreateLesson", "Lesson", new { amount = Chossen_Amount, booking_id = booking.Booking_Id });
            }
            
            //if we reach here something hasnt gone right
            return Content("Oops something isn't quite right please try again" +
                            "<a href=\"javascript: history.go(-1)\">Go Back</a>");
        }

        public ActionResult BookingConfirmation(int Booking_Id)
        {
            Booking booking = context.Bookings.Where(p => p.Booking_Id == Booking_Id)
                                                    .Include(p => p.Student).FirstOrDefault();
            booking.Lessons = GetBookingsLessons(booking.Booking_Id);
            booking.Payment_Status = "Complete";

            context.SaveChanges();
            SendCreateBookingConfirmationEmail(booking.Student, booking);
            return View(booking);
        }
        public ActionResult DeleteBooking(int Booking_Id)
        {
            //add alert to warn a user that this will delete all the lessons too
            Booking booking = context.Bookings.Where(p => p.Booking_Id == Booking_Id)
                                                    .Include(p => p.Student)
                                                    .FirstOrDefault();
            booking.Lessons = GetBookingsLessons(booking.Booking_Id);

            //delete all the lessons first
            foreach (var item in booking.Lessons.ToList())
            {
                context.Lessons.Remove(item);
                booking.Lessons.Remove(item);
            }

            context.SaveChanges();

            context.Bookings.Remove(booking);
            context.SaveChanges();

            return RedirectToAction("ViewAllBookings");
        }

        public ActionResult ViewBooking(int? Booking_Id)
        {
            if (!Booking_Id.HasValue)
                return View("ViewAllBookings");

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (User.IsInRole("Student"))
            {
                Student student = GetLoggedInStudent();
                if (student == null)
                    return RedirectToAction("Login", "Account");
                Booking booking = context.Bookings.Where(p => p.Booking_Id == Booking_Id)
                                                        .Include(p => p.Student)
                                                        .FirstOrDefault();

                if (student.Id != booking.Student.Id)
                    //students is trying to access another students booking
                    return Content("Not your booking");
                booking.Lessons = GetBookingsLessons(booking.Booking_Id);
                return View(booking);
            }
            else if (User.IsInRole("Instructor"))
            {
                Instructor instructor = GetLoggedInInstructor();
                if (instructor == null)
                    return RedirectToAction("Login", "Account");
                Booking booking = context.Bookings.Where(p => p.Booking_Id == Booking_Id)
                                                        .Include(p => p.Student)
                                                        .FirstOrDefault();

                booking.Lessons = GetBookingsLessons(booking.Booking_Id);
                return View(booking);
            }
            else if (User.IsInRole("Admin"))
            {
                User admin = GetLoggedInAdmin();
                if (admin == null)
                    return RedirectToAction("Login", "Account");
                Booking booking = context.Bookings.Where(p => p.Booking_Id == Booking_Id)
                                                        .Include(p => p.Student)
                                                        .FirstOrDefault();
                
                booking.Lessons = GetBookingsLessons(booking.Booking_Id);
                return View(booking);
            }
            //user is not in any role
            return null;
            
        }

        public ActionResult UpdateBookingsLesson(int Booking_Id)
        {
            Booking booking = context.Bookings.Where(p => p.Booking_Id == Booking_Id)
                                              .Include(p => p.Student)
                                              .FirstOrDefault();

            if (booking.Lesson_Amount == 1)
            {
                //theres only one lesson in the booking and we'll have to delete the whole booking
                return RedirectToAction("DeleteBooking", new { Booking_Id = booking.Booking_Id});
            }
            else
            {
                booking.Lesson_Amount--;
                context.SaveChanges();
            }

            //bookings lesson amount has been lowered 
            //send a confirmation email to student
            SendUpdateBookingConfirmationEmail(booking.Student, booking);
            return null;
        }

        public void SendCreateBookingConfirmationEmail(Student student, Booking booking)
        {
            string ViewAllBooking_URL = Url.Action("ViewAllBookings", "Booking");
            var Email = new IdentityMessage
            {
                Destination = student.Email,
                Subject = $"ACE Driving School Booking Confirmation. Booking Id:{booking.Booking_Id}",
                Body = $"Congratiolations {student.FullName}, you have made a booking of {booking.Lessons.Count} lessons \n" +
                       $"on {booking.Date_and_Time}\n" +
                       $"Make sure to check your paypal account for the receipt \n" +
                       $"Check your bookings: {ViewAllBooking_URL}"
            };

            SendEmail(Email);
        }

        public void SendUpdateBookingConfirmationEmail(Student student, Booking booking)
        {
            string ViewBooking_URL = Url.Action("ViewBooking", "Booking", new { Booking_Id = booking.Booking_Id });
            string ACE_Driving_School_Email_Address = "acedrivingschoolofficial@gmail.com";
            var Email = new IdentityMessage
            {
                Destination = student.Email,
                Subject = $"ACE Driving School Update Booking Confirmation {booking.Booking_Id}",
                Body = $"Just to let you know {student.FullName}, you have recently updated a booking that now only contains {booking.Lessons.Count} lessons \n" +
                       $"Updated on: {booking.Date_and_Time} \n" +
                       $"Check your bookings here: {ViewBooking_URL}" +
                       $"If this wasnt you then make sure to send us an email straight away to inform us {ACE_Driving_School_Email_Address}"
            };

            SendEmail(Email);
        }
        public void SendEmail(IdentityMessage message)
        {
            string To = message.Destination;
            string body = message.Body;
            string subject = message.Subject;
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.SmtpUseDefaultCredentials = true;
            WebMail.EnableSsl = true;
            WebMail.UserName = ConfigurationManager.AppSettings["Email"].ToString();
            WebMail.Password = ConfigurationManager.AppSettings["Email_Password"].ToString();

            WebMail.Send(To, subject, body);
        }
        public decimal CalculateLessonPrice(int LessonAmount)
        {
            switch (LessonAmount)
            {
                case 1:
                    decimal Lesson_1_Price = 20.00M;
                    return Lesson_1_Price;
                case 2:
                    decimal Lesson_2_Price = 40.00M;
                    return Lesson_2_Price;
                case 5:
                    decimal Lesson_5_Price = 95.00M;
                    return Lesson_5_Price;
                case 10:
                    decimal Lesson_10_Price = 180.00M;
                    return Lesson_10_Price;
                case 20:
                    decimal Lesson_20_Price = 350.00M;
                    return Lesson_20_Price;
                default:
                    //return 0 if isnt any of them, THIS SHOULDN'T BE REACHED
                    return 0.00M;
            }
        }

        public Student GetLoggedInStudent()
        {
            string StudentId = User.Identity.GetUserId();
            try
            {
                Student student = (Student)context.Users.Find(StudentId); 
                return student;
            }
            catch (Exception e)
            {
                return null;
            }
            
        }
        
        public Instructor GetLoggedInInstructor()
        {
            string InstructorId = User.Identity.GetUserId();
            try
            {
                Instructor instructor = (Instructor)context.Users.Find(InstructorId); 
                return instructor;
            }
            catch (Exception e)
            {
                return null;
            }
            
        }
        
        public User GetLoggedInAdmin()
        {
            string AdminId = User.Identity.GetUserId();
            try
            {
                User admin = context.Users.Find(AdminId); 
                return admin;
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

        
        public List<Lesson> GetBookingsLessons(int Booking_Id)
        {
            List<Lesson> LessonsOfBooking = context.Lessons.Where(p => p.Booking_Id == Booking_Id)
                                                                 .Include(p => p.Car)
                                                                 .Include(p => p.Student)
                                                                 .Include(p => p.Instructor)
                                                                 .Include(p => p.Booking)
                                                                 .ToList();
            return LessonsOfBooking;
        }
    }
}