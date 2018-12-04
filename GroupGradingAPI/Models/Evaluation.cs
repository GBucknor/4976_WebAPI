using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.Models
{
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }
        public string StudentGroupName { get; set; }
        public string StudentId { get; set; }
        public int CourseCrn { get; set; }
        public string CourseName { get; set; }
        public string Grades { get; set; }
        public string Comments { get; set; }
        public bool Completed { get; set; }
    }
}
