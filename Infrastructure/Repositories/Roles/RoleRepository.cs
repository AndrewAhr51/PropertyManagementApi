using PropertyManagementAPI.Domain.Entities.Roles;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MySqlDbContext _context;

        public RoleRepository(MySqlDbContext context)
        {
            _context = context;
        }

        // ✅ Create a new role
        public async Task<Role> AddAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        // ✅ Get a role by ID
        public async Task<Role> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        // ✅ Update role details
        public async Task<bool> UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✅ Delete a role by ID
        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return false;

            _context.Roles.Remove(role);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
