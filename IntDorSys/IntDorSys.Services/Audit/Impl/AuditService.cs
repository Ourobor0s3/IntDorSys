using IntDorSys.Core.Entities;
using IntDorSys.Core.Models;
using IntDorSys.DataAccess;
using IntDorSys.Services.Audit;
using Microsoft.EntityFrameworkCore;
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

        public async Task RecordAsync(long userId, string action, string entityName, string? entityId = null, string? details = null)
        {
            _db.Set<AuditLog>().Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
            });

            await _db.SaveChangesAsync();
        }

        public async Task<DataResult<List<AuditLogModel>>> GetLogsAsync(int page = 1, int pageSize = 50, CancellationToken ct = default)
        {
            var result = new DataResult<List<AuditLogModel>>();

            var query = _db.Set<AuditLog>()
                .Include(x => x.User)
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
    }
}