using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    [Category("IntegrationTests")]
    public class ParcelJourneyTest
    {
        private string baseUrl;
        private HttpClient _httpClient;
        [SetUp]
        public void Setup()
        {
            baseUrl = TestContext.Parameters.Get("baseUrl", "http://localhost");
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseUrl)
            };
        }
        
        [Test]
        public async Task ParcelJourney()
        {

            //Submit Parcel
            StringContent content = new("{\n  \"weight\": 12,\n  \"recipient\": {\n    \"name\": \"Tommy Boy\",\n    \"street\": \"Mitterweg 4\",\n    \"postalCode\": \"1110\",\n    \"city\": \"Wien\",\n    \"country\": \"AT\"\n  },\n  \"sender\": {\n    \"name\": \"Beans\",\n    \"street\": \"Rautenweg 15\",\n    \"postalCode\": \"1220\",\n    \"city\": \"Wien\",\n    \"country\": \"AT\"\n  }\n}", Encoding.UTF8, "application/json");
            var submitresult = await _httpClient.PostAsync(baseUrl + "/parcel", content);
            var jsonBody = await submitresult.Content.ReadAsStringAsync();
            Console.WriteLine(jsonBody);
            string trackingId = JObject.Parse(jsonBody)["trackingId"].ToString();
            Console.WriteLine(trackingId);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, submitresult.StatusCode);
            Assert.IsNotEmpty(jsonBody);

            //Track Parcel
            var trackingResult = await _httpClient.GetAsync(baseUrl + "/parcel/" + trackingId);
            var trackingJsonBody = await trackingResult.Content.ReadAsStringAsync();
            Console.WriteLine(trackingJsonBody);
            var json = JObject.Parse(trackingJsonBody);
            var futureHops = json["futureHops"];
            Console.WriteLine(futureHops.ToString());
            var code = futureHops[0]["code"].ToString();
            Console.WriteLine(code);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, trackingResult.StatusCode);
            Assert.IsNotEmpty(trackingJsonBody);

            //Report Hop Arrival
            var reportArrivalResult = await _httpClient.PostAsync(
                baseUrl + "/parcel/" + trackingId + "/reportHop/" + code,
                new StringContent("", Encoding.UTF8, "application/json"));
            Console.WriteLine(reportArrivalResult);           
            Assert.AreEqual(System.Net.HttpStatusCode.OK, reportArrivalResult.StatusCode);


            //Track Parcel
            trackingResult = await _httpClient.GetAsync(baseUrl + "/parcel/" + trackingId);
            trackingJsonBody = await trackingResult.Content.ReadAsStringAsync();
            Console.WriteLine(trackingJsonBody);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, trackingResult.StatusCode);
            Assert.IsNotEmpty(trackingJsonBody);

            //Report Hop Delivery
            var reportDeliveryResult = await _httpClient.PostAsync(
                baseUrl + "/parcel/" + trackingId + "/reportDelivery",
                new StringContent("", Encoding.UTF8, "application/json"));
            Assert.AreEqual(System.Net.HttpStatusCode.OK, reportDeliveryResult.StatusCode);

            //Track Parcel
            trackingResult = await _httpClient.GetAsync(baseUrl + "/parcel/" + trackingId);
            trackingJsonBody = await trackingResult.Content.ReadAsStringAsync();
            Console.WriteLine(trackingJsonBody);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, trackingResult.StatusCode);
            Assert.IsNotEmpty(trackingJsonBody);
        }

        [Test]
        public async Task TransitionParcel()
        {
            string trackingId = "TRACKING5";
            StringContent content =  new("{\n  \"weight\": 12,\n  \"recipient\": {\n    \"name\": \"Tommy Boy\",\n    " +
                "\"street\": \"Mitterweg 4\",\n    \"postalCode\": \"1110\",\n    \"city\": \"Wien\",\n    \"country\": \"AT\"\n  }," +
                "\n  \"sender\": {\n    \"name\": \"Beans\",\n    \"street\": \"Rautenweg 15\",\n    \"postalCode\": \"1220\",\n   " +
                " \"city\": \"Wien\",\n    \"country\": \"AT\"\n  }\n}", Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync(baseUrl + "/parcel/" + trackingId, content);
            var jsonBody = await result.Content.ReadAsStringAsync();
            Console.WriteLine(jsonBody);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("application/json", result.Content.Headers.ContentType.MediaType);
            Assert.IsNotEmpty(jsonBody);

        }

        [Test]
        public async Task GetWarehouse()
        {
            string code = "WENB01";
            var result = await _httpClient.GetAsync(baseUrl + "/warehouse/" + code);
            var jsonBody = await result.Content.ReadAsStringAsync();
            Console.WriteLine(jsonBody);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("application/json", result.Content.Headers.ContentType.MediaType);
            Assert.IsNotEmpty(jsonBody);

        }

        [Test]
        public async Task ExportWarehouse()
        {
            var result = await _httpClient.GetAsync(baseUrl + "/warehouse/");
            var jsonBody = await result.Content.ReadAsStringAsync();
            Console.WriteLine(jsonBody);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual("application/json", result.Content.Headers.ContentType.MediaType);
            Assert.IsNotEmpty(jsonBody);

        }
        
    }
}
