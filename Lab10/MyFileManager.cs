using System;
using System.IO;

namespace Lab10
{
    public abstract class MyFileManager : IFileManager, IFileLifeController
    {
        private string _name;
        private string _folderPath;
        private string _fileName;
        private string _fileExtension;

        public string Name => _name;
        public string FolderPath => _folderPath;
        public string FileName => _fileName;
        public string FileExtension => _fileExtension;

        public string FullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_folderPath) || string.IsNullOrEmpty(_fileName) || string.IsNullOrEmpty(_fileExtension))
                    return string.Empty;
                return Path.Combine(_folderPath, _fileName + "." + _fileExtension);
            }
        }

        public MyFileManager(string name)
        {
            _name = name ?? string.Empty;
            _folderPath = string.Empty;
            _fileName = string.Empty;
            _fileExtension = "txt";
        }

        public MyFileManager(string name, string folder, string fileName, string ext = "txt")
        {
            _name = name ?? string.Empty;
            _folderPath = folder ?? string.Empty;
            _fileName = fileName ?? string.Empty;
            _fileExtension = string.IsNullOrEmpty(ext) ? "txt" : ext;
        }

        public void SelectFolder(string folder)
        {
            _folderPath = folder ?? string.Empty;
            if (!string.IsNullOrEmpty(_folderPath) && !Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);
        }

        public void ChangeFileName(string fileName)
        {
            _fileName = fileName ?? string.Empty;
        }

        public void ChangeFileFormat(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return;
            _fileExtension = extension;
            CreateFile();
        }

        public void CreateFile()
        {
            if (string.IsNullOrEmpty(FullPath))
                return;

            if (!string.IsNullOrEmpty(_folderPath) && !Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);

            if (!File.Exists(FullPath))
                File.Create(FullPath).Dispose();
        }

        public void DeleteFile()
        {
            if (!string.IsNullOrEmpty(FullPath) && File.Exists(FullPath))
                File.Delete(FullPath);
        }

        public virtual void EditFile(string content)
        {
            if (string.IsNullOrEmpty(FullPath))
                return;

            if (!File.Exists(FullPath))
                CreateFile();

            File.WriteAllText(FullPath, content ?? string.Empty);
        }

        public virtual void ChangeFileExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return;

            string oldPath = FullPath;
            string content = null;

            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                content = File.ReadAllText(oldPath);

            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                File.Delete(oldPath);

            _fileExtension = extension;

            CreateFile();

            if (content != null && !string.IsNullOrEmpty(FullPath))
                File.WriteAllText(FullPath, content);
        }
    }
}