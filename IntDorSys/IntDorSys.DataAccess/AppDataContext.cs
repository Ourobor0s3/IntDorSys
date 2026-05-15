using IntDorSys.Core.Entities.Users;
using IntDorSys.Laundress.Core;
using Microsoft.EntityFrameworkCore;
using Ouro.DatabaseUtils.Conventions;
using Ouro.DatabaseUtils.DbContexts;
using Ouro.DatabaseUtils.Entities;
using Ouro.DatabaseUtils.Extensions;
using FileInfo = IntDorSys.Core.Entities.FileInfo;

namespace IntDorSys.DataAccess
{
    public sealed class AppDataContext : BaseDbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options)
            : base(options)
        {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            // configurationBuilder.Conventions.Add(_ => new DateTimeUtcConvertConvention());
            configurationBuilder.Conventions.Add(_ => new ForeignKeyOnDeleteActionConversion());
            configurationBuilder.Conventions.Add(_ => new NamePolicyConvention());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddPersistentEntity<UserInfo>(
                builder =>
                {
                    //Fields
                    builder.Property(x => x.FullName).HasMaxLength(100).IsRequired(false);
                    builder.Property(x => x.NumGroup).IsRequired(false);
                    builder.Property(x => x.NumRoom).IsRequired(false);
                    builder.Property(x => x.Email).HasMaxLength(50).IsRequired();
                    builder.Property(x => x.Password).HasMaxLength(100).IsRequired();
                    builder.Property(x => x.Status);
                    builder.Property(x => x.Address).IsRequired(false);
                    builder.Property(x => x.PhoneNumber).IsRequired(false);
                    builder.Property(x => x.Username).IsRequired();
                    builder.Property(x => x.TelegramId).IsRequired();
                    builder.Property(x => x.LanguageCode).HasMaxLength(5).IsRequired();
                    builder.Property(x => x.HasInitialDialog).IsRequired();
                    builder.Property(x => x.IsConfirm).IsRequired();
                    builder.Property(x => x.IsBot).IsRequired();

                    //Index
                    builder.HasIndex(x => x.TelegramId).IsUnique();
                });

            modelBuilder.AddPersistentEntity<UserRoles>(
                builder =>
                {
                    //Fields
                    builder.Property(x => x.KeyRoles).HasMaxLength(50).IsRequired();

                    builder.RequiredReference(x => x.User, x => x.UserId);
                });

            modelBuilder.AddPersistentEntity<FileInfo>(
                builder =>
                {
                    // Fields
                    builder.Property(x => x.OriginalName).IsRequired().HasMaxLength(100);
                    builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
                    builder.Property(x => x.Extension).IsRequired().HasMaxLength(100);
                    builder.Property(x => x.Guid).IsRequired().HasMaxLength(100);
                    builder.Property(x => x.Size);
                    builder.Property(x => x.GroupId);
                });

            modelBuilder.AddLaundressMap();
        }

        public void AddOrUpdateEntity<T>(T entity) where T : class, IEntity
        {
            if (entity.Id is long and > 0)
            {
                Set<T>().Update(entity);
            }
            else
            {
                Set<T>().Add(entity);
            }
        }

        public void DeleteEntity<T>(T entity) where T : class, ISoftDeletable
        {
            Set<T>().Remove(entity);
        }
    }
}