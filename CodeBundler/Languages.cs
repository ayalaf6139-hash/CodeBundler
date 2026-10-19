using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeBundler
{
    public static class Languages
    {
        public const string All = "all";

        // שם שפה -> סיומות הקבצים שלה
        public static readonly Dictionary<string, string[]> Extensions =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["csharp"] = new[] { ".cs" },
                ["cs"] = new[] { ".cs" },
                ["java"] = new[] { ".java" },
                ["python"] = new[] { ".py" },
                ["py"] = new[] { ".py" },
                ["javascript"] = new[] { ".js", ".jsx" },
                ["js"] = new[] { ".js", ".jsx" },
                ["typescript"] = new[] { ".ts", ".tsx" },
                ["ts"] = new[] { ".ts", ".tsx" },
                ["html"] = new[] { ".html", ".htm" },
                ["css"] = new[] { ".css", ".scss", ".sass", ".less" },
                ["c"] = new[] { ".c", ".h" },
                ["cpp"] = new[] { ".cpp", ".cc", ".cxx", ".hpp", ".h" },
                ["sql"] = new[] { ".sql" },
                ["go"] = new[] { ".go" },
                ["kotlin"] = new[] { ".kt", ".kts" },
                ["php"] = new[] { ".php" },
                ["ruby"] = new[] { ".rb" },
                ["swift"] = new[] { ".swift" },
                ["rust"] = new[] { ".rs" },
            };

        public static IEnumerable<string> Names => Extensions.Keys;

        public static HashSet<string> AllExtensions =>
            new HashSet<string>(Extensions.Values.SelectMany(e => e), StringComparer.OrdinalIgnoreCase);

        // מחזיר את הסיומות הנבחרות, או null אם נבחר all
        public static HashSet<string>? Resolve(IEnumerable<string> languages)
        {
            var list = languages.ToList();
            if (list.Any(l => l.Equals(All, StringComparison.OrdinalIgnoreCase)))
                return null;

            return new HashSet<string>(list.SelectMany(l => Extensions[l]), StringComparer.OrdinalIgnoreCase);
        }

        // מחזיר הודעת שגיאה, או null אם הרשימה תקינה
        public static string? Validate(IReadOnlyCollection<string> languages)
        {
            if (languages.Count == 0)
                return "יש לציין לפחות שפה אחת, או all.";

            var hasAll = languages.Any(l => l.Equals(All, StringComparison.OrdinalIgnoreCase));
            if (hasAll && languages.Count > 1)
                return "לא ניתן לשלב את all עם שפות נוספות.";

            var unknown = languages
                .Where(l => !l.Equals(All, StringComparison.OrdinalIgnoreCase) && !Extensions.ContainsKey(l))
                .ToList();
            if (unknown.Count > 0)
                return $"שפה לא מוכרת: {string.Join(", ", unknown)}. שפות נתמכות: all, {string.Join(", ", Names)}";

            return null;
        }
    }
}
