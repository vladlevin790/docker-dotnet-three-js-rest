using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using docker_dotnet_three_js.DataAccess.Contracts;

namespace docker_dotnet_three_js.DataAccess.Implementations.Entities
{
    public class BaseEntity:IEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
    }
}