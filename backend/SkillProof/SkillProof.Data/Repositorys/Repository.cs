using Microsoft.EntityFrameworkCore;
using SkillProof.Entities.Helper;

namespace SkillProof.Data.Repositorys
{
    public class Repository<T> : IRepository<T> where T : class, IIdentity
    {
        private readonly SkillProofDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(SkillProofDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> Create(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteById(string id)
        {
            var entity = await GetOne(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<T?> GetOne(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> Update(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
