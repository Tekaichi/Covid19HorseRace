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
            string []horses = { "https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/120/samsung/220/horse-racing_1f3c7.png ", 
                "https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/120/google/241/horse_1f40e.png",
                "https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/120/whatsapp/238/horse_1f40e.png", 
                "https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/120/softbank/145/horse_1f40e.png",
            "https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/120/docomo/205/horse_1f40e.png",
            "https://emojipedia-us.s3.dualstack.us-west-1.amazonaws.com/thumbs/120/htc/37/horse_1f40e.png"};
            var countries = new List<Country>();
            var data = GetData();
           
            var table = Regex.Match(data, "<tbody>[\\s\\S]*?</tbody>");
            var rows = Regex.Matches(table.Value, "<tr .*?>[\\s\\S]*?</tr>");
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            foreach(var row in rows)
            {
                var cols = Regex.Matches(row.ToString(), "<td .*?>[\\s\\S]*?</td>");
                var name = Regex.Match(cols[0].Value, ">[A-Z].*?<");
                var totalcases = Regex.Match(cols[1].Value, "[\\d,]+");
                var active_cases = Regex.Match(cols[6].Value, "[\\d,]+");
                int new_cases;
                int deaths;
                try
                {
                    new_cases = int.Parse(Regex.Match(cols[2].Value, "[\\d,]+").Value.Replace(",", ""));
                }catch(Exception e)
                {
                    new_cases = 0;
                }

                try
                {
                    deaths = int.Parse(Regex.Match(cols[3].Value, "[\\d,]+").Value.Replace(",", ""));
                }catch(Exception e)
                {
                    deaths = 0;
                }
                var cases = new Cases()
                {
                    ActiveCases = int.Parse(active_cases.Value.Replace(",", "")),
                    TotalCases = int.Parse(totalcases.Value.Replace(",", "")),
                    NewCases = new_cases,
                    Deaths = deaths
                   
                };
                var country = new Country()
                {
                    Name = name.Value.Replace("<", "").Replace(">", ""),
                    Horse = horses[rand.Next(0, horses.Length - 1)],
                    Cases = cases
                };
                countries.Add(country);
         
            }
             
           

            var active_cases_sel = countries.Select(i => i.Cases.ActiveCases);
            var max = active_cases_sel.Max();

            const int max_distance = 80;
          
      

            var total_deaths = countries.Select(i => i.Cases.Deaths);

            var max_deaths = total_deaths.Max();
            foreach (var country in countries)
            {
                country.Distance = max_distance * country.Cases.ActiveCases / max;
                country.DeathDistance = max_distance * country.Cases.Deaths / max_deaths;
            }


          

            return countries.OrderByDescending(i => i.Cases.ActiveCases).ToList();
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
