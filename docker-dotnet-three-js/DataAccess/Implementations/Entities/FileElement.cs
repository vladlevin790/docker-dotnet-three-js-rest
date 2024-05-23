using System.ComponentModel.DataAnnotations.Schema;

namespace docker_dotnet_three_js.DataAccess.Implementations.Entities
{
    [Table("file_element")]
    public class FileElement : BaseEntity
    {
        [Column("file_path")]
        public string file_path { get; set; }

        [Column("description")]
        public string description { get; set; }

        public FileElement()
        {
        }

        public FileElement(string path, string text)
        {
            file_path = path;
            description = text;
        }
    }
}
