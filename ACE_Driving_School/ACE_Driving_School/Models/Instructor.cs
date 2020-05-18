using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class Instructor : User
    {
        public List<Student> Current_Students { get; set; } 
        public List<Lesson> Lessons { get; set; }
        public List<DateTime> Availability{ get; set; }

        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }
        [Display (Name = "Experience Years")]
        public int Experience_Years { get; set; }


    }
}