using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Utils
{
	internal static class EntityFrameworkFluentApiExtensions
	{
		public static void IsIndex(this PrimitivePropertyConfiguration config)
		{
			config.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));
		}
	}
}
