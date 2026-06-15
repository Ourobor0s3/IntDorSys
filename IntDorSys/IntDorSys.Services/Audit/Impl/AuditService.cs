using IntDorSys.Core.Entities;
using IntDorSys.Core.Models;
using IntDorSys.DataAccess;
using IntDorSys.Services.Audit;
using Microsoft.EntityFrameworkCore;
using Ouro.CommonUtils.Base.Entities;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.Audit.Impl
{
    internal sealed class AuditService : IAuditService
    {
        private readonly AppDataContext _db;

        public AuditService(AppDataContext db)
        {
            _db = db;
        }

        public async Task RecordAsync(long userId, string action, string entityName, string? entityId = null, string? details = null, CancellationToken ct = default)
        {
            _db.Set<AuditLog>().Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
            });

            await _db.SaveChangesAsync(ct);
        }

        public async Task<DataResult<List<AuditLogModel>>> GetLogsAsync(int page = 1, int pageSize = 50, DateTime? startDate = null, DateTime? endDate = null, CancellationToken ct = default)
        {
            var result = new DataResult<List<AuditLogModel>>();

            var query = _db.Set<AuditLog>()
                .Include(x => x.User)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(x => x.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(x => x.CreatedAt <= endDate.Value.AddDays(1));

            query = query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var logs = await query.Select(x => new AuditLogModel
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User!.FullName ?? x.User.Username,
                Action = x.Action,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                Details = x.Details,
                CreatedAt = x.CreatedAt,
            }).ToListAsync(ct);

            return result.WithData(logs);
        }

        public async Task<DataResult<List<AuditLogModel>>> GetLogsAsync(BaseFilterModel filter, CancellationToken ct)
        {
            var page = (filter.Skip / Math.Max(filter.Take, 1)) + 1;
            var pageSize = filter.Take > 0 ? filter.Take : 50;
            DateTime? startDate = null, endDate = null;

            if (!string.IsNullOrEmpty(filter.StartDate) && DateTime.TryParse(filter.StartDate, out var sd))
                startDate = sd;
            if (!string.IsNullOrEmpty(filter.EndDate) && DateTime.TryParse(filter.EndDate, out var ed))
                endDate = ed;

            return await GetLogsAsync(page, pageSize, startDate, endDate, ct);
        }
    }
}