using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class Student : User
    {
        [NotMapped]
        public List<Booking> Bookings { get; set; }
        [Display(Name ="Driving License Number")]
        [RegularExpression("[A-Z0-9]{5}\\d[0156]\\d([0][1-9]|[12]\\d|3[01])\\d[A-Z0-9]{3}[A-Z]{2}", ErrorMessage = "Please enter a valid driving license number" )]
        public string DrivingLicenseNo { get; set; }
        public bool hasPassed { get; set; }
        public DateTime? PassedDate { get; set; }
        public string Most_Recent_Instructor_Id{ get; set; }


    }
}