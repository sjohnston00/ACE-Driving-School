using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.View_Models
{
    public class AddInstructorNoteViewModel
    {
        public int Lesson_Id { get; set; }
        public string Instructor_Id{ get; set; }
        public string Note { get; set; }
    }
}