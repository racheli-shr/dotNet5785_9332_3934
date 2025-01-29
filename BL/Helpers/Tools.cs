using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;
namespace BL.Helpers
{
    internal static class Tools
    {
        private const string ApiKey = "AIzaSyAfqbckIhPbc6rQkv7P2j711zLSfmnGxmo"; // הכניסי את ה-API Key שלך כאן
        private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
        public static (double Latitude, double Longitude) GetCoordinates(string address)
        {
            try
            {
                using (var client = new WebClient())
                {
                    // צור את כתובת ה-URL לשאילתה
                    string requestUrl = $"{BaseUrl}?address={Uri.EscapeDataString(address)}&key={ApiKey}";

                    // שלח את הבקשה והחזר את התשובה בצורה סינכרונית
                    string json = client.DownloadString(requestUrl);

                    var data = JObject.Parse(json);

                    if (data["status"].ToString() == "OK")
                    {
                        var location = data["results"][0]["geometry"]["location"];
                        double latitude = (double)location["lat"];
                        double longitude = (double)location["lng"];

                        return (latitude, longitude);
                    }
                    else
                    {
                        throw new Exception($"Error: {data["status"]}");
                    }

                }
            }
            catch (WebException webEx)
            {
                throw new Exception("Web error occurred: " + webEx.Message);
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות אחרות
                throw new Exception("An error occurred: invalid format address " + ex.Message);
            }
        }
    }
}

