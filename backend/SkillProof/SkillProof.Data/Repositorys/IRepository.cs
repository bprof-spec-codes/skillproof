using SkillProof.Entities.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillProof.Data.Repositorys
{
    public interface IRepository<T> where T : class, IIdentity
    {
        Task<T> Create(T entity);
        Task DeleteById(string id);
        Task<T?> GetOne(string id);
        IQueryable<T> GetAll();
        Task<T> Update(T entity);
    }
}
