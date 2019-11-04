using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;

namespace HierarchyExp
{
    public class Context : DbContext
    {
        public Context() : base("name=efhdb")
        {

        }

        public Context(DbConnection connection): base(connection, true)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
