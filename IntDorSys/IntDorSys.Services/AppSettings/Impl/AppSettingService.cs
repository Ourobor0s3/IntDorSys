using IntDorSys.Core.Entities;
using IntDorSys.DataAccess;
using IntDorSys.Services.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IntDorSys.Services.AppSettings.Impl
{
    internal sealed class AppSettingService : IAppSettingService
    {
        private readonly AppDataContext _db;
        private readonly IAuditService _audit;
        private readonly ILogger<AppSettingService> _logger;

        public AppSettingService(AppDataContext db, IAuditService audit, ILogger<AppSettingService> logger)
        {
            _db = db;
            _audit = audit;
            _logger = logger;
        }

        public async Task<string?> GetValueAsync(string key, CancellationToken ct)
        {
            return await _db.Set<AppSetting>()
                .Where(x => x.Key == key)
                .Select(x => x.Value)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<AppSetting>> GetAllEditableAsync(CancellationToken ct)
        {
            return await _db.Set<AppSetting>()
                .Where(x => x.IsEditable)
                .OrderBy(x => x.Key)
                .ToListAsync(ct);
        }

        public async Task<AppSetting?> GetByIdAsync(long id, CancellationToken ct)
        {
            return await _db.Set<AppSetting>()
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task UpdateAsync(long id, string value, long userId, CancellationToken ct)
        {
            var setting = await _db.Set<AppSetting>()
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (setting == null || !setting.IsEditable)
                throw new InvalidOperationException("Setting not found or not editable");

            var oldValue = setting.Value;
            setting.Value = value;
            await _db.SaveChangesAsync(ct);

            try
            {
                var entityName = $"AppSetting:{setting.Key}";
                var details = $"Changed from: '{oldValue}' to: '{value}'";
                await _audit.RecordAsync(userId, "UpdateSetting", entityName,
                    setting.Id.ToString(), details);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to audit UpdateSetting for key={Key}", setting.Key);
            }
        }
    }
}