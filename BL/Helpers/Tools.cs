using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using DalApi;
//using Newtonsoft.Json.Linq;
namespace BL.Helpers
{
    internal static class Tools
    {
        private static IDal s_dal = Factory.Get; //stage 4
        private const string ApiKey = "AIzaSyAfqbckIhPbc6rQkv7P2j711zLSfmnGxmo"; // הכניסי את ה-API Key שלך כאן


        #region distance
        /// <summary>
        /// Calculates the distance in kilometers between a tutor and a student call location.
        /// </summary>
        /// <param name="volunteerId">The ID of the tutor.</param>
        /// <param name="callLat">The latitude of the student call location.</param>
        /// <param name="callLong">The longitude of the student call location.</param>
        /// <returns>The distance in kilometers between the tutor and the student call location.</returns>
        internal static double? CalculateDistance(int volunteerId, double? callLat, double? callLong)
        {
            var volunteer = s_dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
                throw new BO.Exceptions.BlDoesNotExistException($"Tutor with ID {volunteerId} not found");

            return GetDistance(volunteer.Latitude, volunteer.longtitude, callLat, callLong);
        }


        /// <summary>
        /// Calculates the distance between two geographical coordinates.
        /// </summary>
        /// <param name="lat1">Latitude of the first point.</param>
        /// <param name="lon1">Longitude of the first point.</param>
        /// <param name="lat2">Latitude of the second point.</param>
        /// <param name="lon2">Longitude of the second point.</param>
        /// <returns>The distance between the two points in kilometers.</returns>
        private static double GetDistance(double? lat1, double? lon1, double? lat2, double? lon2)
        {
            // Haversine formula to calculate distance between two coordinates on Earth.
            double earthRadiusKm = 6371;
            double dLat = DegreesToRadians((double)lat2 - (double)lat1);
            double dLon = DegreesToRadians((double)lon2 - (double)lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians((double)lat1)) * Math.Cos(DegreesToRadians((double)lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        #endregion


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
     
        private class LocationIqResponse
        {
            [JsonPropertyName("lat")]
            public string Latitude { get; set; }

            [JsonPropertyName("lon")]
            public string Longitude { get; set; }
        }
        // מפתח API עבור שליפת קואורדינטות
        private static readonly HttpClient HttpClient = new HttpClient();


        #region check address
        /// <summary>
        /// Retrieves the geographical coordinates (latitude and longitude) for a given address.
        /// </summary>
        /// <param name="address">The address to get the coordinates for.</param>
        /// <returns>A tuple containing the latitude and longitude of the address.</returns>
        /// <exception cref="BO.BlValidationException">Thrown when the address is invalid.</exception>
        public static async Task<(double Latitude, double Longitude)> GetCoordinatesFromAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new BO.Exceptions.BLInvalidDataException("The address is invalid.");
            }

            string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key=679a8da6c01a6853187846vomb04142";

            try
            {
                using (WebClient client = new WebClient())
                {
                    string response = client.DownloadString(url);

                    var result = JsonSerializer.Deserialize<LocationIqResponse[]>(response);

                    if (result == null || result.Length == 0)
                    {
                        throw new BO.Exceptions.BLInvalidDataException("The address is invalid.");
                    }

                    double latitude = double.Parse(result[0].Latitude);
                    double longitude = double.Parse(result[0].Longitude);

                    return (latitude, longitude);
                }
            }
            catch (Exception ex)
            {
                throw new BO.Exceptions.BLInvalidDataException("Error retrieving coordinates" + ex.Message);
            }
        }

        //public static async Task<(double Latitude, double Longitude)> GetCoordinatesFromAddressAsync(string address)
        //{
        //    if (string.IsNullOrWhiteSpace(address))
        //        throw new BO.Exceptions.BLInvalidDataException("הכתובת אינה תקינה.");

        //    string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key=...";

        //    try
        //    {
        //        HttpResponseMessage response = await HttpClient.GetAsync(url);
        //        response.EnsureSuccessStatusCode();

        //        string jsonResponse = await response.Content.ReadAsStringAsync();

        //        var results = JsonSerializer.Deserialize<LocationIqResponse[]>(jsonResponse, new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        });

        //        if (results == null || results.Length == 0)
        //            throw new BO.Exceptions.BlDoesNotExistException("לא נמצאו קואורדינטות לכתובת זו.");

        //        if (!double.TryParse(results[0].Latitude, out double latitude) ||
        //            !double.TryParse(results[0].Longitude, out double longitude))
        //            throw new BO.Exceptions.BLGeneralException("שגיאה בהמרת קואורדינטות.");

        //        Console.WriteLine($"Latitude: {latitude}, Longitude: {longitude}");
        //        return (latitude, longitude);
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        throw new BO.Exceptions.BLGeneralException("שגיאה בבקשת הרשת: " + httpEx.Message);
        //    }
        //    catch (JsonException jsonEx)
        //    {
        //        throw new BO.Exceptions.BLGeneralException("שגיאה בפענוח JSON: " + jsonEx.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BO.Exceptions.BLGeneralException("שגיאה בעת שליפת קואורדינטות: " + ex.Message);
        //    }
        //}


        #endregion



    }


}

#endregion