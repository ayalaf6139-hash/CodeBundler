using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeBundler
{
    public static class Bundler
    {
        // תיקיות שאין לכלול ב-bundle
        private static readonly HashSet<string> ExcludedDirectories =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "bin", "obj", "debug", "release", "node_modules", "packages",
                ".git", ".vs", ".vscode", ".idea", "x64", "x86", "dist", "build", "out"
            };

        public static int Run(string[] languages, string output, bool note, string sort,
            bool removeEmptyLines, string? author)
        {
            string outputPath;
            try
            {
                outputPath = Path.GetFullPath(output);
            }
            catch (Exception e) when (e is ArgumentException || e is NotSupportedException || e is PathTooLongException)
            {
                return Error($"נתיב הקובץ '{output}' אינו תקין.");
            }

            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (string.IsNullOrEmpty(outputDirectory) || !Directory.Exists(outputDirectory))
                return Error($"התיקיה '{outputDirectory}' לא קיימת. אם יש רווח בנתיב - יש לעטוף אותו במרכאות.");

            if (Directory.Exists(outputPath))
                return Error($"'{outputPath}' היא תיקיה. יש לציין שם קובץ.");

            var root = Directory.GetCurrentDirectory();
            var extensions = Languages.Resolve(languages) ?? Languages.AllExtensions;

            var files = CollectFiles(root)
                .Where(f => extensions.Contains(Path.GetExtension(f)))
                .Where(f => !string.Equals(Path.GetFullPath(f), outputPath, StringComparison.OrdinalIgnoreCase));

            files = sort == "type"
                ? files.OrderBy(f => Path.GetExtension(f), StringComparer.OrdinalIgnoreCase)
                       .ThenBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
                : files.OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase);

            var fileList = files.ToList();
            if (fileList.Count == 0)
                return Error("לא נמצאו קבצי קוד מתאימים בתיקיה הנוכחית.");

            var bundle = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(author))
            {
                bundle.AppendLine($"// Author: {author}");
                bundle.AppendLine();
            }

            foreach (var file in fileList)
            {
                if (note)
                    bundle.AppendLine($"// Source: {Path.GetRelativePath(root, file)}");

                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (removeEmptyLines && string.IsNullOrWhiteSpace(line))
                        continue;
                    bundle.AppendLine(line);
                }
                bundle.AppendLine();
            }

            try
            {
                File.WriteAllText(outputPath, bundle.ToString());
            }
            catch (UnauthorizedAccessException)
            {
                return Error($"אין הרשאה לשמור את הקובץ בנתיב '{outputPath}'.");
            }
            catch (IOException e)
            {
                return Error($"שמירת הקובץ '{outputPath}' נכשלה: {e.Message}");
            }

            Console.WriteLine($"נארזו {fileList.Count} קבצים לתוך {outputPath}");
            return 0;
        }

        private static IEnumerable<string> CollectFiles(string directory)
        {
            IEnumerable<string> files;
            IEnumerable<string> subDirectories;
            try
            {
                files = Directory.GetFiles(directory);
                subDirectories = Directory.GetDirectories(directory);
            }
            catch (UnauthorizedAccessException)
            {
                yield break;
            }

            foreach (var file in files)
                yield return file;

            foreach (var subDirectory in subDirectories)
            {
                if (ExcludedDirectories.Contains(Path.GetFileName(subDirectory)))
                    continue;
                foreach (var file in CollectFiles(subDirectory))
                    yield return file;
            }
        }

        private static int Error(string message)
        {
            Console.Error.WriteLine($"שגיאה: {message}");
            return 1;
        }
    }
}
