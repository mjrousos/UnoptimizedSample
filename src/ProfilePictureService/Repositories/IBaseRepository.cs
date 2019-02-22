using System.Threading.Tasks;

namespace ProfilePictureService.Repositories
{
    interface IBaseRepository<T>
    {
        Task<T> Get(string name);
        Task Set(string name, T data);
        Task<bool> Delete(string name);
    }
}
