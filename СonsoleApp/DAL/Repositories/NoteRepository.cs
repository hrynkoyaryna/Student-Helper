using Microsoft.EntityFrameworkCore;
using DAL.Interfaces;
using DAL.Models;
using DAL.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class NoteRepository : BaseRepository<Note>, INoteRepository
    {
        public NoteRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Note>> GetUserNotesAsync(int userId)
        {
            return await _dbSet
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Note>> GetPinnedNotesAsync(int userId)
        {
            return await _dbSet
                .Where(n => n.UserId == userId && n.IsPinned)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Note>> SearchNotesAsync(int userId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetUserNotesAsync(userId);
            }

            var term = searchTerm.ToLower();

            return await _dbSet
                .Where(n => n.UserId == userId && 
                           (n.Title.ToLower().Contains(term) || 
                            n.Body.ToLower().Contains(term)))
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync();
        }
    }
}