using IntDorSys.Laundress.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Ouro.DatabaseUtils.Extensions;

namespace IntDorSys.Laundress.Core
{
    public static class LaundressCoreInstaller
    {
        public static void AddLaundressMap(this ModelBuilder modelBuilder)
        {
            modelBuilder.AddPersistentEntity<UseLaundress>(
                builder =>
                {
                    //Fields
                    builder.Property(x => x.TimeWash).IsRequired();
                    builder.Property(x => x.IsSendDay);
                    builder.Property(x => x.IsSendHours);

                    builder.RequiredReference(x => x.CreatedUser, x => x.CreatedUserId);
                    builder.OptionalReference(x => x.SelectUser, x => x.SelectUserId);

                    //Index
                    builder.HasIndex(x => x.TimeWash).IsUnique();
                });

            modelBuilder.AddPersistentEntity<Report>(
                builder =>
                {
                    //Fields
                    builder.Property(x => x.GroupId);
                    builder.Property(x => x.Description);

                    builder.RequiredReference(x => x.User, x => x.UserId);
                });
        }
    }
}