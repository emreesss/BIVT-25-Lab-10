using System.IO;

namespace Lab10.White
{
    public abstract class WhiteFileManager : MyFileManager, IWhiteSerializer
    {
        public WhiteFileManager(string name) : base(name) { }

        public WhiteFileManager(string name, string folder, string fileName, string ext = "txt")
            : base(name, folder, fileName, ext) { }

        public abstract void Serialize(Lab9.White.White obj);
        public abstract Lab9.White.White Deserialize();

        public override void EditFile(string content)
        {
            if (string.IsNullOrEmpty(FullPath))
                return;

            if (!File.Exists(FullPath))
                CreateFile();

            File.WriteAllText(FullPath, content ?? string.Empty);
        }

        public override void ChangeFileExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return;

            string oldPath = FullPath;
            string content = null;

            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                content = File.ReadAllText(oldPath);

            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                File.Delete(oldPath);

            ChangeFileFormat(extension);

            if (content != null && !string.IsNullOrEmpty(FullPath))
                File.WriteAllText(FullPath, content);
        }
    }
}