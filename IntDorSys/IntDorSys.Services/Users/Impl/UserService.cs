using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;

namespace IntDorSys.Services.Users.Impl
{
    internal sealed class UserService : IUserService
    {
        private readonly AppDataContext _db;

        public UserService(AppDataContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> GetByEmailAsync(string email, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var user = await _db.Set<UserInfo>()
                .SingleOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), ct);

            return user == null
                ? result.WithError("Not found")
                : result.WithData(user);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> GetByTgIdAsync(long id, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var userReg = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(u => u.TelegramId == id, ct);
            return userReg == null
                ? result.WithError("Not found")
                : result.WithData(userReg);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> GetByUserIdAsync(long id, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var userReg = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(u => u.Id == id, ct);
            return userReg == null
                ? result.WithError("Not found")
                : result.WithData(userReg);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> CreateOrUpdateTgInfoAsync(User user, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();
            var userInfo = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(u => u.TelegramId == user.Id, ct);

            if (userInfo == null)
            {
                userInfo = new UserInfo
                {
                    Username = GetUsername(user),
                    TelegramId = user.Id,
                    LanguageCode = user.LanguageCode,
                    IsBot = user.IsBot,
                    Email = "",
                    Password = "",
                };

                _db.AddOrUpdateEntity(userInfo);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                var hasChanged = false;
                if (GetUsername(user) != userInfo.Username)
                {
                    userInfo.Username = GetUsername(user);
                    hasChanged = true;
                }

                if (!userInfo.HasInitialDialog)
                {
                    userInfo.HasInitialDialog = true;
                    hasChanged = true;
                }

                if (hasChanged)
                {
                    _db.AddOrUpdateEntity(userInfo);
                    await _db.SaveChangesAsync(ct);
                }
            }

            return result.WithData(userInfo);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> UpdateUserInfo(UserInfo newInfo, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();
            var userInfo = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(u => u.TelegramId == newInfo.TelegramId, ct);
            if (userInfo == null)
            {
                return result.WithError("Not found");
            }

            userInfo.FullName = newInfo.FullName;
            userInfo.NumGroup = newInfo.NumGroup;
            userInfo.NumRoom = newInfo.NumRoom;

            _db.AddOrUpdateEntity(userInfo);
            await _db.SaveChangesAsync(ct);
            return result.WithData(userInfo);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> CreateAsync(UserInfo user, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var userReg = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(u => u.TelegramId == user.TelegramId, ct);

            if (userReg == null)
            {
                return result.WithError("Not found");
            }

            if (!string.IsNullOrEmpty(userReg.Email))
            {
                return result.WithError("Unable to create, use a different telegram");
            }

            var userExists = await _db.Set<UserInfo>()
                .AnyAsync(
                    u => u.Email.ToLower() == user.Email.ToLower(),
                    ct);

            if (userExists)
            {
                return result.WithError("Already exists");
            }

            userReg.Email = user.Email.ToLower();
            userReg.Password = user.Password;

            _db.AddOrUpdateEntity(userReg);
            await _db.SaveChangesAsync(ct);
            return result.WithData(user);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> GetAsync(long id, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var user = await _db.Set<UserInfo>()
                .SingleOrDefaultAsync(x => x.Id == id, ct);

            return user == null ? result.WithError("Not Found") : result.WithData(user);
        }

        /// <inheritdoc />
        public async Task<DataResult<List<UserInfo>>> GetListUsersAsync(CancellationToken ct)
        {
            var res = new DataResult<List<UserInfo>>();

            var users = await _db.Set<UserInfo>()
                .Where(x => x.FullName != "")
                .OrderBy(x => x.FullName)
                .ToListAsync(ct);

            return res.WithData(users);
        }

        public async Task<DataResult<bool>> ChangeUserStatus(long userId, UserStatus newStatus, CancellationToken ct)
        {
            var res = new DataResult<bool>();

            var user = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(x => x.Id == userId, ct);

            if (user == null)
            {
                return res.WithError("UserInfo not found");
            }

            user.Status = newStatus;

            _db.AddOrUpdateEntity(user);
            await _db.SaveChangesAsync(ct);
            return res.WithData(true);
        }

        private string GetUsername(User user)
        {
            return user.Username != null ? "@" + user.Username : $"{user.Id}";
        }
    }
}