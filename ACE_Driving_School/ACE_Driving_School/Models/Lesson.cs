using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class Lesson
    {
        [Key]
        public int Lesson_Id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name ="Date And Time")]
        public DateTime Date_And_Time { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }

        [Display(Name ="Instructor Note")]
        public string Instructor_Note { get; set; }

        [ForeignKey("Booking")]
        [Display(Name ="Booking Id")]
        public int Booking_Id { get; set; }
        public Booking Booking { get; set; }

        [ForeignKey("Student")]
        public string Student_Id { get; set; }
        public Student Student { get; set; }

        [ForeignKey("Instructor")]
        public string Instructor_Id { get; set; }
        public Instructor Instructor { get; set; }

        [ForeignKey("Car")]
        public string Car_Id { get; set; }
        public Car Car { get; set; }
        
    }
}