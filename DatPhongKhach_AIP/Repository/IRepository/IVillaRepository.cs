using DatPhongKhach_AIP.Models;
using System.Linq.Expressions;

namespace DatPhongKhach_AIP.Repository.IRepository
{
    public interface IVillaRepository : IRepository<Villa>
    {
        Task<Villa> UpdateAsync(Villa entity);

    }
}
