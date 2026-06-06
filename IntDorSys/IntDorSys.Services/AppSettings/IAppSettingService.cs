using IntDorSys.Core.Entities;

namespace IntDorSys.Services.AppSettings
{
    public interface IAppSettingService
    {
        Task<string?> GetValueAsync(string key, CancellationToken ct = default);

        Task<List<AppSetting>> GetAllEditableAsync(CancellationToken ct = default);

        Task<AppSetting?> GetByIdAsync(long id, CancellationToken ct = default);

        Task UpdateAsync(long id, string value, long userId, CancellationToken ct = default);
    }
}
