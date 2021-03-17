using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

using ClinicalSearchService.Models;
using System.Web;

namespace ClinicalSearchService.Services
{
	public interface IDataService
	{
		List<string> GetAvailableFields();

		List<SearchResult> GetSearchResults(SearchRequest request);

		public SearchResult GetAllDetails(string npi);
	}

	public class APIDataService : IDataService
	{
		private readonly string apiUrl = "https://clinicaltables.nlm.nih.gov/api/npi_idv/v3/search";

		public List<string> GetAvailableFields()
		{
			return new List<string>()
			{
				"NPI", "provider_type", "gender", "name.full", "addr_practice.full", "licenses", "licenses.taxonomy", "licenses.taxonomy.code",
				"licenses.taxonomy.grouping", "licenses.taxonomy.classification", "licenses.taxonomy.specialization", "licenses.medicare",
				"licenses.medicare.spc_code", "licenses.medicare.type", "name", "name.last", "name.first", "name.middle", "name.credential",
				"name.prefix", "name.suffix", "addr_practice", "addr_practice.line1", "addr_practice.line2", "addr_practice.city", "addr_practice.state",
				"addr_practice.zip", "addr_practice.phone", "addr_practice.fax", "addr_practice.country", "addr_mailing", "addr_mailing.full",
				"addr_mailing.line1", "addr_mailing.line2", "addr_mailing.city", "addr_mailing.state", "addr_mailing.zip", "addr_mailing.phone",
				"addr_mailing.fax", "addr_mailing.country", "name_other", "name_other.full", "name_other.last", "name_other.first", "name_other.middle",
				"name_other.credential", "name_other.prefix", "name_other.suffix", "other_ids", "other_ids.id", "other_ids.type", "other_ids.issuer",
				"other_ids.state", "misc", "misc.auth_official", "misc.auth_official.last", "misc.auth_official.first", "misc.auth_official.middle",
				"misc.auth_official.credential", "misc.auth_official.title", "misc.auth_official.prefix", "misc.auth_official.suffix",
				"misc.auth_official.phone", "misc.replacement_NPI", "misc.EIN", "misc.enumeration_date", "misc.last_update_date",
				"misc.is_sole_proprietor", "misc.is_org_subpart", "misc.parent_LBN", "misc.parent_TIN"
			};
		}

		public List<SearchResult> GetSearchResults(SearchRequest request)
		{
			var result = MakeGetRequest(request.SearchString, request.SearchFields);

			var npis = result[1];
			var displayFields = result[3];

			List<SearchResult> results = new List<SearchResult>();

			for(int index = 0; index < npis.GetArrayLength(); index++)
			{
				SearchResult sResult = new SearchResult();
				sResult.NPI = npis[index].GetString();

				for(int fieldIndex = 0; fieldIndex < displayFields[index].GetArrayLength(); fieldIndex++)
				{
					sResult.Fields[request.SearchFields.ElementAt(fieldIndex)] = displayFields[index][fieldIndex].GetString();
				}

				results.Add(sResult);
			}

			return results;
		}

		public SearchResult GetAllDetails(string npi)
		{
			var availableFields = GetAvailableFields();
			var result = MakeGetRequest(npi, new List<string>() { "NPI" }, availableFields);

			var displayFields = result[3];

			SearchResult sResult = new SearchResult();
			sResult.NPI = npi;

			for (int fieldIndex = 0; fieldIndex < displayFields[0].GetArrayLength(); fieldIndex++)
			{
				sResult.Fields[availableFields[fieldIndex]] = displayFields[0][fieldIndex].GetString();
			}


			return sResult;
		}

		JsonElement MakeGetRequest(string terms, IEnumerable<string> searchFields)
		{
			return MakeGetRequest(terms, searchFields, searchFields);
		}

		JsonElement MakeGetRequest(string terms, IEnumerable<string> searchFields, IEnumerable<string> displayFields)
		{
			using (var http = new HttpClient())
			{
				var builder = new UriBuilder(apiUrl);

				var query = HttpUtility.ParseQueryString(string.Empty);
				query["terms"] = terms;

				query["maxList"] = "10";

				query.Add("sf", string.Join(",", searchFields));
				query.Add("df", string.Join(",", displayFields));

				builder.Query = query.ToString();

				Uri uri = builder.Uri;

				using (var response = http.GetAsync(uri).Result)
				{
					var content = response.Content.ReadAsStringAsync().Result;

					return JsonSerializer.Deserialize<JsonElement>(content);
				}
			}
		}
	}
}
