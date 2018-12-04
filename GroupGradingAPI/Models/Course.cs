using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.Models
{
    public class Course
    {
        [Key]
        public int CourseCrn { get; set; }
        public string CourseTerm { get; set; }
        public int CourseYear { get; set; }
        public string CourseName { get; set; }
        public string InstructorId { get; set; }
    }
}
