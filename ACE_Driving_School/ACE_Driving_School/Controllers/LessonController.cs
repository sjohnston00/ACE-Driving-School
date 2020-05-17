using ACE_Driving_School.Models;
using ACE_Driving_School.View_Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace ACE_Driving_School.Controllers
{
    public class LessonController : Controller
    {
        private ACE_Driving_School_Db_Context context = new ACE_Driving_School_Db_Context();   
        public ActionResult ViewAllLessons()
        {
            if (User.IsInRole("Student"))
            {
                //get all the users lessons
                Student student = GetLoggedInStudent();
                if (student == null)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.AbsoluteUri });
                }
                List<Lesson> students_Lessons = context.Lessons.Where(p => p.Student_Id == student.Id)
                                                               .Include(p => p.Student)
                                                               .Include(p => p.Booking)
                                                               .Include(p => p.Car)
                                                               .ToList();
                //need to loop through all the instructors and add them to the lesson
                //because there isnt a student or instructors table
                foreach (var item in students_Lessons)
                {
                    Instructor instructor = (Instructor)context.Users.Find(item.Instructor_Id);
                    item.Instructor = instructor;
                }
                return View(students_Lessons);
            }
            else if (User.IsInRole("Instructor"))
            {
                string Instructor_Id = User.Identity.GetUserId();
                Instructor current_Instructor = (Instructor)context.Users.Find(Instructor_Id);
                //get all the lessons from the databse for the logged in instructor
                List<Lesson> Instructors_Lessons = context.Lessons.Where(p => p.Instructor_Id == current_Instructor.Id)
                                                          .Include(p => p.Student)
                                                          .Include(p => p.Booking)
                                                          .Include(p => p.Car)
                                                          .ToList();

                //adding the instructor and student objects to the list
                foreach (var item in Instructors_Lessons.ToList())
                {
                    item.Instructor = current_Instructor;
                    Student student = (Student)context.Users.Find(item.Student_Id);
                    item.Student = student;
                }

                return View(Instructors_Lessons);
            }
            else if (User.IsInRole("Admin"))
            {

                List<Lesson> All_Lessons = context.Lessons.Include(p => p.Student)
                                                          .Include(p => p.Booking)
                                                          .Include(p => p.Car)
                                                          .ToList();
                //adding the instructor and student objects to the list
                foreach (var item in All_Lessons.ToList())
                {
                    Instructor instructor = (Instructor)context.Users.Find(item.Instructor_Id);
                    item.Instructor = instructor;
                    Student student = (Student)context.Users.Find(item.Student_Id);
                    item.Student = student;
                }
                return View(All_Lessons);

            }
            else 
            {
                //user is not logged in. Redirect
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.AbsoluteUri });
            }
        }
        [HttpGet]
        public ActionResult CreateLesson(int? LessonInBooking, int amount, int? booking_id, string ErrorMessage)
        {
            if (!LessonInBooking.HasValue)
                //if is doesnt have a value if means its the first lesson in the booking
                LessonInBooking = 1;

            if (!ErrorMessage.IsNullOrWhiteSpace())
                ViewBag.ErrorMessage = ErrorMessage;

            //If the instructor hasnt been choosen dont display a list of times
            CreateLessonViewModel newLesson = new CreateLessonViewModel();
            InstructorController instructor_Controller = new InstructorController();
            //Show a list of available times for each instructor
            newLesson.Choosen_Amount = amount;
            newLesson.LessonInBooking = LessonInBooking.Value;
            newLesson.Booking_Id = booking_id.Value;

            List<string> dates = new List<string>();
            dates = FormatDatesList(Next_90_Days());
            newLesson.Dates = dates;
            List<string> times = new List<string>();
            times = FormatTimesList(Working_Hours());
            newLesson.Times = times;

            
            //set the new filtered list to the objects list
            newLesson.Instructors = GetInstructors();

            //set the availabilty of each instructor
            foreach (var item in newLesson.Instructors.ToList())
            {
                item.Availability = instructor_Controller.GetInstructorAvailability(DateTime.Now);
            }

            //set the booking id of the model to the passed in booking id
            return View(newLesson);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLesson(CreateLessonViewModel model)
        {
            Student student = GetLoggedInStudent();

            if (student == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.AbsoluteUri });
            
            Car car = context.Cars.Where(p => p.Gear_Type == model.Car_Type_Chossen).FirstOrDefault();
            if (car == null)
                return Content("Couldnt find a car");
            Instructor instructor = (Instructor)context.Users.Find(model.Chossen_Instructor.Id);
            if (instructor == null)
                return Content("Did not choose instructor");
            
                Booking booking = context.Bookings.Find(model.Booking_Id);

                if (booking == null)
                    //this shouldnt happen
                    return HttpNotFound();

                //create a datetime with the choosen_Date and choosen_Time string
                DateTime date = DateTime.Parse(model.Chossen_Date);
                DateTime time = DateTime.Parse(model.Chossen_Time);
                //add the properties from the view model to the lesson model
                Lesson lesson = new Lesson()
                {
                    Lesson_Id = 0,
                    Booking = booking,
                    Booking_Id = booking.Booking_Id,
                    Student = student,
                    Student_Id = student.Id,
                    Car = car,
                    Car_Id = car.Car_Id,
                    Status = "Incomplete",
                    Date_And_Time = new DateTime(date.Year, date.Month, date.Day, time.Hour, 0, 0),
                    Duration = model.Choosen_Hours,
                    Instructor = instructor,
                    Instructor_Id = instructor.Id,
                    Instructor_Note = "N/A"
                };
                
                //check that the student doesnt book a lesson at the same time as someone else
                List<Lesson> AllLessons = GetAllLessons();
                foreach (var item in AllLessons)
                {
                    if (item.Date_And_Time == lesson.Date_And_Time)
                    {
                        return RedirectToAction("CreateLesson", new { LessonInBooking = model.LessonInBooking, amount = model.Choosen_Amount, booking_id = model.Booking_Id, ErrorMessage = "Someone else has this lesson booked"});
                    }
                }
                
                
                context.Lessons.Add(lesson);
                context.SaveChanges();

                //set the students most recent instructor id to the new lessons instructor id
                student.Most_Recent_Instructor_Id = lesson.Instructor_Id;
                context.SaveChanges();

            if (model.LessonInBooking < model.Choosen_Amount)
            {
                //if the current lesson is less than the chossen amount
                //then save changes and redirect to the same page with an incremented lesson amount by one
                model.LessonInBooking++;
                return RedirectToAction("CreateLesson", new { LessonInBooking = model.LessonInBooking, amount = model.Choosen_Amount, booking_id = model.Booking_Id});
            }
            booking.Lessons = GetBookingsLessons(booking.Booking_Id);
            booking.Price = CalculateLessonPrice(booking.Lessons.Count);
            context.SaveChanges();
            return RedirectToAction("PayForBooking", "Payment", new { Booking_Id = booking.Booking_Id});
        }
        public ActionResult LessonDetails(int Lesson_Id)
        {
            Lesson lesson = context.Lessons.Where(p => p.Lesson_Id == Lesson_Id)
                                                 .Include(p => p.Student)
                                                 .Include(p => p.Instructor)
                                                 .Include(p => p.Booking)
                                                 .Include(p => p.Car)
                                                 .FirstOrDefault();
            return View(lesson);
        }

        [HttpGet]
        public ActionResult EditLesson(int? Lesson_Id, string Return_Url)
        {
            if (!Lesson_Id.HasValue)
                return HttpNotFound("No lesson found");

            Lesson lesson = context.Lessons.Where(p => p.Lesson_Id == Lesson_Id)
                                                 .Include(p => p.Student)
                                                 .Include(p => p.Instructor)
                                                 .Include(p => p.Booking)
                                                 .Include(p => p.Car)
                                                 .FirstOrDefault();
            CreateLessonViewModel viewModel = new CreateLessonViewModel
            {
                Booking_Id = lesson.Booking_Id,
                Lesson_Id = lesson.Lesson_Id,
                Car_Type_Chossen = lesson.Car.Gear_Type,
                Choosen_Hours = lesson.Duration,
                Chossen_Instructor = lesson.Instructor,
                Chossen_Date = lesson.Date_And_Time.Date.ToString(),
                Chossen_Time = lesson.Date_And_Time.Hour.ToString(),
                
            };

            List<string> dates = new List<string>();
            dates = FormatDatesList(Next_90_Days());
            viewModel.Dates = dates;
            List<string> times = new List<string>();
            times = FormatTimesList(Working_Hours());
            viewModel.Times = times;

            viewModel.Instructors = GetInstructors();

            if (!Return_Url.IsNullOrWhiteSpace())
                viewModel.Return_Url = Return_Url;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditLesson(CreateLessonViewModel UpdatedLesson)
        {
            Lesson CurrentDbLesson = context.Lessons.Where(p => p.Lesson_Id == UpdatedLesson.Lesson_Id)
                                                 .Include(p => p.Student)
                                                 .Include(p => p.Instructor)
                                                 .Include(p => p.Booking)
                                                 .Include(p => p.Car)
                                                 .FirstOrDefault();


            DateTime date = DateTime.Parse(UpdatedLesson.Chossen_Date);
            DateTime time = DateTime.Parse(UpdatedLesson.Chossen_Time);
            DateTime newLessonDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, 0, 0);

            //---------------------
            Car car = context.Cars.Where(p => p.Gear_Type == UpdatedLesson.Car_Type_Chossen).FirstOrDefault();
            if (car == null)
                return Content("Couldnt find a car");
            //--------------
            Instructor instructor = (Instructor)context.Users.Find(UpdatedLesson.Chossen_Instructor.Id);

            //these are all the possible values a student can change
            CurrentDbLesson.Car = car;
            CurrentDbLesson.Car_Id = car.Car_Id;
            CurrentDbLesson.Date_And_Time = newLessonDateTime;
            CurrentDbLesson.Duration = UpdatedLesson.Choosen_Hours;
            CurrentDbLesson.Instructor = instructor;
            CurrentDbLesson.Instructor_Id = instructor.Id;

            context.SaveChanges();
            if (!UpdatedLesson.Return_Url.IsNullOrWhiteSpace())
                return Redirect(UpdatedLesson.Return_Url);


            return RedirectToAction("LessonDetails", new { Lesson_Id = UpdatedLesson.Lesson_Id});
        }

        public ActionResult DeleteLesson(int Lesson_Id)
        {
            //add an alert asking the user is they want to confirm the deletion of the lessons
            Lesson lesson = context.Lessons.Where(p => p.Lesson_Id == Lesson_Id)
                                                                  .Include(p => p.Booking)
                                                                  .Include(p => p.Student)
                                                                  .Include(p => p.Car)
                                                                  .Include(p => p.Instructor)
                                                                  .FirstOrDefault();
            context.Lessons.Remove(lesson);
            context.SaveChanges();

            //update the bookings lessons
            BookingController booking = new BookingController();
            booking.UpdateBookingsLesson(lesson.Booking_Id);
            return RedirectToAction("ViewBooking","Booking", new { Booking_Id = lesson.Booking_Id});
        }

        [HttpGet]
        public ActionResult AddInstructorNote(int? Lesson_Id)
        {
            if (!Lesson_Id.HasValue)
                return HttpNotFound();

            if (!User.Identity.IsAuthenticated || !User.IsInRole("Instructor"))
                return RedirectToAction("Login", "Account");

            //correct user is logged in
            Lesson lesson = context.Lessons.Where(p => p.Lesson_Id == Lesson_Id)
                                                                  .Include(p => p.Booking)
                                                                  .Include(p => p.Student)
                                                                  .Include(p => p.Car)
                                                                  .Include(p => p.Instructor)
                                                                  .FirstOrDefault();
            AddInstructorNoteViewModel viewModel = new AddInstructorNoteViewModel()
            {
                Instructor_Id = lesson.Instructor_Id,
                Lesson_Id = lesson.Lesson_Id,
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddInstructorNote(AddInstructorNoteViewModel viewModel)
        {
            if (viewModel == null)
                return HttpNotFound();

            Lesson lesson = context.Lessons.Where(p => p.Lesson_Id == viewModel.Lesson_Id)
                                                                  .Include(p => p.Booking)
                                                                  .Include(p => p.Student)
                                                                  .Include(p => p.Car)
                                                                  .Include(p => p.Instructor)
                                                                  .FirstOrDefault();
            lesson.Instructor_Note = viewModel.Note;
            context.SaveChanges();
            return RedirectToAction("LessonDetails", new { Lesson_Id = lesson.Lesson_Id});
        }
        
        public ActionResult MarkLessonComplete(int Lesson_Id)
        {
            Lesson lesson = context.Lessons.Find(Lesson_Id);
            lesson.Status = "Complete";
            context.SaveChanges();
            return RedirectToAction("ViewBooking", "Booking", new { Booking_Id = lesson.Booking_Id });
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

        public Student GetLoggedInStudent()
        {
            string StudentId = User.Identity.GetUserId();
            Student student = (Student)context.Users.Find(StudentId);
            return student;
        }

        public List<Lesson> GetAllLessons()
        {
            List<Lesson> AllLesson = context.Lessons.ToList();

            return AllLesson;
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

        public List<DateTime> Next_90_Days()
        {
            var Tomorrow = DateTime.Today.AddDays(1);
            var Ninety_Days_Ahead = Tomorrow.AddDays(89);
            List<DateTime> Next_90_Days = new List<DateTime>();

            for (DateTime date = Tomorrow; date <= Ninety_Days_Ahead; date = date.AddDays(1))
            {
                Next_90_Days.Add(date);
            }
            return Next_90_Days;
        }

        public List<DateTime> Working_Hours()
        {
            var NineOClock = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
            var SevenOClock = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
            List<DateTime> Working_Times = new List<DateTime>();
            for (DateTime hour = NineOClock; hour.Hour < SevenOClock.Hour; hour = hour.AddHours(1))
            {
                Working_Times.Add(hour);
            }
            return Working_Times;
        }

        public List<string> FormatDatesList(List<DateTime> dates)
        {
            if (dates == null)
                return null;

            List<string> ParsedDates = new List<string>();
            string date;
            foreach (var item in dates)
            {
                date = item.ToString("dd/MM/yyyy");
                ParsedDates.Add(date);
            }
            return ParsedDates;
        }

        public List<string> FormatTimesList(List<DateTime> Times)
        {
            if (Times == null)
                return null;

            List<string> ParsedTimes = new List<string>();
            string Time;
            foreach (var item in Times)
            {
                Time = item.ToString("HH:mm");
                ParsedTimes.Add(Time);
            }
            return ParsedTimes;
        }

        public List<Instructor> GetInstructors()
        {
            //create a new list and filter all the users to be instructors and add them to the list
            List<User> users = new List<User>();
            List<Instructor> instructors = new List<Instructor>();
            users = context.Users.ToList();


            foreach (var item in users)
            {
                //need to save the current role into a string first, for the method to run properly
                string currentRole = item.CurrentRole;
                if (currentRole == "Instructor")
                {
                    instructors.Add((Instructor)item);
                }
            }

            return instructors;
        }
    }
}