using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic.Utils
{
	public static class IEnumerableAsyncExtensions
	{
		public static async Task<bool> Any<T>(this IEnumerable<T> collection, Func<T, Task<bool>> predicate)
		{
			foreach(var element in collection)
			{
				if(await predicate(element))
					return true;
			}

			return false;
		}

		public static async Task<bool> All<T>(this IEnumerable<T> collection, Func<T, Task<bool>> predicate)
		{
			foreach(var element in collection)
			{
				if(!await predicate(element))
					return false;
			}

			return true;
		}
	}
}
