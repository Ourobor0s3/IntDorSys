using IntDorSys.Core.Constants;
using IntDorSys.Services.Users;
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

        /// <inheritdoc cref="ICallbackHandlerService" />
        public CallbackHandlerService(
            IUserService userService,
            IUserRoleService userRoleService,
            IAdminCallbackHandler adminHandler,
            ILaundressCallbackHandler laundressHandler)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _adminHandler = adminHandler;
            _laundressHandler = laundressHandler;
        }

        /// <inheritdoc />
        public async Task HandleAsync(CallbackQuery callbackQuery, CancellationToken ct = default)
        {
            var userInfo = (await _userService.GetByTgIdAsync(callbackQuery.From.Id, ct)).Data;
            var userRoles = (await _userRoleService.GetByIdAsync(userInfo.Id, ct)).Data;

            if (userRoles.Contains(UserRoleKeys.Admin) || AdminConstants.AdminsChatId.Contains(userInfo.TelegramId))
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