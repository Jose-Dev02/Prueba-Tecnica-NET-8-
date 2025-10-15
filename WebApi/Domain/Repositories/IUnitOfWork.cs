using System;
using System.Threading.Tasks;
using WebApi.Domain.Repositories;

namespace WebApi.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IPropertyRepository PropertyRepository { get; }
        Task<int> CompleteAsync();
    }
}