using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.View_Models
{
    public class LessonAmounts_ViewModel
    {
        public List<int> Lesson_Amount 
        {
            get 
            {
                List<int> amounts = new List<int>() { 1, 2, 5, 10, 20 };
                return amounts;
            }
        }
        public int chossen_Amount { get; set; }
    }
}