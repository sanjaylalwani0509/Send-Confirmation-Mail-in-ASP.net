using Microsoft.AspNetCore.Mvc;
using MSU_BARODA.Data;
using MSU_BARODA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;

namespace MSU_BARODA.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private readonly List<string> allowedExtensions = new List<string> { ".pdf", ".docx", ".jpg", ".jpeg", ".png" };
        private const long MaxFileSize = 50 * 1024 * 1024; // 50MB
        private const long MinFileSize = 1 * 1024;         // 1KB

        public AssignmentController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var prn = HttpContext.Session.GetString("UserPRN");
            if (string.IsNullOrEmpty(prn)) return RedirectToAction("Login", "Account");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.PRN == prn);
            if (student == null) return RedirectToAction("Login", "Account");

            // 🧠 Fetch subjects based on student's course, department, and faculty
            var subjects = await _context.Subjects
                .Where(s => s.Faculty == student.Faculty &&
                            s.Department == student.Department &&
                            s.Course == student.Course)
                .Select(s => s.SubjectName)
                .ToListAsync();

            ViewBag.SubjectList = subjects;

            return View("SubmitAssignment");
        }



        [HttpPost]
        public async Task<IActionResult> SubmitAssignment(string subject, string fileNames, List<IFormFile> files)
        {
            var prn = HttpContext.Session.GetString("UserPRN");
            if (string.IsNullOrEmpty(prn)) return RedirectToAction("Login", "Account");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.PRN == prn);
            ViewBag.SubjectList = await _context.Subjects
                .Where(s => s.Faculty == student.Faculty &&
                            s.Department == student.Department &&
                            s.Course == student.Course)
                .Select(s => s.SubjectName)
                .ToListAsync();


            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(fileNames) || files.Count == 0)
            {
                ViewBag.Error = "⚠️ Please fill all fields and upload at least one file.";
                return View("SubmitAssignment");
            }

            if (files.Count > 5)
            {
                ViewBag.Error = "⚠️ You can upload a maximum of 5 files.";
                return View("SubmitAssignment");
            }

            var folderPath = Path.Combine(_environment.WebRootPath, "assignments", prn);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var fileNameArray = fileNames.Split(',').Select(f => f.Trim()).ToList();
            List<string> duplicateMessages = new();
            int uploadedCount = 0;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    duplicateMessages.Add($"⚠️ Invalid format: {file.FileName}. Allowed: pdf, docx, jpg, jpeg, png.");
                    continue;
                }

                if (file.Length < MinFileSize || file.Length > MaxFileSize)
                {
                    duplicateMessages.Add($"⚠️ File size for '{file.FileName}' must be 1KB to 50MB.");
                    continue;
                }

                var customFileName = i < fileNameArray.Count ? fileNameArray[i] : Path.GetFileNameWithoutExtension(file.FileName);
                var finalFileName = customFileName + extension;
                var filePath = Path.Combine(folderPath, finalFileName);

                bool fileExists = System.IO.File.Exists(filePath);
                bool dbExists = await _context.AssignmentSubmissions
                    .AnyAsync(s => s.PRN == prn && s.FileName == finalFileName && s.Subject == subject);

                if (fileExists || dbExists)
                {
                    duplicateMessages.Add($"⚠️ File is already present: {finalFileName}");
                    continue;
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var submission = new AssignmentSubmission
                {
                    PRN = prn,
                    Subject = subject,
                    FileName = finalFileName,
                    FilePath = Path.Combine("assignments", prn, finalFileName),
                    SubmittedAt = DateTime.Now
                };

                _context.AssignmentSubmissions.Add(submission);
                uploadedCount++;
            }

            await _context.SaveChangesAsync();

            if (uploadedCount == 0)
            {
                ViewBag.Error = string.Join("<br/>", duplicateMessages);
            }
            else
            {
                ViewBag.Success = $"✅ {uploadedCount} file(s) uploaded successfully.";
                if (duplicateMessages.Any())
                {
                    ViewBag.Success += "<br/>" + string.Join("<br/>", duplicateMessages);
                }
            }

            return View("SubmitAssignment");
        }

        public async Task<IActionResult> ViewSubmissions()
        {
            var prn = HttpContext.Session.GetString("UserPRN");
            if (string.IsNullOrEmpty(prn)) return RedirectToAction("Login", "Account");

            var submissions = await _context.AssignmentSubmissions
                .Where(a => a.PRN == prn)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            return View(submissions);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubmission(int id)
        {
            var submission = await _context.AssignmentSubmissions.FindAsync(id);
            if (submission == null) return NotFound();

            var fullPath = Path.Combine(_environment.WebRootPath, submission.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            _context.AssignmentSubmissions.Remove(submission);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "✅ Assignment deleted successfully!";
            return RedirectToAction("ViewSubmissions");
        }
    }
}
