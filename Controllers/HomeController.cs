using ClinicalSearchService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using ClinicalSearchService.Services;
using System.Web;

namespace ClinicalSearchService.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IDataService _dataService;

		public HomeController(ILogger<HomeController> logger, IDataService dataService)
		{
			_logger = logger;
			_dataService = dataService;
		}

		public IActionResult Index()
		{
			ViewBag.AvailableFields = _dataService.GetAvailableFields();
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpPost]
		public IActionResult Index(SearchRequest searchRequest)
		{
			if(searchRequest.SearchFields.Count() == 0)
			{
				ModelState.AddModelError("SearchFields", "Search fields list is empty");
			}
			if (ModelState.IsValid)
			{
				var queryParams = HttpUtility.ParseQueryString("");

				queryParams.Add("SearchString", searchRequest.SearchString);

				foreach(var field in searchRequest.SearchFields)
				{
					queryParams.Add("SearchFields", field);
				}

				return Redirect("/Home/ViewResults?" + queryParams.ToString());
			}
			else
			{
				ViewBag.AvailableFields = _dataService.GetAvailableFields();
				return View(searchRequest);
			}

		}

		public IActionResult ViewResults(string SearchString, IEnumerable<string> SearchFields)
		{
			var searchRequest = new SearchRequest() { SearchString = SearchString, SearchFields = SearchFields };
			var results = _dataService.GetSearchResults(searchRequest);

			if(results.Count > 0)
			{
				ViewBag.SearchResults = results;
				return View();
			}
			else
			{
				ViewBag.SearchString = SearchString;
				ViewBag.SearchFields = SearchFields;
				return View("EmptyResults");
			}

			
		}

		public IActionResult ViewDetails(string npi)
		{
			var result = _dataService.GetAllDetails(npi);

			ViewBag.Details = result;
			return View();
		}

	}
}
