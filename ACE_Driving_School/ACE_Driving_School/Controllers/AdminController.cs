using ACE_Driving_School.Models;
using ACE_Driving_School.View_Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ACE_Driving_School.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

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

            return View();
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

            //if both everything is successful return the admin to the control panel
            if (result.Succeeded && AddingToRole.Succeeded)
                return RedirectToAction("ControlPanel");



            //if we got here something went wrong
            return View(model);
        }

        public ActionResult ChangeLessonPrice()
        {

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