﻿using System;
using System.Collections.Generic;

namespace Snippy.Data.Models
{
	public class Owner : DataModelBase
	{
		public string UserName { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public IList<OwnerUrls> OwnerURLs { get; set; }
	}
}
