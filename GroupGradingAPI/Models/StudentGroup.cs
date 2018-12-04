using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.Models
{
    public class StudentGroup
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string StudentId { get; set; }
        public int CourseCrn { get; set; }
        public double Grade { get; set; }
        //public List<Student> Students { get; set; }
    }
}
