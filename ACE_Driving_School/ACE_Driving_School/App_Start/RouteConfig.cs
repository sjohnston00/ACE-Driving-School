using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ACE_Driving_School
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Create_Multiple_Lessons",
                url: "Lesson/Create/{LessonInBooking}/{Booking_Id}/{amount}",
                defaults: new { controller = "Lesson", action = "CreateLesson" }
                );

            routes.MapRoute(
                name: "Create_First_Lesson",
                url: "Lesson/Create/{amount}",
                defaults: new { controller = "Lesson", action = "CreateLesson" }
                );
            
            routes.MapRoute(
                name: "Choose_Lesson_Amount",
                url: "Booking/ChooseLessonAmount/{Booking_Id}",
                defaults: new { controller = "Booking", action = "ChooseLessonAmount" }
                );
                
                

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
