using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Utils
{
	internal static class ExpressionExtensions
	{
		public static Expression<T> Negate<T>(this Expression<T> originalExpression)
		{
			Contract.Requires(originalExpression != null);

			var negatedExpression = Expression.Not(originalExpression.Body);

			return Expression.Lambda<T>(negatedExpression, originalExpression.Parameters);
		}
	}
}
