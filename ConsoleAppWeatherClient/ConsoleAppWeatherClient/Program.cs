using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleAppWeatherClient
{ 
    class Program
    {
        static async Task Main(string[] args)
        {
            // Get the city name from the user input
            Console.Write("Enter the city name: ");
            string cityName = Console.ReadLine();

            // Get the city's latitude and longitude from the data source
            string latitude = "0", longitude = "0";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://simplemaps.com/static/data/country-cities/in/in.json");
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var cities = JsonSerializer.Deserialize<City[]>(json.Replace("ā","a"), options);
                var city = Array.Find(cities, c => c.city.ToLower() == cityName.ToLower());
                if (city != null)
                {
                    latitude = city.lat;
                    longitude = city.lng;
                }
                else
                {
                    Console.WriteLine("City not found!");
                    return;
                }
            }

            // Call the API endpoint with the latitude and longitude of the city
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var weather = JsonSerializer.Deserialize<Weather>(json, options);

                // Parse the JSON response and display the weather information to the user
                Console.WriteLine($"Temperature: {weather.current_weather.temperature}°C");
                Console.WriteLine($"Wind speed: {weather.current_weather.windspeed} m/s");
            }
        }
    }

    class City
    {
        public string city { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }

        public string country { get; set; }

        //public string iso2 { get; set; }
        //public string admin_name { get; set; }
        //public string capital { get; set; }

        //public int population { get; set; }

        //public int population_proper { get; set; }

        //      {
        //  "city": "Delhi", 
        //  "lat": "28.6600", 
        //  "lng": "77.2300", 
        //  "country": "India", 
        //  "iso2": "IN", 
        //  "admin_name": "Delhi", 
        //  "capital": "admin", 
        //  "population": "29617000", 
        //  "population_proper": "16753235"
        //}
    }

    class Weather
    {
        public current_weather current_weather
        {
            get;
            set;
        }
    }

    class current_weather
    {
        public decimal temperature { get; set; }
        public decimal windspeed { get; set; }
    }

}
