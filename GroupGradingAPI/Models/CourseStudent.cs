using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.Models
{
    public class CourseStudent
    {
        [Key]
        public int CourseStudentId { get; set; }
        public string StudentId { get; set; }
        public int CourseCrn { get; set; }
    }
}
