namespace MediaDashboard.Persistence.Storage
{
    public static class MdStorage
    {
        public static IMdStorage Instance
        {
            get { return new BlobMdStorage(); }
        }
    }

    class BlobMdStorage : IMdStorage
    {
        public void Write(string fileName, string fileContent)
        {
            // later...
        }
    }
}
