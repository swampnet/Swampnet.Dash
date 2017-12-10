using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swampnet.Dash.DAL
{
	class HistoryContext : DbContext
	{
		public DbSet<HistoryRoot> Roots { get; set; }

		public HistoryContext()
			: base("history")
		{

		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.HasDefaultSchema("dash");

			// HistoryRoot
			modelBuilder.Entity<HistoryRoot>().ToTable("HistoryRoot");
			modelBuilder.Entity<HistoryRoot>().Property(f => f.Name).HasMaxLength(100).IsRequired();

			// History
			modelBuilder.Entity<History>().ToTable("History");
			modelBuilder.Entity<History>().Property(f => f.Name).HasMaxLength(100).IsRequired();
			modelBuilder.Entity<History>().Property(f => f.Value).HasMaxLength(100).IsRequired();
		}
	}
}
