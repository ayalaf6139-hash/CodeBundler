using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeBundler
{
    public static class ResponseFileCreator
    {
        public static int Run()
        {
            var languages = Ask(
                $"אילו שפות לארוז? (מופרדות ברווח, או all). נתמכות: {string.Join(", ", Languages.Names)}",
                input =>
                {
                    var list = SplitWords(input);
                    return Languages.Validate(list);
                });

            var output = Ask("מה שם קובץ ה-bundle? (שם קובץ או נתיב מלא)", input =>
                string.IsNullOrWhiteSpace(input) || input.IndexOfAny(Path.GetInvalidPathChars()) >= 0
                    ? "יש להזין שם קובץ תקין."
                    : null);

            var note = AskYesNo("לרשום את מקור הקוד כהערה? (y/n)");

            var sort = Ask("סדר מיון: file-name או type? (Enter = file-name)", input =>
                input == "" || input == "file-name" || input == "type"
                    ? null
                    : "יש להזין file-name או type.");
            if (sort == "")
                sort = "file-name";

            var removeEmptyLines = AskYesNo("למחוק שורות ריקות? (y/n)");

            var author = Ask("שם יוצר הקובץ? (Enter לדילוג)", _ => null);

            var rspName = Ask("שם קובץ התגובה? (Enter = bundle.rsp)", input =>
                input.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ? "שם הקובץ מכיל תווים לא חוקיים." : null);
            if (rspName == "")
                rspName = "bundle.rsp";
            if (!rspName.EndsWith(".rsp", StringComparison.OrdinalIgnoreCase))
                rspName += ".rsp";

            var lines = new List<string>
            {
                "bundle",
                $"--language {string.Join(" ", SplitWords(languages))}",
                $"--output {Quote(output)}",
                $"--sort {sort}"
            };
            if (note)
                lines.Add("--note");
            if (removeEmptyLines)
                lines.Add("--remove-empty-lines");
            if (author != "")
                lines.Add($"--author {Quote(author)}");

            try
            {
                File.WriteAllLines(rspName, lines);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                Console.Error.WriteLine($"שגיאה: שמירת קובץ התגובה נכשלה: {e.Message}");
                return 1;
            }

            Console.WriteLine($"קובץ התגובה נוצר: {Path.GetFullPath(rspName)}");
            Console.WriteLine($"להרצה: cb @{rspName}");
            return 0;
        }

        private static string Ask(string question, Func<string, string?> validate)
        {
            while (true)
            {
                Console.WriteLine(question);
                var input = (Console.ReadLine() ?? "").Trim();
                var error = validate(input);
                if (error == null)
                    return input;
                Console.WriteLine($"קלט לא תקין: {error}");
            }
        }

        private static bool AskYesNo(string question)
        {
            var answer = Ask(question, input =>
                input.Equals("y", StringComparison.OrdinalIgnoreCase) || input.Equals("n", StringComparison.OrdinalIgnoreCase)
                    ? null
                    : "יש להזין y או n.");
            return answer.Equals("y", StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> SplitWords(string input) =>
            input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        private static string Quote(string value) =>
            value.Contains(' ') ? $"\"{value}\"" : value;
    }
}
