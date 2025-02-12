using Microsoft.EntityFrameworkCore;
using StudentEfCoreDemo.Models;
using System.Collections.Generic;

namespace StudentEfCoreDemo.Data
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; } = null!;
    }
}
