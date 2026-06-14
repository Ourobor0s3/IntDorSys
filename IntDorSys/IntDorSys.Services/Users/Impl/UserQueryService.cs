using IntDorSys.Core.Entities.Users;
using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Users.Impl
{
    internal sealed class UserQueryService : IUserQueryService
    {
        private readonly AppDataContext _db;

        public UserQueryService(AppDataContext db)
        {
            _db = db;
        }

        public async Task<DataResult<UserInfo>> GetByEmailAsync(string email, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var normalizedEmail = email.ToUpper();
            var user = await _db.Set<UserInfo>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email.ToUpper() == normalizedEmail, ct);

            return user == null
                ? result.WithError("Not found")
                : result.WithData(user);
        }

        public async Task<DataResult<UserInfo>> GetByTgIdAsync(long id, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var userReg = await _db.Set<UserInfo>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TelegramId == id, ct);
            return userReg == null
                ? result.WithError("Not found")
                : result.WithData(userReg);
        }

        public async Task<DataResult<UserInfo>> GetByUserIdAsync(long id, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var userReg = await _db.Set<UserInfo>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, ct);
            return userReg == null
                ? result.WithError("Not found")
                : result.WithData(userReg);
        }

        public async Task<DataResult<UserInfo>> GetAsync(long id, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var user = await _db.Set<UserInfo>()
                .SingleOrDefaultAsync(x => x.Id == id, ct);

            return user == null ? result.WithError("Not found") : result.WithData(user);
        }

        public async Task<DataResult<List<UserInfo>>> GetListUsersAsync(CancellationToken ct)
        {
            var res = new DataResult<List<UserInfo>>();

            var users = await _db.Set<UserInfo>()
                .AsNoTracking()
                .Where(x => x.FullName != "")
                .OrderBy(x => x.FullName)
                .ToListAsync(ct);

            return res.WithData(users);
        }
    }
}
