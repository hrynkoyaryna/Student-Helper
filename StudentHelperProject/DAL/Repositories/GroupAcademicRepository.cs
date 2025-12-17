using Microsoft.EntityFrameworkCore;
using DAL.Interfaces;
using DAL.Models;
using DAL.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GroupAcademicRepository : BaseRepository<GroupAcademic>, IGroupAcademicRepository
    {
        public GroupAcademicRepository(AppDbContext context) : base(context) { }

        public async Task<GroupAcademic?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(g => g.Code == code);
        }

        public async Task<IEnumerable<GroupAcademic>> GetGroupsByFacultyAsync(string faculty)
        {
            return await _dbSet
                .Where(g => g.Faculty == faculty)
                .OrderBy(g => g.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupAcademic>> GetGroupsByYearAsync(int year)
        {
            return await _dbSet
                .Where(g => g.Year == year)
                .OrderBy(g => g.Code)
                .ToListAsync();
        }
    }
}