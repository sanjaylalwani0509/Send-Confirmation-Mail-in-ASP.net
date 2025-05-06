using System.Collections.Generic;

namespace MSU_BARODA.Helpers
{
    public static class StaticContent
    {
        public static List<string> GetFaculties() => new List<string>
        {
            "Engineering",
            "Science",
            "Arts",
            "Commerce"
        };

        public static List<string> GetDepartments(string faculty)
        {
            return faculty switch
            {
                "Engineering" => new List<string> { "CSE", "Electrical", "Civil", "Chemical" },
                "Science" => new List<string> { "Bio-Chemistry", "Botany", "Chemistry", "Environmental Science", "Mathematics", "Physics", "Computer Applications", "Medical Biotechnology", "Others" },
                "Arts" => new List<string> { "Department of English", "Department of Sindhi", "Department of Hindi", "Department of Gujarati" },
                "Commerce" => new List<string> { "Banking and Insurance", "Commerce" },
                _ => new List<string>()
            };
        }

        public static List<string> GetCourses(string faculty, string department)
        {
            if (faculty == "Engineering")
            {
                return department switch
                {
                    "CSE" => new List<string> { "Master of Computer Applications (MCA)", "Bachelor of Engineering (D to D students)", "Bachelor of Engineering (Regular Students)" },
                    "Electrical" => new List<string> { "Bachelor of Engineering (D to D students)", "Bachelor of Engineering (Regular Students)", "Master of Engineering" },
                    "Civil" => new List<string> { "Bachelor of Engineering (D to D students)", "Bachelor of Engineering (Regular Students)" },
                    "Chemical" => new List<string> { "Bachelor of Engineering (D to D students)", "Bachelor of Engineering (Regular Students)" },
                    _ => new List<string>()
                };
            }
            else if (faculty == "Science")
            {
                return department switch
                {
                    "Bio-Chemistry" => new List<string> { "B.Sc.", "M.Sc.", "M.Sc. in Bio-Chemistry" },
                    "Botany" => new List<string> { "B.Sc.", "M.Sc." },
                    "Chemistry" => new List<string> { "B.Sc.", "M.Sc." },
                    "Environmental Science" => new List<string> { "B.Sc.", "M.Sc." },
                    "Mathematics" => new List<string> { "B.Sc.", "M.Sc." },
                    "Physics" => new List<string> { "B.Sc.", "M.Sc." },
                    "Computer Applications" => new List<string> { "BCA (HPP)" },
                    "Medical Biotechnology" => new List<string> { "M.Sc. in Medical Biotechnology (HPP)" },
                    "Others" => new List<string> { "M.Phil.", "Ph.D." },
                    _ => new List<string>()
                };
            }
            else if (faculty == "Arts")
            {
                return department switch
                {
                    "Department of English" => new List<string> { "Bachelor of Arts (BA)", "Master of Arts (MA)" },
                    "Department of Sindhi" => new List<string> { "Bachelor of Arts (BA)", "Master of Arts (MA)" },
                    "Department of Hindi" => new List<string> { "Bachelor of Arts (BA)", "Master of Arts (MA)" },
                    "Department of Gujarati" => new List<string> { "Bachelor of Arts (BA)", "Master of Arts (MA)" },
                    _ => new List<string>()
                };
            }
            else if (faculty == "Commerce")
            {
                return department switch
                {
                    "Banking and Insurance" => new List<string> { "Under Graduate", "Post Graduate" },
                    "Commerce" => new List<string> { "Bachelor of Commerce", "Master of Commerce" },
                    _ => new List<string>()
                };
            }

            return new List<string>();
        }
    }
}
