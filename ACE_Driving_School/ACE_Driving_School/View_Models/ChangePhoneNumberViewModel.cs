using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.View_Models
{
    public class ChangePhoneNumberViewModel
    {
        public string User_Id { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Old_PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string New_PhoneNumber { get; set; }

    }
}