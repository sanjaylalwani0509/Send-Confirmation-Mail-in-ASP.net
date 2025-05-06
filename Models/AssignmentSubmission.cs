using System;
using System.ComponentModel.DataAnnotations;

namespace MSU_BARODA.Models
{
    public class AssignmentSubmission
    {
        public int Id { get; set; }

        [Required]
        public string PRN { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
