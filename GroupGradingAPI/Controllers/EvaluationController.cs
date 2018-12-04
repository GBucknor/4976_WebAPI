using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupGradingAPI.Data;
using GroupGradingAPI.Models;
using GroupGradingAPI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroupGradingAPI.Controllers
{
    [EnableCors("AllAccessCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GradingContext _context;

        /**
         * EvaluationController
         *
         * Constructor
         *
         * @param GradingContext context - database context
         * @param UserManager<IdentityUser> userManger - manages user identities
         */
        public EvaluationController(GradingContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        /*
         *         [Key]
        public string EvaluationId { get; set; }


        public string StudentGroupId { get; set; }
        public int CourseCrn { get; set; }
        public string CourseTerm { get; set; }
        public int CourseYear { get; set; }
         */
        /**
         * Inserts a new evaluation into database
         * @param Evaluation model - database context
         * @return JSONObject - returns a JSONObject confirming successful evaluation; else returns an error
         */
        [HttpPost("create/{CourseCrn}")]
        public ActionResult<string> createEvaluation(int CourseCrn)
        {
            try
            {
                var evaluation = _context.Evaluations
                   .Where(c => c.CourseCrn == CourseCrn).FirstOrDefault();
                if (evaluation == null)
                {
                    Evaluation newEvaluation;
                    var sList = _context.CourseStudents
                        .Where(c => c.CourseCrn == CourseCrn)
                        .Select(c => c.StudentId);
                    Console.WriteLine("List:" + sList);

                    foreach (string student in sList)
                    {
                        var st = _context.Students
                            .Where(c => c.UserName == student)
                            .Select(c => new { c.FirstName, c.LastName, c.UserName })
                            .ToList();

                        newEvaluation = new Evaluation();
                        newEvaluation.StudentGroupName = _context.StudentGroup
                            .Where(c => c.StudentId == student && c.CourseCrn == CourseCrn)
                            .Select(c => c.GroupName)
                            .FirstOrDefault();
                        newEvaluation.StudentId = student;
                        newEvaluation.CourseCrn = CourseCrn;
                        newEvaluation.Grades = "";
                        newEvaluation.Comments = "";
                        newEvaluation.CourseName = _context.Courses
                            .Where(c => c.CourseCrn == CourseCrn)
                            .Select(c => c.CourseName)
                            .FirstOrDefault();
                        newEvaluation.Completed = false;
                        _context.Evaluations.Add(newEvaluation);

                    }
                    //Evaluation newEvaluation = new Evaluation();

                    _context.SaveChanges();
                    return JsonConvert.SerializeObject("Created New Evaluations");

                }
                return JsonConvert.SerializeObject("You already created forms for this course.");
            }
            catch (Exception e)
            {

            }
            return JsonConvert.SerializeObject("Error");
        }

        /**
         * Deletes an evaluation from database
         * @param string id - Evaluation ID
         * @return JSONObject - returns a JSONObject confirming successful deletion; else returns an error
         */
        [HttpDelete("delete/{id}")]
        public ActionResult<string> deleteEvaluation(int id)
        {
            try
            {
                var evaluation = _context.Evaluations.Where(c => c.Id == id).FirstOrDefault();

                _context.Evaluations.Remove(evaluation);
                _context.SaveChanges();
                return JsonConvert.SerializeObject("Deleted ");
            }
            catch (Exception e)
            {

            }
            return JsonConvert.SerializeObject("Error");
        }

        /**
         * Gets an evaluation by ID
         * @param string id - Evaluation ID
         * @return JSONObject - returns a JSONObject of specified Evaluation; else returns an error
         */
        [Authorize(Roles = "Student")]
        [HttpGet("{StudentId}/{CourseCrn}")]
        public ActionResult<string> getStudentEvaluation(string StudentId, int CourseCrn)
        {
            try
            {
                var evaluation = _context.Evaluations
                    .Where(c => c.StudentId == StudentId
                     && c.CourseCrn == CourseCrn
                     && c.Completed == false)
                     .FirstOrDefault();
                if (evaluation == null)
                {
                    return JsonConvert.SerializeObject("Error");
                }
                return JsonConvert.SerializeObject(evaluation);
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject("Parsing issue");
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("/i/{CourseCrn}")]
        public ActionResult<bool> getTeacherEvaluations(int CourseCrn)
        {
            try
            {
                var evaluation = _context.Evaluations
                    .Where(c => c.CourseCrn == CourseCrn).FirstOrDefault();                    
                if (evaluation == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [Authorize(Roles = "Student")]
        [HttpGet("{StudentId}")]
        public ActionResult<string> getStudentEvaluations(string StudentId)
        {
            try
            {
                var evaluations = _context.Evaluations
                  .Where(c => 
                  c.StudentId == StudentId
                   && c.Completed == false)
                   .ToList();
                if (evaluations == null)
                {
                    return JsonConvert.SerializeObject("Error");
                }
                return JsonConvert.SerializeObject(evaluations);
            }
            catch (Exception e)
            {
                return JsonConvert.SerializeObject(e);
            }
        }

        /**
         * Edits specified evaluation
         * @param Evaluation model - database context
         * @param string id - Evaluation ID
         * @return JSONObject - returns a JSONObject confirming successful changes; else returns an error
         */
        [Authorize(Roles ="Student")]
        [HttpPut("{id}")]
        public ActionResult<string> seStudentData([FromBody] Evaluation model, [FromRoute] int id)
        {
            try
            {
                var evaluation = _context.Evaluations
                    .Where(c => c.Id == id).FirstOrDefault();
                if (evaluation != null
                    && !evaluation.Completed
                    && evaluation.StudentId == model.StudentId)
                {
                    evaluation.Grades = model.Grades;
                    evaluation.Comments = model.Comments;
                    evaluation.Completed = true;
                    _context.Evaluations.Update(evaluation);
                    _context.SaveChanges();
                    return JsonConvert.SerializeObject("Success");
                }
                return JsonConvert.SerializeObject("Evaluation doesn't exist or have been completed.");


            }
            catch (Exception e)
            {

            }
            return JsonConvert.SerializeObject("Error");
        }

        /**
         * Gets all existing evaluations from database
         * @return JSONObject - returns a list of evaluations as a JSONObject; else returns an error
         */
        [Authorize(Roles ="Teacher")]
        [HttpGet("grades/{id}")]
        public ActionResult<string> getEvaluationData(int id)
        {
            try
            {
                try
                {
                    // pull all groups
                    var groups = _context.StudentGroup
                        .Where(c => c.CourseCrn == id)
                        .OrderBy(c => c.StudentId)
                        .GroupBy(c => c.GroupName)
                        .ToList();
                    var evals = _context.Evaluations
                           .Where(c => c.CourseCrn == id)
                           .OrderBy(c => c.StudentId)
                           .GroupBy(c => c.StudentGroupName)
                           .ToList();
                    List<EvaluatedStudent> eStudents = new List<EvaluatedStudent>();
                    // for each group
                    for (int i = 0; i < groups.Count; i++)
                    {
                        var group = groups[i].ToList();
                        var eval = evals[i].ToList();


                        // create and calculate grade array
                        var res = new double[eval.Count];
                        int evCount = eval.Count;
                        for (int k = 0; k < eval.Count; k++)
                        {
                            var ev = eval[k];
                            
                            if (ev.Grades == "")
                            {
                                // do something here
                                evCount--;
                            } else
                            {
                                var evalGrade = JsonConvert.DeserializeObject<List<double>>(ev.Grades);
                                
                                for(int j = 0; j < res.Length; j++)
                                {
                                    res[j] = res[j] + evalGrade[j];
                                }
                                Console.WriteLine("THIS: " + evalGrade);
                            }

                        }
                        List<double> finalRes;
                        if (evCount == 0)
                        {
                            finalRes = res.Select(c => 50.0).ToList();
                        } else {
                            finalRes = res.Select(c => c / evCount).ToList();
                        }
                        // for each group member update the grade
                        for (int j = 0; j < group.Count; j++)
                        {
                            EvaluatedStudent newStudent = new EvaluatedStudent();
                            newStudent.Grade = Math.Round(finalRes[j]/100.0 * group[j].Grade, 2);
                            newStudent.StudentId = group[j].StudentId;
                            newStudent.GroupName = group[j].GroupName;
                            newStudent.Comments = eval[j].Comments;
                            newStudent.FirstName = _context.Students
                                .Where(c => c.UserName == group[j].StudentId)
                                .Select(c => c.FirstName)
                                .FirstOrDefault();
                            newStudent.LastName = _context.Students
                                .Where(c => c.UserName == group[j].StudentId)
                                .Select(c => c.LastName)
                                .FirstOrDefault();
                            eStudents.Add(newStudent);
                        }
                    }

                    // add to resulted list
                    return JsonConvert.SerializeObject(eStudents);
                }
                catch (Exception e)
                {

                }
                return JsonConvert.SerializeObject("Error");
            }
            catch (Exception e)
            {

            }
            return JsonConvert.SerializeObject("Error");
        }
    }
}
