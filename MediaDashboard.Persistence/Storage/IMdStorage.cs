namespace MediaDashboard.Persistence.Storage
{
    public interface IMdStorage
    {
        void Write(string fileName, string fileContent);
    }
}
