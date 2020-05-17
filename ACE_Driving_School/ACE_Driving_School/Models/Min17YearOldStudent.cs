using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class Min17YearOldStudent : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var User = (RegisterViewModel)validationContext.ObjectInstance;


            DateTime today = DateTime.Today;
            int age = today.Year - User.Date_Of_Birth.Year;
            if (User.Date_Of_Birth > today.AddYears(-age))
            {
                age--;
            }

            if (age >= 17)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("You must be 17 years old");
            }
        }
    }
}