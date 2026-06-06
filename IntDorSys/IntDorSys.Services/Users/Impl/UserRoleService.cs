using IntDorSys.Core.Entities.Users;
using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Users.Impl
{
    internal sealed class UserRoleService : IUserRoleService
    {
        private readonly AppDataContext _db;

        public UserRoleService(AppDataContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<DataResult<List<string>>> GetByIdAsync(long id, CancellationToken ct)
        {
            var result = new DataResult<List<string>>();

            var role = await _db.Set<UserRoles>()
                .Where(x => x.UserId == id)
                .Select(x => x.KeyRoles)
                .ToListAsync(ct);

            return result.WithData(role);
        }
    }
}