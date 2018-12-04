using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupGradingAPI.ViewModel
{
    public class EvaluatedStudent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
        public double Grade { get; set; }
        public string Comments { get; set; }
        public string GroupName { get; set; }
    }
}
