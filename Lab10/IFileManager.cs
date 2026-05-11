namespace Lab10
{
    public interface IFileManager
    {
        string FolderPath { get; }
        string FileName { get; }
        string FileExtension { get; }
        string FullPath { get; }

        void SelectFolder(string folder);
        void ChangeFileName(string fileName);
        void ChangeFileFormat(string extension);
    }
}
