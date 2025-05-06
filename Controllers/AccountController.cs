using Microsoft.AspNetCore.Mvc;
using MSU_BARODA.Data;
using MSU_BARODA.Models;
using MSU_BARODA.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MSU_BARODA.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string PRN, string Password)
        {
            if (string.IsNullOrEmpty(PRN) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "PRN and Password are required.";
                return View();
            }

            var user = await _context.Students.FirstOrDefaultAsync(s => s.PRN == PRN);
            if (user == null || !VerifyPassword(Password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid PRN or password.";
                return View();
            }

            if (!user.EmailConfirmed)
            {
                ViewBag.Error = "Email not confirmed. Please check your inbox.";
                return View();
            }

            HttpContext.Session.SetString("UserPRN", user.PRN);
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Dashboard");
        }

       

        public IActionResult Register()
        {
            ViewBag.FacultyList = new[] { "Science", "Arts", "Commerce", "Engineering" };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Student model, string Password)
        {
            // ✅ Validate required fields
            if (string.IsNullOrWhiteSpace(model.FullName) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.ContactNo) ||
                string.IsNullOrWhiteSpace(model.Faculty) ||
                string.IsNullOrWhiteSpace(model.Department) ||
                string.IsNullOrWhiteSpace(model.Course) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "All fields are required.";
                ViewBag.FacultyList = new[] { "Science", "Arts", "Commerce", "Engineering" };
                return View();
            }

            // ✅ Contact number: Must be 10 digits only
            if (model.ContactNo.Length != 10 || !model.ContactNo.All(char.IsDigit))
            {
                ViewBag.Error = "Contact number must be exactly 10 digits.";
                ViewBag.FacultyList = new[] { "Science", "Arts", "Commerce", "Engineering" };
                return View();
            }

            // ✅ Check if email already exists
            var existingUser = await _context.Students.AnyAsync(s => s.Email == model.Email);
            if (existingUser)
            {
                ViewBag.Error = "This email is already registered.";
                ViewBag.FacultyList = new[] { "Science", "Arts", "Commerce", "Engineering" };
                return View();
            }

            // ✅ Generate PRN and store hashed password
            var prn = GeneratePRN();
            model.PRN = prn;
            model.PasswordHash = HashPassword(Password);
            model.EmailConfirmed = false;

            _context.Students.Add(model);
            await _context.SaveChangesAsync();

            // ✅ Send confirmation email
            await SendConfirmationEmail(model);

            return RedirectToAction("WaitingForConfirmation");
        }



        public IActionResult WaitingForConfirmation() => View();

        public async Task<IActionResult> ConfirmEmail(string prn)
        {
            var user = await _context.Students.FirstOrDefaultAsync(s => s.PRN == prn);
            if (user == null) return NotFound("User not found.");

            user.EmailConfirmed = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Dashboard()
        {
            var prn = HttpContext.Session.GetString("UserPRN");

            if (string.IsNullOrEmpty(prn))
                return RedirectToAction("Login");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.PRN == prn);
            if (student == null)
                return RedirectToAction("Login");

            return View(student);
        }


        
        
        private async Task SendConfirmationEmail(Student student)
        {
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { prn = student.PRN }, Request.Scheme);

            var emailBody = $@"
                <h2>Welcome to MSU BARODA</h2>
                <p>Your PRN number is: <strong>{student.PRN}</strong></p>
                <p>Click the link below to confirm your email and complete your registration:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you did not register, please ignore this email.</p>";

            await _emailService.SendEmailAsync(student.Email, "Confirm Your Email - MSU BARODA", emailBody, null, null);
        }

        private string GeneratePRN()
        {
            return "MSU" + new Random().Next(100000, 999999);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return HashPassword(enteredPassword) == storedHash;
        }
    }
}
