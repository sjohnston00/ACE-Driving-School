using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ACE_Driving_School.Models
{
    public class ACE_Driving_School_Db_Context : IdentityDbContext<User>
    {
        public DbSet<Booking> Bookings{ get; set; }
        public DbSet<Lesson> Lessons{ get; set; }
        public DbSet<Car> Cars{ get; set; }

        public ACE_Driving_School_Db_Context() : base ("ACE_Driving_School_DB", throwIfV1Schema: false)
        {
            Database.SetInitializer(new DatabaseInitializer());
        }

        public static ACE_Driving_School_Db_Context Create()
        {
            return new ACE_Driving_School_Db_Context();
        }
    }
}