using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Logic
{
	public class ObjectNotFoundException : LogicException
	{
		public ObjectNotFoundException(string message) : base(message)
		{
		}
	}
}
