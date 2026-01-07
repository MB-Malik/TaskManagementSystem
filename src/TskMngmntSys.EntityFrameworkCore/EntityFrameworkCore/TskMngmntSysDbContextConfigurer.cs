using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace TskMngmntSys.EntityFrameworkCore
{
    public static class TskMngmntSysDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<TskMngmntSysDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<TskMngmntSysDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
