using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class Payment
    {
        [Key]
        public string Payment_Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date{ get; set; }
        public string Status { get; set; }
        [ForeignKey("Student")]
        public string Student_Id { get; set; }
        public Student Student { get; set; }
    }
}