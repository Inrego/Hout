namespace Hout.Models
{
    public class CustomElement
    {
        public CustomElement() { }
        public CustomElement(string path, string name)
        {
            Name = name;
            Path = path;
        }

        public CustomElement(string path)
        {
            Path = path;
            Name = path.Substring(path.LastIndexOf('/') + 1).Replace(".html", "");
        }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
