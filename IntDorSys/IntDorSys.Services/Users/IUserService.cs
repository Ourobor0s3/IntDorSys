using IntDorSys.Core.Entities.Users;
using IntDorSys.Core.Enums;
using Ouro.CommonUtils.Results;
using Telegram.Bot.Types;

namespace IntDorSys.Services.Users
{
    /// <summary>
    ///     Combines user query and command services
    /// </summary>
    public interface IUserService : IUserQueryService, IUserCommandService
    {
    }
}