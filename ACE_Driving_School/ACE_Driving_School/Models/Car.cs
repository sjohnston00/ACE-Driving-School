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
        public int Car_Id { get; set; }
        [NotMapped]
        public string Name { get { return $"{Year.Year} {Make} {Model}"; } }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Year { get; set; }
        [Required]
        public int Miles { get; set; }
        [Required]
        [Display(Name ="Registration Plate")]
        public string Registration_Plate { get; set; }
        [Required]
        [Display(Name = "Gear Type")]
        public string Gear_Type { get; set; }
    }
}