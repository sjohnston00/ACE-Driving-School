using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class Car
    {
        [Key]
        [Display(Name ="Car Id")]
        public string Car_Id { get; set; }
        [NotMapped]
        public string Name { get { return $"{Year.Year} {Make} {Model}"; } }
        public string Make { get; set; }
        public string Model { get; set; }
        public DateTime Year { get; set; }
        public int Miles { get; set; }
        [Display(Name ="Registration Plate")]
        public string Registration_Plate { get; set; }
        [Display(Name = "Gear Type")]
        public string Gear_Type { get; set; }
    }
}