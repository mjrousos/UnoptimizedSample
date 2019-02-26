﻿using System.Threading.Tasks;

namespace ProfilePictureService.Repositories
{
    interface IBaseRepository<T>
    {
        Task<T> GetAsync(string name);
        Task SetAsync(string name, T data);
        Task<bool> DeleteAsync(string name);
    }
}
