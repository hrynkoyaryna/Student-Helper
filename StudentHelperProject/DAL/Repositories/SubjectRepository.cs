using Microsoft.EntityFrameworkCore;
using DAL.Interfaces;
using DAL.Models;
using DAL.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(AppDbContext context) : base(context) { }

        public async Task<Subject?> GetByShortNameAsync(string shortName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.ShortName == shortName);
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByGroupAsync(int groupId)
        {
            return await _dbSet.ToListAsync();
        }
    }
}