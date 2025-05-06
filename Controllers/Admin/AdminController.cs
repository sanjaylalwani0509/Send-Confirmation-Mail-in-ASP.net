using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MSU_BARODA.Models;
using MSU_BARODA.Data;
using MSU_BARODA.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using MSU_BARODA.Helpers;

namespace MSU_BARODA.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailServiceAdmin _emailService;

        public AdminController(ApplicationDbContext context, EmailServiceAdmin emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string adminID, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.AdminID == adminID && a.Password == password);

            if (admin == null)
            {
                ViewBag.Error = "Invalid Admin ID or Password!";
                return View();
            }

            HttpContext.Session.SetString("AdminID", admin.AdminID);
            HttpContext.Session.SetString("AdminName", admin.Name);

            return RedirectToAction("Dashboard");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Admin model)
        {
            if (model.SecretCode != "MSU_ADMIN")
            {
                ViewBag.Error = "Invalid Secret Code!";
                return View();
            }

            int lastId = _context.Admins.Count() + 1;
            model.AdminID = $"MSU_ADMIN_{lastId:D3}";

            _context.Admins.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { adminId = model.AdminID });
        }

        public IActionResult Confirmation(string adminId)
        {
            ViewBag.AdminID = adminId;
            return View();
        }

        public IActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
                return RedirectToAction("Login");

            ViewBag.AdminName = HttpContext.Session.GetString("AdminName");
            return View();
        }

        public IActionResult ManageUsers()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
                return RedirectToAction("Login");

            var students = _context.Students.ToList();

            ViewBag.Faculties = students.Select(s => s.Faculty).Where(f => !string.IsNullOrEmpty(f)).Distinct().ToList();
            ViewBag.Departments = students.Select(s => s.Department).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();
            ViewBag.Courses = students.Select(s => s.Course).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

            return View(students);
        }

        [HttpGet]
        public JsonResult GetDepartments(string faculty)
        {
            var departments = _context.Students
                .Where(s => s.Faculty == faculty)
                .Select(s => s.Department)
                .Distinct()
                .ToList();

            return Json(departments);
        }

        [HttpGet]
        public JsonResult GetCourses(string department)
        {
            var courses = _context.Students
                .Where(s => s.Department == department)
                .Select(s => s.Course)
                .Distinct()
                .ToList();

            return Json(courses);
        }

        [HttpGet]
        public IActionResult SearchUsers(string faculty, string department, string course, string searchTerm)
        {
            var students = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(faculty))
                students = students.Where(s => s.Faculty == faculty);

            if (!string.IsNullOrEmpty(department))
                students = students.Where(s => s.Department == department);

            if (!string.IsNullOrEmpty(course))
                students = students.Where(s => s.Course == course);

            if (!string.IsNullOrEmpty(searchTerm))
                students = students.Where(s => s.FullName.Contains(searchTerm) || s.PRN.Contains(searchTerm));

            return PartialView("_UserListPartial", students.ToList());
        }

        [HttpGet]
        public IActionResult EditUser(string id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
                return RedirectToAction("Login");

            var student = _context.Students.FirstOrDefault(s => s.PRN == id);
            if (student == null)
                return NotFound();

            // Static list of faculties
            var faculties = new List<string> { "Engineering", "Science", "Commerce", "Arts" };

            // Static list of departments by faculty
            var departments = new Dictionary<string, List<string>>
    {
        { "Engineering", new List<string> { "Computer Science", "Mechanical Engineering" } },
        { "Science", new List<string> { "Physics", "Mathematics" } },
        { "Commerce", new List<string> { "Business Administration" } },
        { "Arts", new List<string> { "English", "History" } }
    };

            // Static list of courses by department
            var courses = new Dictionary<string, List<string>>
    {
        { "Computer Science", new List<string> { "BTech", "MTech" } },
        { "Mechanical Engineering", new List<string> { "BTech", "MTech" } },
        { "Physics", new List<string> { "BSc", "MSc" } },
        { "Mathematics", new List<string> { "BSc", "MSc" } },
        { "Business Administration", new List<string> { "BBA", "MBA" } },
        { "English", new List<string> { "BA", "MA" } },
        { "History", new List<string> { "BA", "MA" } }
    };

            // Sending static lists to the view
            ViewBag.Faculties = faculties;
            ViewBag.Departments = departments;
            ViewBag.Courses = courses;

            // Set the selected faculty, department, and course based on the student's current data
            ViewBag.SelectedFaculty = student.Faculty;
            ViewBag.SelectedDepartment = student.Department;
            ViewBag.SelectedCourse = student.Course;

            return View(student);
        }


        // POST: Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(Student student)
        {
            // Check if Admin is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
                return RedirectToAction("Login");

            // Fetch the existing student data from the database
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.PRN == student.PRN);
            if (existingStudent == null)
                return NotFound();

            // Update the student's data
            existingStudent.FullName = student.FullName;
            existingStudent.Email = student.Email;
            existingStudent.ContactNo = student.ContactNo;
            existingStudent.Faculty = student.Faculty;
            existingStudent.Department = student.Department;
            existingStudent.Course = student.Course;

            // Save the updated student data
            await _context.SaveChangesAsync();

            // Redirect to the "ManageUsers" page or another appropriate action
            return RedirectToAction("ManageUsers");
        }

        public IActionResult DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
                return RedirectToAction("Login");

            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid user ID!");

            var student = _context.Students.FirstOrDefault(s => s.PRN == id);
            if (student == null)
                return NotFound("User not found!");

            _context.Students.Remove(student);
            _context.SaveChanges();

            return RedirectToAction("ManageUsers");
        }

        [HttpGet]
        public IActionResult SendEmail()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
                return RedirectToAction("Login");

            ViewBag.Faculties = _context.Students.Select(s => s.Faculty).Distinct().ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string faculty, string department, string course, string body, IFormFile attachment)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminID")))
                return RedirectToAction("Login");

            var studentsQuery = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(faculty))
            {
                studentsQuery = studentsQuery.Where(s => s.Faculty == faculty);
            }

            if (!string.IsNullOrEmpty(department))
            {
                studentsQuery = studentsQuery.Where(s => s.Department == department);
            }

            if (!string.IsNullOrEmpty(course))
            {
                studentsQuery = studentsQuery.Where(s => s.Course == course);
            }

            var students = await studentsQuery.Select(s => s.Email).ToListAsync();

            if (!students.Any())
            {
                ViewBag.Error = "No students found for the selected criteria!";
                ViewBag.Faculties = _context.Students.Select(s => s.Faculty).Distinct().ToList(); // ADD THIS
                return View();
            }

            byte[] fileData = null;
            string fileName = null;

            if (attachment != null && attachment.ContentType == "application/pdf")
            {
                using (var memoryStream = new MemoryStream())
                {
                    await attachment.CopyToAsync(memoryStream);
                    fileData = memoryStream.ToArray();
                    fileName = attachment.FileName;
                }
            }

            try
            {
                await _emailService.SendBulkEmailAsync(students, "Important Notification", body, fileName, fileData);
                ViewBag.Message = "Emails sent successfully!";
            }
            catch
            {
                ViewBag.Error = "An error occurred while sending emails.";
            }

            ViewBag.Faculties = _context.Students.Select(s => s.Faculty).Distinct().ToList(); // ADD THIS
            return View();
        }

        public async Task<IActionResult> ViewAllSubmissions()
        {
            var submissions = await _context.AssignmentSubmissions
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }
        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var submission = await _context.AssignmentSubmissions.FindAsync(id);
            if (submission == null || string.IsNullOrEmpty(submission.FilePath))
                return NotFound("File not found!");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", submission.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(path))
                return NotFound("File does not exist on server!");

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            var contentType = "application/octet-stream";
            return File(memory, contentType, Path.GetFileName(path));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubmission(int id)
        {
            var submission = await _context.AssignmentSubmissions.FindAsync(id);
            if (submission == null)
                return NotFound("Submission not found!");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", submission.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.AssignmentSubmissions.Remove(submission);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewAllSubmissions");
        }



        public IActionResult AdminLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        // ------------------------- MANAGE SUBJECTS -------------------------
        public IActionResult ManageSubjects(string faculty, string department, string course)
        {
            var subjects = _context.Subjects.AsQueryable();

            if (!string.IsNullOrEmpty(faculty))
                subjects = subjects.Where(s => s.Faculty == faculty);

            if (!string.IsNullOrEmpty(department))
                subjects = subjects.Where(s => s.Department == department);

            if (!string.IsNullOrEmpty(course))
                subjects = subjects.Where(s => s.Course == course);

            ViewBag.Faculties = StaticData.Faculties;
            ViewBag.SelectedFaculty = faculty;
            ViewBag.SelectedDepartment = department;
            ViewBag.SelectedCourse = course;

            return View(subjects.ToList());
        }

        // GET: AddSubject
        [HttpGet]
        public IActionResult AddSubject()
        {
            return View();
        }

        // POST: AddSubject
        [HttpPost]
        public IActionResult AddSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Add(subject);
                _context.SaveChanges();
                return RedirectToAction("ManageSubjects");
            }

            return View(subject);
        }

        // GET: EditSubject
        [HttpGet]
        public IActionResult EditSubject(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.SubjectId == id);
            if (subject == null)
                return NotFound();

            return View(subject);
        }

        // POST: EditSubject
        [HttpPost]
        public IActionResult EditSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Update(subject);
                _context.SaveChanges();
                return RedirectToAction("ManageSubjects");
            }

            return View(subject);
        }

        // POST: DeleteSubject
        [HttpPost]
        public IActionResult DeleteSubject(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.SubjectId == id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageSubjects");
        }

        // Cascading dropdown APIs
        [HttpGet]
        public JsonResult GetDepartmentsForSubjects(string faculty)
        {
            if (!string.IsNullOrEmpty(faculty) && StaticData.Faculties.ContainsKey(faculty))
            {
                var departments = StaticData.Faculties[faculty].Keys.ToList();
                return Json(departments);
            }

            return Json(new List<string>());
        }

        [HttpGet]
        public JsonResult GetCoursesForSubjects(string faculty, string department)
        {
            if (!string.IsNullOrEmpty(faculty) && !string.IsNullOrEmpty(department))
            {
                if (StaticData.Faculties.ContainsKey(faculty) &&
                    StaticData.Faculties[faculty].ContainsKey(department))
                {
                    var courses = StaticData.Faculties[faculty][department];
                    return Json(courses);
                }
            }

            return Json(new List<string>());
        }
    }
}
