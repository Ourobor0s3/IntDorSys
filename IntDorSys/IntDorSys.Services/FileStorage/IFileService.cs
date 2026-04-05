using Ouro.CommonUtils.Results;
using FileInfo = IntDorSys.Core.Entities.FileInfo;

namespace IntDorSys.Services.FileStorage
{
    public interface IFileService
    {
        /// <summary>
        ///     Get file info by guid
        /// </summary>
        /// <param name="id">Guid</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<FileInfo>> GetAsync(Guid id, CancellationToken ct);

        /// <summary>
        ///     Save file
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="stream">Memory stream</param>
        /// <param name="groupId">Group id</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        Task<DataResult<Guid>> SaveAsync(
            string name,
            MemoryStream stream,
            string? groupId = null,
            CancellationToken ct = default);
    }
}