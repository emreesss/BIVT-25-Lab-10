using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab10.White
{
    public class WhiteTxtFileManager : WhiteFileManager
    {
        public WhiteTxtFileManager(string name) : base(name) { }

        public WhiteTxtFileManager(string name, string folder, string fileName, string ext = "txt")
            : base(name, folder, fileName, ext) { }

        public override void Serialize(Lab9.White.White obj)
        {
            if (obj == null || string.IsNullOrEmpty(FullPath))
                return;

            if (!string.IsNullOrEmpty(FolderPath) && !Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            var sb = new StringBuilder();
            sb.Append("TypeName=").Append(obj.GetType().FullName).Append('\n');
            sb.Append("Input=").Append(EscapeValue(obj.Input ?? string.Empty)).Append('\n');

            if (obj is Lab9.White.Task3 t3)
            {
                var codes = ExtractCodes(t3);
                if (codes != null)
                {
                    sb.Append("CodesCount=").Append(codes.GetLength(0)).Append('\n');
                    for (int i = 0; i < codes.GetLength(0); i++)
                    {
                        sb.Append("Code").Append(i).Append("=")
                          .Append(EscapeValue(codes[i, 0] ?? string.Empty))
                          .Append("|||")
                          .Append(EscapeValue(codes[i, 1] ?? string.Empty))
                          .Append('\n');
                    }
                }
            }

            File.WriteAllText(FullPath, sb.ToString());
        }

        public override Lab9.White.White Deserialize()
        {
            if (string.IsNullOrEmpty(FullPath) || !File.Exists(FullPath))
                return null;

            string content = File.ReadAllText(FullPath);
            if (string.IsNullOrEmpty(content))
                return null;

            var dict = new Dictionary<string, string>();
            var lines = content.Split('\n');
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                int eq = line.IndexOf('=');
                if (eq < 0) continue;
                string key = line.Substring(0, eq);
                string value = line.Substring(eq + 1);
                dict[key] = UnescapeValue(value);
            }

            if (!dict.TryGetValue("TypeName", out var typeName))
                return null;

            dict.TryGetValue("Input", out var input);
            input = input ?? string.Empty;

            string[,] codes = null;
            if (dict.TryGetValue("CodesCount", out var cntStr) && int.TryParse(cntStr, out int cnt))
            {
                codes = new string[cnt, 2];
                for (int i = 0; i < cnt; i++)
                {
                    if (dict.TryGetValue("Code" + i, out var pair))
                    {
                        var parts = pair.Split(new[] { "|||" }, StringSplitOptions.None);
                        codes[i, 0] = parts.Length > 0 ? parts[0] : string.Empty;
                        codes[i, 1] = parts.Length > 1 ? parts[1] : string.Empty;
                    }
                }
            }

            return CreateInstance(typeName, input, codes);
        }

        public override void EditFile(string content)
        {
            var obj = Deserialize();
            if (obj == null)
                return;

            obj.ChangeText(content);
            Serialize(obj);
        }

        public override void ChangeFileExtension(string extension)
        {
            if (extension != "txt")
                return;

            string oldPath = FullPath;
            string fileContent = null;
            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                fileContent = File.ReadAllText(oldPath);

            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                File.Delete(oldPath);

            ChangeFileFormat("txt");

            if (fileContent != null && !string.IsNullOrEmpty(FullPath))
                File.WriteAllText(FullPath, fileContent);
        }

        private static string EscapeValue(string v)
        {
            if (v == null) return string.Empty;
            return v.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "\\r");
        }

        private static string UnescapeValue(string v)
        {
            if (v == null) return string.Empty;
            var sb = new StringBuilder();
            for (int i = 0; i < v.Length; i++)
            {
                if (v[i] == '\\' && i + 1 < v.Length)
                {
                    char next = v[i + 1];
                    if (next == 'n') { sb.Append('\n'); i++; }
                    else if (next == 'r') { sb.Append('\r'); i++; }
                    else if (next == '\\') { sb.Append('\\'); i++; }
                    else sb.Append(v[i]);
                }
                else
                {
                    sb.Append(v[i]);
                }
            }
            return sb.ToString();
        }

        private static string[,] ExtractCodes(Lab9.White.Task3 task)
        {
            var f = typeof(Lab9.White.Task3).GetField("_codeTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (f == null) return null;
            return f.GetValue(task) as string[,];
        }

        private static Lab9.White.White CreateInstance(string typeName, string input, string[,] codes)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;

            if (typeName.EndsWith("Task1"))
                return new Lab9.White.Task1(input ?? string.Empty);
            if (typeName.EndsWith("Task2"))
                return new Lab9.White.Task2(input ?? string.Empty);
            if (typeName.EndsWith("Task3"))
            {
                var t3 = new Lab9.White.Task3(input ?? string.Empty, codes ?? new string[0, 2]);
                t3.Review();
                return t3;
            }
            if (typeName.EndsWith("Task4"))
                return new Lab9.White.Task4(input ?? string.Empty);

            return null;
        }
    }
}