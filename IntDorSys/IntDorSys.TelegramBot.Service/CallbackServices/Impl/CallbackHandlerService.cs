using IntDorSys.Core.Constants;
using IntDorSys.Core.Settings;
using IntDorSys.Services.Users;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.CallbackServices.Impl
{
    /// <inheritdoc />
    internal sealed class CallbackHandlerService : ICallbackHandlerService
    {
        private readonly IAdminCallbackHandler _adminHandler;
        private readonly ILaundressCallbackHandler _laundressHandler;
        private readonly IUserRoleService _userRoleService;
        private readonly IUserService _userService;
        private readonly IOptionsMonitor<AdminSettings> _adminSettings;

        /// <inheritdoc cref="ICallbackHandlerService" />
        public CallbackHandlerService(
            IUserService userService,
            IUserRoleService userRoleService,
            IAdminCallbackHandler adminHandler,
            ILaundressCallbackHandler laundressHandler,
            IOptionsMonitor<AdminSettings> adminSettings)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _adminHandler = adminHandler;
            _laundressHandler = laundressHandler;
            _adminSettings = adminSettings;
        }

        /// <inheritdoc />
        public async Task HandleAsync(CallbackQuery callbackQuery, CancellationToken ct = default)
        {
            var userInfo = (await _userService.GetByTgIdAsync(callbackQuery.From.Id, ct)).Data;
            var userRoles = (await _userRoleService.GetByIdAsync(userInfo.Id, ct)).Data;

            if (userRoles.Contains(UserRoleKeys.Admin) || _adminSettings.CurrentValue.AdminsChatId.Contains(userInfo.TelegramId))
            {
                await _adminHandler.HandleAsync(callbackQuery, ct);
            }

            if (userRoles.Contains(UserRoleKeys.Student))
            {
                await _laundressHandler.HandleAsync(callbackQuery, ct);
            }
        }
    }
}
