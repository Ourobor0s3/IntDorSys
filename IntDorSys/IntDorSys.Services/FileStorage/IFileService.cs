using IntDorSys.Core.Entities;
using Ouro.CommonUtils.Results;

namespace IntDorSys.Services.FileStorage
{
    /// <summary>
    ///     Provides file storage and retrieval by GUID
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        ///     Retrieves file metadata by its GUID
        /// </summary>
        /// <param name="id">File GUID</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>File info if found</returns>
        Task<DataResult<StoredFileInfo>> GetAsync(Guid id, CancellationToken ct);

        /// <summary>
        ///     Saves a file from a memory stream
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="stream">File content stream</param>
        /// <param name="groupId">Optional group identifier</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>GUID of the saved file</returns>
        Task<DataResult<Guid>> SaveAsync(
            string name,
            MemoryStream stream,
            string? groupId = null,
            CancellationToken ct = default);
    }
}