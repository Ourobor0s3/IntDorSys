using IntDorSys.Core.Constants;
using IntDorSys.Services.Users;
using Telegram.Bot.Types;

namespace IntDorSys.TelegramBot.Service.MessageServices.Impl
{
    internal sealed class MessageHandlerService : IMessageHandlerService
    {
        private readonly IAdminMessageHandler _adminHandler;

        private readonly ILaundressMessageHandler _laundressHandler;
        private readonly IUserRoleService _userRoleService;
        private readonly IUserQueryService _userService;
        private readonly IUsersMessageHandler _usersHandler;


        /// <inheritdoc cref="IMessageHandlerService" />
        public MessageHandlerService(
            IUserQueryService userService,
            IUserRoleService userRoleService,
            ILaundressMessageHandler laundressHandler,
            IUsersMessageHandler usersHandler,
            IAdminMessageHandler adminHandler)
        {
            _userService = userService;
            _userRoleService = userRoleService;
            _laundressHandler = laundressHandler;
            _usersHandler = usersHandler;
            _adminHandler = adminHandler;
        }

        /// <inheritdoc />
        public async Task HandleAsync(Message message, CancellationToken ct)
        {
            var userResult = await _userService.GetByTgIdAsync(message.From!.Id, ct);
            if (!userResult.IsSuccess) return;

            var userInfo = userResult.Data;
            var rolesResult = await _userRoleService.GetByIdAsync(userInfo.Id, ct);
            if (!rolesResult.IsSuccess) return;

            var userRoles = rolesResult.Data;

            if (userRoles.Contains(UserRoleKeys.Admin))
            {
                await _adminHandler.HandleAsync(message, ct);
            }

            if (userRoles.Contains(UserRoleKeys.Student))
            {
                await _usersHandler.HandleAsync(message, ct);
            }

            await _laundressHandler.HandleAsync(message, ct);
        }
    }
}