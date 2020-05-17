using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class User : IdentityUser
    {
        //DO THIS FOR EVERY CLASS
        //Author: Sam Johnston
        //Date: 25/3/2020
        //Description: Generalised users class for users in the systems, stores basic information about each user

        private ApplicationUserManager userManager;
        [Required]
        [Display (Name = "First Name")]
        public string First_Name { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }
        [NotMapped]
        public string FullName
        {
            get
            {
                return First_Name + " " + Last_Name;
            }
        }
        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime Date_Of_Birth { get; set; }
        [Required]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Required]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Postcode { get; set; }
        [NotMapped]
        public string CurrentRole
        {
            get
            {
                if (userManager == null)
                {
                    userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

                }

                return userManager.GetRoles(Id).SingleOrDefault();
            }
           
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}