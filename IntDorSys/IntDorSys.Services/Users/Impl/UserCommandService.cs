using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using IntDorSys.DataAccess;
using Microsoft.EntityFrameworkCore;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;
using Ouro.DatabaseUtils.Extensions;

namespace IntDorSys.Services.Users.Impl
{
    internal sealed class UserCommandService : IUserCommandService
    {
        private readonly AppDataContext _db;

        public UserCommandService(AppDataContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> CreateAsync(UserInfo user, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            if (user.TelegramId == 0)
            {
                var normalizedEmail = user.Email.ToUpper();
                var existingUser = await _db.Set<UserInfo>()
                    .FirstOrDefaultAsync(u => u.Email.ToUpper() == normalizedEmail, ct);

                if (existingUser != null)
                {
                    return result.WithError("Email already registered");
                }

                user.Username = user.Email.Split('@')[0];
                _db.AddOrUpdateEntity(user);
                await _db.SaveChangesAsync(ct);
                return result.WithData(user);
            }

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

            var userEmail = user.Email.ToUpper();
            var userExists = await _db.Set<UserInfo>()
                .AnyAsync(
                    u => u.Email.ToUpper() == userEmail,
                    ct);

            if (userExists)
            {
                return result.WithError("Already exists");
            }

            userReg.Email = user.Email.ToLower();
            userReg.Password = user.Password;

            await _db.SaveChangesAsync(ct);
            return result.WithData(userReg);
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
                    Password = "TelegramUser.NoPassword",
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
                .FirstOrDefaultAsync(u => u.Id == newInfo.Id, ct);
            if (userInfo == null)
            {
                return result.WithError("Not found");
            }

            userInfo.FullName = newInfo.FullName;
            userInfo.NumGroup = newInfo.NumGroup;
            userInfo.NumRoom = newInfo.NumRoom;

            await _db.SaveChangesAsync(ct);
            return result.WithData(userInfo);
        }

        /// <inheritdoc />
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

            await _db.SaveChangesAsync(ct);
            return res.WithData(true);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> UpdatePasswordAsync(long userId, string passwordHash, CancellationToken ct)
        {
            var res = new DataResult<UserInfo>();

            var user = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(x => x.Id == userId, ct);

            if (user == null)
            {
                return res.WithError("User not found");
            }

            user.Password = passwordHash;

            await _db.SaveChangesAsync(ct);
            return res.WithData(user);
        }

        /// <inheritdoc />
        public async Task<DataResult<UserInfo>> ConfirmUserWithRoleAsync(long userId, string roleKey, CancellationToken ct)
        {
            var result = new DataResult<UserInfo>();

            var user = await _db.Set<UserInfo>()
                .FirstOrDefaultAsync(x => x.Id == userId, ct);

            if (user == null)
            {
                return result.WithError("User not found");
            }

            var existingRole = await _db.Set<UserRoles>()
                .FirstOrDefaultAsync(x => x.KeyRoles == roleKey && x.UserId == userId, ct);

            if (existingRole == null)
            {
                var userRole = new UserRoles
                {
                    UserId = userId,
                    User = user,
                    KeyRoles = roleKey,
                };
                _db.AddOrUpdateEntity(userRole);
            }

            user.IsConfirm = true;
            await _db.SaveChangesAsync(ct);
            return result.WithData(user);
        }

        private static string GetUsername(User user)
        {
            return user.Username != null ? "@" + user.Username : $"{user.Id}";
        }
    }
}
