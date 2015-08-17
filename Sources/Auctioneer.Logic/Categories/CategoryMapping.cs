using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Auctioneer.Logic.Utils;

namespace Auctioneer.Logic.Categories
{
	internal class CategoryMapping : EntityTypeConfiguration<Category>
	{
		public CategoryMapping()
		{
			base.HasKey(x => x.Id);
			base.Property(x => x.Name).IsRequired();

			base.Property(x => x.Left).IsIndex();
			base.Property(x => x.Right).IsIndex();
		}
	}
}
