﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {

        public Course()
        {
            Resources = new HashSet<Resource>();
            HomeworkSubmissions = new HashSet<Homework>();
            StudentsEnrolled = new HashSet<StudentCourse>();
        }
        public int CourseId { get; set; }

        [Column(TypeName = "NVARCHAR(80)")]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal Price { get; set; }


        public ICollection<Resource> Resources { get; set; }
        public ICollection<Homework> HomeworkSubmissions { get; set; }
        public ICollection<StudentCourse> StudentsEnrolled { get; set; }

}
}
