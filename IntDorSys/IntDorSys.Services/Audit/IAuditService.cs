using IntDorSys.Core.Models;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Audit
{
    public interface IAuditService
    {
        Task RecordAsync(long userId, string action, string entityName, string? entityId = null, string? details = null);

        Task<DataResult<List<AuditLogModel>>> GetLogsAsync(int page = 1, int pageSize = 50, CancellationToken ct = default);
    }
}