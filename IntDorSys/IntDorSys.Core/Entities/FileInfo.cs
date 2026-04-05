using System.ComponentModel.DataAnnotations.Schema;
using Ouro.DatabaseUtils.Entities.Impl;

namespace IntDorSys.Core.Entities
{
    public class FileInfo : SoftDeletableEntity<long>
    {
        /// <summary>
        ///     Original file name
        /// </summary>
        required public string OriginalName { get; set; }

        /// <summary>
        ///     File name (guid + extension)
        /// </summary>
        required public string Name { get; set; }

        /// <summary>
        ///     File extension
        /// </summary>
        required public string Extension { get; set; }

        /// <summary>
        ///     File guid
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        ///     File size
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        ///     Group id
        /// </summary>
        public string? GroupId { get; set; }

        /// <summary>
        ///     File path
        /// </summary>
        [NotMapped]
        public string? FilePath { get; set; }

        /// <summary>
        ///     File content
        /// </summary>
        [NotMapped]
        public byte[]? Content { get; set; }
    }
}