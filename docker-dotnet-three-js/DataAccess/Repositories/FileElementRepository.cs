using docker_dotnet_three_js.DataAccess.Contracts;
using docker_dotnet_three_js.DataAccess.DBContexts;
using docker_dotnet_three_js.DataAccess.Implementations.Entities;

namespace docker_dotnet_three_js.DataAccess.Repositories
{
    public class FileElementRepository(ApplicationContext context, IConfiguration configuration) : GenericRepository<FileElement>(context, configuration), IFileElementRepository
    {
    }
}