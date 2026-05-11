namespace Lab10
{
    public interface IWhiteSerializer
    {
        void Serialize(Lab9.White.White obj);
        Lab9.White.White Deserialize();
    }
}
