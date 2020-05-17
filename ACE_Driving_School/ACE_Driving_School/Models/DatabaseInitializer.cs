using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<ACE_Driving_School_Db_Context>
    {
        protected override void Seed(ACE_Driving_School_Db_Context context)
        {
            if (!context.Users.Any())
            {

                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                if (!roleManager.RoleExists("Admin"))
                {
                    roleManager.Create(new IdentityRole("Admin"));
                }
                if (!roleManager.RoleExists("Instructor"))
                {
                    roleManager.Create(new IdentityRole("Instructor"));
                }
                if (!roleManager.RoleExists("Student"))
                {
                    roleManager.Create(new IdentityRole("Student"));
                }

                context.SaveChanges();

                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));

                if (userManager.FindByName("admin1@acedrivingschool.com") == null)
                {
                    userManager.PasswordValidator = new PasswordValidator()
                    {
                        RequireDigit = true,
                        RequiredLength = 8,
                        RequireLowercase = true,
                        RequireNonLetterOrDigit = false,
                        RequireUppercase = true
                    };
                    //creating an admin
                    var admin1 = new User()
                    {
                        UserName = "admin1",
                        First_Name = "George",
                        Last_Name = "Sampson",
                        Date_Of_Birth = new DateTime(1996, 01, 23),
                        Email = "georgesampson@gmail.com",
                        EmailConfirmed = true,
                        AddressLine1 = "21 London Road",
                        AddressLine2 = "Wakefield",
                        City = "Leeds",
                        Postcode = "LS12 3DS",
                        PhoneNumber = "07429472819"
                    };
                    userManager.Create(admin1, "ACEadmin1");
                    userManager.AddToRole(admin1.Id, "Admin");
                    context.SaveChanges();


                    

                    //creating an instructor
                    var instructor1 = new Instructor()
                    {
                        UserName = "instructor1",
                        First_Name = "Tony",
                        Last_Name = "Johnston",
                        Date_Of_Birth = new DateTime(1965, 10, 7),
                        Email = "tonyjohnston@gmail.com",
                        EmailConfirmed = true,
                        AddressLine1 = "72 Wakefield Road",
                        AddressLine2 = "Rothwell",
                        City = "Leeds",
                        Postcode = "LS26 0SF",
                        PhoneNumber = "07439201921",
                        Experience_Years = 12,
                        Bio = "My name is Tony Johnston and I have been an instructor for a long time \n " +
                        "I love teaching new students to drive. \n" +
                        "I believe in a rough approach to teaching so be prepared if you pick me"
                      
                    };
                    userManager.Create(instructor1, "Instructor1");
                    userManager.AddToRole(instructor1.Id, "Instructor");
                    context.SaveChanges();

                    //creating a student
                    var student1 = new Student()
                    {
                        UserName = "student1",
                        First_Name = "Sam",
                        Last_Name = "Johnston",
                        Date_Of_Birth = new DateTime(2000, 12, 12),
                        Email = "student1@student1.com",
                        EmailConfirmed = true,
                        AddressLine1 = "200 North Hanover",
                        AddressLine2 = "601 Dobbies Point",
                        City = "Glasgow",
                        Postcode = "G4 0PY",
                        PhoneNumber = "07498508958",
                        Most_Recent_Instructor_Id = instructor1.Id,
                        DrivingLicenseNo = "MORGA753116SM9IJ",
                        hasPassed = false,
                        TestDate = DateTime.Now.Date.AddDays(-1)
                    };
                    userManager.Create(student1, "ACEstudent1");
                    userManager.AddToRole(student1.Id, "Student");
                    context.SaveChanges();

                    //creating a student
                    var student2 = new Student()
                    {
                        UserName = "student2",
                        First_Name = "John",
                        Last_Name = "Doe",
                        Date_Of_Birth = new DateTime(1998, 06, 03),
                        Email = "student2@student2.com",
                        EmailConfirmed = true,
                        AddressLine1 = "32 Wakefield Way",
                        AddressLine2 = "Watford",
                        City = "London",
                        Postcode = "LN2 3RD",
                        PhoneNumber = "07485472617",
                        Most_Recent_Instructor_Id = instructor1.Id,
                        DrivingLicenseNo = "MORGA753116SM9IJ",
                        hasPassed = true,
                        PassedDate = DateTime.Now.Date
                    };
                    userManager.Create(student2, "ACEstudent2");
                    userManager.AddToRole(student2.Id, "Student");
                    context.SaveChanges();

                    //------creating a booking---------
                    var booking1 = new Booking 
                    {
                        Booking_Id = 1,
                        Date_and_Time = DateTime.Now,
                        Payment_Status = "Incomplete",
                        Price = 20.00M,
                        Student = student1,
                    };
                    //--------creating a car for lesson-------
                    var car1 = new Car 
                    {
                        Car_Id = "1",
                        Make = "Peugeot",
                        Miles = 2000,
                        Model = "207",
                        Registration_Plate = "BR20 DSG",
                        Gear_Type = "Manual",
                        Year = new DateTime(2004,12,12)
                    };
                    var car2 = new Car 
                    {
                        Car_Id = "2",
                        Make = "Tesla",
                        Miles = 1000,
                        Model = "Model S",
                        Registration_Plate = "SR71 BOP",
                        Gear_Type = "Automatic",
                        Year = new DateTime(2017,01,01)
                    };
                    //------------creating a lesson for a booking-----------
                    var lesson1 = new Lesson
                    {
                        Lesson_Id = 1,
                        Booking = booking1,
                        Booking_Id = booking1.Booking_Id,
                        Date_And_Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 0, 0),
                        Duration = 1,
                        Instructor = instructor1,
                        Instructor_Note = "Take student round M6",
                        Status = "Incomplete",
                        Student = student1,
                        Car = car1
                    };
                    //---ADDING THE REST OF THE REFERNCES TO THE BOOKING
                        //adding the lesson to the booking
                        booking1.Lessons = new List<Lesson>() { lesson1 };
                        booking1.Lesson_Amount = booking1.Lessons.Count;

                    //adding all the objects to the database
                    context.Bookings.Add(booking1);
                    context.Cars.Add(car1);
                    context.Cars.Add(car2);
                    context.Lessons.Add(lesson1);
                    context.SaveChanges();

                }

            }
        }
    }
}