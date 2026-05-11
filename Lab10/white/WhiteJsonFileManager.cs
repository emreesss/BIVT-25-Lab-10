using System;
using System.IO;
using System.Text.Json;

namespace Lab10.White
{
    public class WhiteJsonFileManager : WhiteFileManager
    {
        public WhiteJsonFileManager(string name) : base(name)
        {
            ChangeFileFormat("json");
        }

        public WhiteJsonFileManager(string name, string folder, string fileName, string ext = "json")
            : base(name, folder, fileName, ext) { }

        private class DTO
        {
            public string TypeName { get; set; }
            public string Input { get; set; }
            public string[][] Codes { get; set; }
        }

        public override void Serialize(Lab9.White.White obj)
        {
            if (obj == null || string.IsNullOrEmpty(FullPath))
                return;

            var dto = new DTO
            {
                TypeName = obj.GetType().FullName,
                Input = obj.Input
            };

            if (obj is Lab9.White.Task3 t3)
            {
                var codes = ExtractCodes(t3);
                dto.Codes = codes;
            }

            if (!string.IsNullOrEmpty(FolderPath) && !Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            var options = new JsonSerializerOptions { WriteIndented = true };
            // сериализуем с конкретными именами, совместимыми с тестами (поле "Input")
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = true }))
            {
                writer.WriteStartObject();
                writer.WriteString("TypeName", dto.TypeName ?? string.Empty);
                writer.WriteString("Input", dto.Input ?? string.Empty);
                if (dto.Codes != null)
                {
                    writer.WriteStartArray("Codes");
                    foreach (var row in dto.Codes)
                    {
                        writer.WriteStartArray();
                        foreach (var v in row)
                            writer.WriteStringValue(v ?? string.Empty);
                        writer.WriteEndArray();
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();
            }

            File.WriteAllBytes(FullPath, ms.ToArray());
        }

        public override Lab9.White.White Deserialize()
        {
            if (string.IsNullOrEmpty(FullPath) || !File.Exists(FullPath))
                return null;

            string content = File.ReadAllText(FullPath);
            if (string.IsNullOrEmpty(content))
                return null;

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                string typeName = root.TryGetProperty("TypeName", out var tn) ? tn.GetString() : null;
                string input = root.TryGetProperty("Input", out var inp) ? inp.GetString() : string.Empty;

                string[,] codes = null;
                if (root.TryGetProperty("Codes", out var codesEl) && codesEl.ValueKind == JsonValueKind.Array)
                {
                    int len = codesEl.GetArrayLength();
                    codes = new string[len, 2];
                    int i = 0;
                    foreach (var row in codesEl.EnumerateArray())
                    {
                        int j = 0;
                        foreach (var v in row.EnumerateArray())
                        {
                            if (j < 2)
                                codes[i, j] = v.GetString();
                            j++;
                        }
                        i++;
                    }
                }

                return CreateInstance(typeName, input, codes);
            }
            catch
            {
                return null;
            }
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
            if (extension != "json")
                return;

            string oldPath = FullPath;
            string fileContent = null;
            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                fileContent = File.ReadAllText(oldPath);

            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                File.Delete(oldPath);

            ChangeFileFormat("json");

            if (fileContent != null && !string.IsNullOrEmpty(FullPath))
                File.WriteAllText(FullPath, fileContent);
        }

        private static string[][] ExtractCodes(Lab9.White.Task3 task)
        {
            var f = typeof(Lab9.White.Task3).GetField("_codeTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (f == null) return null;
            var arr = f.GetValue(task) as string[,];
            if (arr == null) return null;
            int rows = arr.GetLength(0);
            var result = new string[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new string[] { arr[i, 0], arr[i, 1] };
            }
            return result;
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