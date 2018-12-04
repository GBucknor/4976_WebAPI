using System;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GradingContext _context;

        /**
         * CourseController
         *
         * Constructor
         *
         * @param GradingContext context - database context
         * @param UserManager<IdentityUser> userManger - manages user identities
         */
        public CourseController(GradingContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        /**
        * Inserts a NEW course into database.
        * @param CreateCourseViewModel model - database context
        * @return returns JSON Object when course information is correct; else returns an error
        */
        [Authorize(Roles ="Teacher")]
        [HttpPost("create")]
        public ActionResult<string> createCourse([FromBody] CreateCourseViewModel model)
        {
            try
            {
                Course newCourse = new Course();
                newCourse.CourseName = model.CourseName;
                newCourse.CourseTerm = model.CourseTerm;
                newCourse.CourseYear = model.CourseYear;
                newCourse.InstructorId = model.InstructorId;
                _context.Courses.Add(newCourse);
                _context.SaveChanges();
                return JsonConvert.SerializeObject("Created New Course");
            }
            catch (Exception e)
            {

            }
            return JsonConvert.SerializeObject("Error");
        }

        /**
        * Deletes an existing course from database.
        * @param int id - Course ID
        * @return returns JSON Object that confirms deletion; else returns an error
        */
        [HttpDelete("delete/{id}")]
        public ActionResult<string> deleteCourse(int id)
        {
            try
            {
                var course = _context.Courses.Where(c => c.CourseCrn == id).FirstOrDefault();
                var cg = _context.StudentGroup.Where(c => c.CourseCrn == id).ToList();
                var cs = _context.CourseStudents.Where(c => c.CourseCrn == id).ToList();
                var ce = _context.Evaluations.Where(c => c.CourseCrn == id).ToList();

                _context.StudentGroup.RemoveRange(cg);
                _context.CourseStudents.RemoveRange(cs);
                _context.Evaluations.RemoveRange(ce);
                _context.Courses.Remove(course);
                _context.SaveChanges();
                return JsonConvert.SerializeObject("Deleted ");
            }
            catch (Exception e)
            {

            }
            return JsonConvert.SerializeObject("Error");
        }

        /**
        * Gets a course by Course ID
        * @param int id - Course ID
        * @return returns JSON Object of specified course; else returns an error
        */
        [HttpGet("{id}")]
        public ActionResult<string> getCourse(int id)
        {
            try
            {
                var course = _context.Courses.Where(c => c.CourseCrn == id).FirstOrDefault();
                return JsonConvert.SerializeObject(course);
            }
            catch (Exception e)
            {

            }
            return JsonConvert.SerializeObject("Error");
        }

        /**
        * Edits an existing course in the database
        * @param Course model - Course to be edited
        * @param int id - Course ID
        * @return returns JSON Object upon successful update; else returns an error
        */
        [HttpPut("{id}")]
        public ActionResult<string> setCourseData([FromBody] Course model, [FromRoute] int id)
        {
            try
            {
                var course = _context.Courses.Where(c => c.CourseCrn == id).FirstOrDefault();
                course.CourseName = model.CourseName;
                course.CourseTerm = model.CourseTerm;
                course.CourseYear = model.CourseYear;
                course.InstructorId = model.InstructorId;
                _context.Courses.Update(course);
                _context.SaveChanges();
                return JsonConvert.SerializeObject("Success");
            }
            catch (Exception e)
            {

            }
            return JsonConvert.SerializeObject("Error");
        }

        /**
        * Gets all existing courses in the database
        * @return returns a list of courses as a JSON Object; else returns an error
        */
        [HttpGet]
        public ActionResult<string> getCourseData()
        {
            try
            {
                try
                {
                    var courses = _context.Courses.ToList();
                    return JsonConvert.SerializeObject(courses);
                }
                catch (Exception e)
                {
                    //
                }
                return JsonConvert.SerializeObject("error");
            }
            catch (Exception e)
            {
                //
            }
            return JsonConvert.SerializeObject("Error");
        }
		
		/**
        *  Gets all the courses the  specified instructor teaches
        */
       [HttpGet("i/{id}")]
       public ActionResult<string> getCoursesForInstructior([FromRoute] string id)
       {
           try
           {
               try
               {
                   var courses = _context.Courses.Where(c => c.InstructorId == id);
                   return JsonConvert.SerializeObject(courses);
               }
               catch (Exception e)
               {
                   //
               }
               return JsonConvert.SerializeObject("Error");
           }
           catch (Exception e) { }
           return JsonConvert.SerializeObject("Error");
       }



       /**
        *  Gets all the courses the specified student is currently taking.
        */
       [HttpGet("s/{id}")]
       public ActionResult<string> getCoursesForStudent([FromRoute] string id)
       {
           try
           {
               try
               {
                   var courseStudents = _context.CourseStudents.Where(s => s.StudentId == id).ToList();
                    var courses = _context.Courses.Where(c => c.CourseCrn == -666).ToList();
                   
                   for (int i = 0; i < courseStudents.Count(); i++)
                   {
                       courses.Add(_context.Courses.Where(c => c.CourseCrn == courseStudents[i].CourseCrn).FirstOrDefault());
                   }
                   return JsonConvert.SerializeObject(courses);
               }
               catch (Exception e)
               {
                   //
               }
               return JsonConvert.SerializeObject("Error1");
           }
           catch (Exception e) { }
           return JsonConvert.SerializeObject("Error");
       }
    }
}
