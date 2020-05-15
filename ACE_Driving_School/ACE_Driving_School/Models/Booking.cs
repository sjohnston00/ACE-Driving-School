using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class Booking
    {
        [Key]
        [Display(Name ="Booking Id")]
        public int Booking_Id { get; set; }
        [Display(Name = "Lessom Amount")]
        public int Lesson_Amount { get; set; }
        [Display(Name ="Date and Time")]
        public DateTime Date_and_Time { get; set; }
        public List<Lesson> Lessons { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "Payment Status")]
        public string Payment_Status { get; set; }
        [ForeignKey("Student")]
        public string Student_Id { get; set; }
        public Student Student { get; set; }


    }
}