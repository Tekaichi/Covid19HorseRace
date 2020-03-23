using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using CoronaRace.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CoronaRace.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IList<Country> countries { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;

        }

        public void OnGet()
        {
            countries = getCountries();

        }

        private List<Country> getCountries()
        {
            var countries = new List<Country>();
            var data = GetData();
            var table = Regex.Match(data, "<tbody>[\\s\\S]*?</tbody>");
            var rows = Regex.Matches(table.Value, "<tr .*?>[\\s\\S]*?</tr>");
            foreach(var row in rows)
            {
                var cols = Regex.Matches(row.ToString(), "<td .*?>[\\s\\S]*?</td>");
                var name = Regex.Match(cols[0].Value, ">[A-Z].*?<");
                var activecases = Regex.Match(cols[1].Value, "[\\d,]+");
                var cases = new Cases()
                {
                    ActiveCases = int.Parse(activecases.Value.Replace(",", ""))
                };
                var country = new Country()
                {
                    Name = name.Value.Replace("<", "").Replace(">", ""),

                    Cases = cases
                };
                countries.Add(country);
         
            }
            
            var active_cases = countries.Select(i => i.Cases.ActiveCases);

            var sum = active_cases.Sum() + 5;
            var avg = active_cases.Average() + 5;
            var standardDeviation = Math.Sqrt((sum) / (active_cases.Count() - 1));
            foreach(var country in countries)
            {
                country.Distance = (int)((country.Cases.ActiveCases - avg) / standardDeviation);
            }
            return countries;
        }

        private string  GetData()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.worldometers.info/coronavirus/");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
                return data;
            }
            return "";
        }
    }
}
