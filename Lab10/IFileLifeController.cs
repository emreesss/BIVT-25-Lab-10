namespace Lab10
{
    public interface IFileLifeController
    {
        void CreateFile();
        void DeleteFile();
        void EditFile(string content);
        void ChangeFileExtension(string extension);
    }
}
