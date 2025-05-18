using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
//using Newtonsoft.Json.Linq;
namespace BL.Helpers
{
    internal static class Tools
    {
        private const string ApiKey = "AIzaSyAfqbckIhPbc6rQkv7P2j711zLSfmnGxmo"; // הכניסי את ה-API Key שלך כאן



        public static string ToStringProperty<T>(this T obj)
        {
            if (obj == null)
                return "null";

            Type type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var result = new StringBuilder();

            result.AppendLine($"{type.Name}:");
            result.AppendLine(new string('-', type.Name.Length));

            foreach (var property in properties)
            {
                try
                {
                    var value = property.GetValue(obj, null);

                    if (value is IEnumerable<object> collection)
                    {
                        result.AppendLine($"{property.Name}:");
                        foreach (var item in collection)
                        {
                            result.AppendLine($"  - {item}");
                        }
                    }
                    else
                    {
                        result.AppendLine($"{property.Name}: {value ?? "Not Provided"}");
                    }
                }
                catch
                {
                    result.AppendLine($"{property.Name}: <unable to retrieve>");
                }
            }

            return result.ToString();
        }

        private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
        #region check address
        public static (double Latitude, double longtitude) GetCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("הכתובת אינה תקינה");
            }

            string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key=679a8da6c01a6853187846vomb04142";

            try
            {
                using (WebClient client = new WebClient())
                {
                    string response = client.DownloadString(url);
                    //Console.WriteLine("Response from server: " + response); // הדפסה לבדיקה

                    var result = JsonSerializer.Deserialize<GeocodeResponse[]>(response);
                    Console.WriteLine(result.ToString()); // הדפסה לבדיקה



                    if (result == null || result.Length == 0)
                    {
                        throw new Exception("לא נמצאו קואורדינטות לכתובת זו");
                    }

                    double latitude = double.Parse(result[0].Latitude);
                    double longtitude = double.Parse(result[0].longtitude);

                    Console.WriteLine($"Adress was chose: {result[0].DisplayName}");
                    return (latitude, longtitude);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during getting coordinates: " + ex.Message);
            }
        }
        private class GeocodeResponse
        {
            [JsonPropertyName("lat")]
            public string Latitude { get; set; } // מוגדר כמחרוזת

            [JsonPropertyName("lon")]
            public string longtitude { get; set; } // מוגדר כמחרוזת

            [JsonPropertyName("display_name")]
            public string DisplayName { get; set; }
        }
        #endregion
    }
   

}

