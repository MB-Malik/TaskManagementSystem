using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TskMngmntSys.Authorization.Roles;
using TskMngmntSys.Authorization.Users;
using TskMngmntSys.Entities.Task;
using TskMngmntSys.EntityFrameworkCore.QueryModels;
using TskMngmntSys.MultiTenancy;

namespace TskMngmntSys.EntityFrameworkCore
{
    public class TskMngmntSysDbContext : AbpZeroDbContext<Tenant, Role, User, TskMngmntSysDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<TaskItem> Tasks { get; set; }


        public TskMngmntSysDbContext(DbContextOptions<TskMngmntSysDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TaskProgressReport>().HasNoKey();

        }
    }
}
