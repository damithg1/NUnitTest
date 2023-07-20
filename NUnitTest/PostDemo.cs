using System;
using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json;

namespace APIDemo
{
    public class PostDeviceData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DeviceData2 Data { get; set; }
    }

    public class DeviceData2
    {
        public string Color { get; set; }
        public string Capacity { get; set; }
    }

    [TestFixture]
    public class RestSharpTest
    {
        private RestClient client;
        private RestRequest request;

        [SetUp]
        public void Setup()
        {
            client = new RestClient("https://api.restful-api.dev/");

            request = new RestRequest("objects", Method.Post);
        }

        [Test]
        public void TestPostNewDevice()
        {
            // Create the new device data to be sent in the request
            PostDeviceData newDevice = new PostDeviceData
            {
                Id = "123",
                Name = "Google Pixel 6 Pro",
                Data = new DeviceData2
                {
                    Color = "Cloudy White",
                    Capacity = "128 GB"
                }
            };

            // Serialize the PostDeviceData object to JSON and add it to the request body
            string jsonBody = JsonConvert.SerializeObject(newDevice);

            // Send POST request to create the new device
            request = new RestRequest("objects", Method.Post);
            request.AddJsonBody(jsonBody);
            RestResponse response = client.Execute(request);

            // Validate the status code
            Assert.AreEqual(200, (int)response.StatusCode, $"Unexpected status code: {response.StatusCode}");

            // Deserialize the response content into PostDeviceData object
            PostDeviceData addedDevice = JsonConvert.DeserializeObject<PostDeviceData>(response.Content);

            // Validate the response body structure
            Assert.IsNotNull(addedDevice.Id, "The added device does not have an ID.");
            Assert.IsNotNull(addedDevice.Name, "The added device does not have a Name.");
            Assert.IsNotNull(addedDevice.Data, "The added device does not have Data.");

            // Validate specific property values
            Assert.AreEqual(newDevice.Name, addedDevice.Name, "Incorrect Name in the added device.");
            Assert.AreEqual(newDevice.Data.Color, addedDevice.Data.Color, "Incorrect Color in the added device.");
            Assert.AreEqual(newDevice.Data.Capacity, addedDevice.Data.Capacity, "Incorrect Capacity in the added device.");

            // Cleanup: Send DELETE request to remove the added device
            request = new RestRequest($"objects/{addedDevice.Id}", Method.Delete);
            response = client.Execute(request);

            // Validate the DELETE response status code (Assuming 204 No Content as the expected status code for successful deletion)
            Assert.AreEqual(204, (int)response.StatusCode, $"Unexpected status code for DELETE request: {response.StatusCode}");

            // Verify that the device is deleted by sending a GET request for the same ID
            request = new RestRequest($"objects/{addedDevice.Id}", Method.Get);
            response = client.Execute(request);

            // Validate the GET response status code (Assuming 404 Not Found as the expected status code for a deleted resource)
            Assert.AreEqual(404, (int)response.StatusCode, $"Unexpected status code for GET request after deletion: {response.StatusCode}");
        }
    }
}
