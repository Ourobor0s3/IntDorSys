using IntDorSys.Core.Models;
using Ouro.CommonUtils.Base.Entities;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Audit
{
    /// <summary>
    ///     Provides audit logging for admin actions across the system
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        ///     Records an audit log entry
        /// </summary>
        /// <param name="userId">ID of the user who performed the action</param>
        /// <param name="action">Action name (e.g. "CreateSlotRange", "DeleteSlot")</param>
        /// <param name="entityName">Entity type name (e.g. "UseLaundress", "UserInfo")</param>
        /// <param name="entityId">Optional entity identifier</param>
        /// <param name="details">Optional additional details</param>
        Task RecordAsync(long userId, string action, string entityName, string? entityId = null, string? details = null, CancellationToken ct = default);

        /// <summary>
        ///     Returns paginated audit log entries, optionally filtered by date range
        /// </summary>
        /// <param name="page">Page number (1-based)</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="startDate">Optional start date filter (inclusive)</param>
        /// <param name="endDate">Optional end date filter (inclusive)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Paginated list of audit log models</returns>
        Task<DataResult<List<AuditLogModel>>> GetLogsAsync(int page = 1, int pageSize = 50, DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default);

        /// <summary>
        ///     Returns paginated audit log entries from a BaseFilterModel (handles string-to-DateTime parsing internally)
        /// </summary>
        /// <param name="filter">Filter with skip/take/startDate/endDate as strings</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Paginated list of audit log models</returns>
        Task<DataResult<List<AuditLogModel>>> GetLogsAsync(BaseFilterModel filter, CancellationToken ct);
    }
}