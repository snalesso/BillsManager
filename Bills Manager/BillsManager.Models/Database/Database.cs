namespace BillsManager.Models
{
    public class Database // TODO: re-evaluate the idea
    {
        public Database(string path)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public string Path { get; private set; }

        public string Name { get; private set; }
    }
}