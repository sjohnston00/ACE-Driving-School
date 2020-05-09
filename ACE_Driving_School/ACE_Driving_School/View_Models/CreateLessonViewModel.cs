using ACE_Driving_School.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.View_Models
{
    public class CreateLessonViewModel
    {
        public int? Booking_Id { get; set; }
        public int? Lesson_Id { get; set; }
        public int Choosen_Amount { get; set; }

        public List<string> Dates { get; set; }
        public string Chossen_Date { get; set; }
        
        public List<string> Times { get; set; }
        public string Chossen_Time { get; set; }

        [Display(Name ="Duraction (Hours)")]
        public List<int> Duration
        {
            get
            {
                List<int> duration = new List<int>() { 1, 2 };
                return duration;
            }
        }
        public int Choosen_Hours { get; set; }

        [Display(Name ="Instructor")]
        public Instructor Chossen_Instructor { get; set; }
        public List<Instructor> Instructors { get; set; }

        [Display(Name ="Car Type")]
        public List<string> Car_Type 
        {
            get { List<string> carType = new List<string>() { "Automatic", "Manual" };
                return carType;
            }
        }
        public string Car_Type_Chossen { get; set; }

        public int LessonInBooking { get; set; }
    }
}