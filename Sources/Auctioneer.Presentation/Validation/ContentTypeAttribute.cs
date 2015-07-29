using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace Auctioneer.Presentation.Validation
{
	public class ContentTypeAttribute : ValidationAttribute
	{
		public ContentTypeAttribute(string contentType)
		{
			Contract.Requires(!String.IsNullOrWhiteSpace(contentType));

			ContentType = contentType;
		}

		public override bool IsValid(object value)
		{
			var files = value as IEnumerable<HttpPostedFileBase>;
			if(files == null)
			{
				var file = value as HttpPostedFileBase;
				if(file == null)
					return false;

				files = new HttpPostedFileBase[] { file };
			}

			return files.All(file => file.ContentType == ContentType);
		}

		public string ContentType { get; private set;}

		public override bool RequiresValidationContext
		{
			get { return false; }
		}
	}
}