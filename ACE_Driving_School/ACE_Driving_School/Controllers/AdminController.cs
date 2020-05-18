using ACE_Driving_School.Models;
using ACE_Driving_School.View_Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ACE_Driving_School.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ACE_Driving_School_Db_Context context = new ACE_Driving_School_Db_Context();

        public AdminController()
        {

        }
        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult ControlPanel()
        {
            //validate user logged in
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            //validate user role
            if (User.IsInRole("Instructor") || User.IsInRole("Student"))
                return RedirectToAction("Index", "Home");
            return View();
        }


        public ActionResult ViewAllUsers()
        {
            List<User> users = context.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public ActionResult NewStaffMember()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewStaffMember(CreateInstructorViewModel model)
        {
            if (!ModelState.IsValid)
               return View(model);

            var user = new Instructor
            {
                UserName = model.Username,
                Email = model.Email,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                City = model.City,
                Postcode = model.Postcode,
                First_Name = model.Firstname,
                Last_Name = model.Lastname,
                PhoneNumber = model.PhoneNumber,
                Date_Of_Birth = model.Date_Of_Birth,
                Bio = model.Bio,
                Experience_Years = model.Experience_Years
            };
            var result = await UserManager.CreateAsync(user, model.Password);

            //find the instructor and assign him to the instructor role
            Instructor instructor = (Instructor) await UserManager.FindByEmailAsync(model.Email);
            var AddingToRole = await UserManager.AddToRoleAsync(instructor.Id, "Instructor");

            //if both have succeeded everything is successful return the admin to the control panel
            if (result.Succeeded && AddingToRole.Succeeded)
                return RedirectToAction("ControlPanel");



            //if we got here something went wrong
            return View(model);
        }

        public ActionResult CheckStudentTestDate()
        {
            List<Student> students_with_upcoming_tests = context.Users.OfType<Student>()
                                                                        .Where(p => p.hasPassed == false)
                                                                        .Where(p => p.TestDate.HasValue)
                                                                        .Where(p => p.TestDate < DateTime.Now)
                                                                        .ToList();
            foreach (var item in students_with_upcoming_tests)
            {
                double daysbetween = (item.TestDate.Value.Date - DateTime.Now.Date).TotalDays;
                if (daysbetween < 5)
                {
                    string URL = Url.Action("ViewAllLessons", "Lesson");
                    var message = new IdentityMessage
                    {
                        Destination = item.Email,
                        Body = $"Hey {item.FullName} we've noticed that you have a test coming up within 5 days make sure to get some lesson booked before then by clicking here:{URL}",
                        Subject = $"Upcoming Test on {item.TestDate.Value.Date}"
                    };

                    SendEmail(message);

                }
            }

            return RedirectToAction("ControlPanel");
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

        public ActionResult ChangeLessonPrice()
        {
            //will look at this in future development
            //will need to have a table in the database with the current lesson prices
            // then be able to change them there
            //and have all the others page load the info from the database
            return View();
        }

        public ActionResult GenerateReports()
        {
            //first will need to see which report the user wants to print
            //then work out the logic for that reports
            //then make the pdf 
            //try using this video as a refernce https://www.youtube.com/watch?v=vXxNDZpCIkQ
            return View();
        }
    }
}