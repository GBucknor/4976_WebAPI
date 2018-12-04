using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.ViewModel
{
    public class SetCourseViewModel
    {
        [Required]
        public string CourseTerm { get; set; }
        [Required]
        public int CourseYear { get; set; }
        [Required]
        public string CourseName { get; set; }
    }
}
