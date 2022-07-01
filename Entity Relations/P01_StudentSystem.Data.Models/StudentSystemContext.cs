﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }
        public StudentSystemContext(DbContextOptions options) : base(options)
        {
        }

        public  DbSet<Course> Courses { get; set; }
        public virtual DbSet<Homework> HomeworkSubmissions { get; set; }
        public  DbSet<Resource> Resources { get; set; }
        public  DbSet<Student> Students { get; set; }
        public  DbSet<StudentCourse> StudentCourses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>().HasKey(c => new { c.StudentId, c.CourseId });
            base.OnModelCreating(modelBuilder);
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=StudentSystem;Integrated Security=True");
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}
