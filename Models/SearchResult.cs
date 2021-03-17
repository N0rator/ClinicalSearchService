using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicalSearchService.Models
{

	public class SearchResult
	{
		public string NPI { get; set; }

		public Dictionary<string, string> Fields { get; set; }

		public SearchResult()
		{
			Fields = new Dictionary<string, string>();
		}
	}
}
