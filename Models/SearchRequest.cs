using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicalSearchService.Models
{
	public class SearchRequest
	{
		[Required(ErrorMessage = "Search string not specified")]
		public string SearchString { get; set; }

		[Required(ErrorMessage = "Search fields not specified")]
		public IEnumerable<string> SearchFields { get; set; }

		public SearchRequest()
		{
			SearchFields = new List<string>();
		}
	}
}
