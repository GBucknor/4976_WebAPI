using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.Models
{
    public class Grade
    {
        [Key]
        public string GradeId { get; set; }
        public string StudentId { get; set; }
        public double Percentage { get; set; }
    }
}
