using System.Collections.Generic;

namespace MSU_BARODA.Helpers
{
    public static class StaticData
    {
        public static Dictionary<string, Dictionary<string, List<string>>> Faculties = new()
        {
            ["Engineering"] = new Dictionary<string, List<string>>
            {
                ["CSE"] = new List<string>
                {
                    "Master of Computer Applications (MCA)",
                    "Bachelor of Engineering (D to D students)",
                    "Bachelor of Engineering (Regular Students)"
                },
                ["Electrical"] = new List<string>
                {
                    "Bachelor of Engineering (D to D students)",
                    "Bachelor of Engineering (Regular Students)",
                    "Master of Engineering"
                },
                ["Civil"] = new List<string>
                {
                    "Bachelor of Engineering (D to D students)",
                    "Bachelor of Engineering (Regular Students)"
                },
                ["Chemical"] = new List<string>
                {
                    "Bachelor of Engineering (D to D students)",
                    "Bachelor of Engineering (Regular Students)"
                }
            },
            ["Science"] = new Dictionary<string, List<string>>
            {
                ["Bio-Chemistry"] = new List<string> { "B.Sc.", "M.Sc.", "M.Sc. in Bio-Chemistry" },
                ["Botany"] = new List<string> { "B.Sc.", "M.Sc." },
                ["Chemistry"] = new List<string> { "B.Sc.", "M.Sc." },
                ["Environmental Science"] = new List<string> { "B.Sc.", "M.Sc." },
                ["Mathematics"] = new List<string> { "B.Sc.", "M.Sc." },
                ["Physics"] = new List<string> { "B.Sc.", "M.Sc." },
                ["Computer Applications"] = new List<string> { "BCA (HPP)" },
                ["Medical Biotechnology"] = new List<string> { "M.Sc. in Medical Biotechnology (HPP)" },
                ["Others"] = new List<string> { "M.Phil.", "Ph.D." }
            },
            ["Arts"] = new Dictionary<string, List<string>>
            {
                ["Department of English"] = new List<string> { "Bachelor of Arts (BA)", "Master of Arts (MA)" },
                ["Department of Sindhi"] = new List<string> { "Bachelor of Arts (BA)", "Master of Arts (MA)" },
                ["Department of Hindi"] = new List<string> { "Bachelor of Arts (BA)", "Master of Arts (MA)" },
                ["Department of Gujarati"] = new List<string> { "Bachelor of Arts (BA)", "Master of Arts (MA)" }
            },
            ["Commerce"] = new Dictionary<string, List<string>>
            {
                ["Banking and Insurance"] = new List<string> { "Under Graduate", "Post Graduate" },
                ["Commerce"] = new List<string> { "Bachelor of Commerce", "Master of Commerce" }
            }
        };
    }
}
