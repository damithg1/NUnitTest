using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json;

namespace APIDemo
{
    public class GetDemo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    [TestFixture]
    public class GetDemo2
    {
        private RestClient client;
        private RestRequest request;
        private List<GetDemo> devices;
        RestResponse response = null;

        [SetUp]
        public void Setup()
        {
            client = new RestClient("https://api.restful-api.dev");
            request = new RestRequest("objects", Method.Get);

            // Fetch and deserialize the devices data from the API
            response = client.Execute(request);
            devices = JsonConvert.DeserializeObject<List<GetDemo>>(response.Content);

        }

        [Test]
        public void TestGetObjectsEndpoint()
        {
            try
            {
                response = client.Execute(request);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception occurred during request execution: " + ex.Message);
            }

            // Check if the response is successful
            Assert.IsTrue(response.IsSuccessful, "Request was not successful.");

            // Verify status code
            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Expected status code 200, but received " + (int)response.StatusCode);

            // Verify content type
            Assert.IsTrue(response.ContentType.StartsWith("application/json"), "Unexpected content type.");

            // Verify response body is not empty
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.Content), "Response body is empty.");

            // Verify response headers (optional)
            Assert.IsTrue(response.Headers.Count > 0, "Response headers are missing.");

            // Verify the total records count
            int expectedRecordCount = 13;
            Assert.AreEqual(expectedRecordCount, devices.Count);
        }

        [Test]
        public void TestSearchDeviceById()
        {
            string deviceIdToSearch = "12";

            // Find the device with the given ID
            GetDemo foundDevice = devices.FirstOrDefault(d => d.Id == deviceIdToSearch);

            // Verify status code
            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Expected status code 200, but received " + (int)response.StatusCode);

            // Verify that the device with the given ID is found
            Assert.IsNotNull(foundDevice, $"No device found with ID '{deviceIdToSearch}'.");

            // Perform additional assertions or actions with the found device if needed
            // For example, you can assert specific details for the device
            Assert.IsTrue(foundDevice.Data.ContainsKey("Generation"), $"Generation not found for device {foundDevice.Id}.");
            Assert.IsTrue(foundDevice.Data.ContainsKey("Price"), $"Price not found for device {foundDevice.Id}.");
            Assert.IsTrue(foundDevice.Data.ContainsKey("Capacity"), $"Capacity not found for device {foundDevice.Id}.");

            Assert.IsTrue(foundDevice.Name.Contains("Apple iPad Air"), $"Device with ID '{foundDevice.Id}' does not have 'Apple' in the name.");
            //Assert.IsTrue(foundDevice.Data.Contains("4th"), $"Invalid screen size for device {foundDevice.Id}.");

            // Perform validations on the device properties
            Assert.That(foundDevice.Name, Is.EqualTo("Apple iPad Air"), $"Incorrect Name for device with ID {foundDevice.Id}.");
            Assert.IsTrue(foundDevice.Data.ContainsKey("Generation"), $"Generation not found for device {foundDevice.Id}.");
            Assert.That(foundDevice.Data["Generation"], Is.EqualTo("4th"), $"Incorrect Generation for device with ID {foundDevice.Id}.");
            Assert.IsTrue(foundDevice.Data.ContainsKey("Price"), $"Price not found for device {foundDevice.Id}.");
            Assert.That(foundDevice.Data["Price"], Is.EqualTo("419.99"), $"Incorrect Price for device with ID {foundDevice.Id}.");
            Assert.IsTrue(foundDevice.Data.ContainsKey("Capacity"), $"Capacity not found for device {foundDevice.Id}.");
            Assert.That(foundDevice.Data["Capacity"], Is.EqualTo("64 GB"), $"Incorrect Capacity for device with ID {foundDevice.Id}.");
        }


        [Test]
        public void TestSearchDeviceByName()
        {
            response = client.Execute(request);
          
            //Assert.Fail("Exception occurred during request execution: " + ex.Message);
          
            string deviceNameToSearch = "Apple iPad Mini 5th Gen";

            // Find the devices with the given name
            List<GetDemo> foundDevice = devices.Where(d => d.Name == deviceNameToSearch).ToList();

            // Verify status code
            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Expected status code 200, but received " + (int)response.StatusCode);

            // Verify that the devices with the given name are found
            Assert.IsTrue(foundDevice.Count > 0, $"No devices found with the name '{deviceNameToSearch}'.");

            // Perform additional assertions or actions with the found devices
            foreach (GetDemo device in foundDevice)
            {
                // Verify Capacity, and Screen size fields
                Assert.IsTrue(device.Data.ContainsKey("Capacity"), $"Capacity not found for device {device.Id}.");
                Assert.IsTrue(device.Data.ContainsKey("Screen size"), $"Screen size not found for device {device.Id}.");
            }
        }

       
        [Test]
        public void TestFilterRecordsByIdsUsingQueryParameter()
        {
            // Add query parameter to filter by IDs (3, 4, and 10)
            request.AddQueryParameter("id", "3,4,10");

            // Fetch and deserialize the devices data from the API
            RestResponse response = client.Execute(request);

            devices = JsonConvert.DeserializeObject<List<GetDemo>>(response.Content);
            // IDs to filter
            List<string> idsToFilter = new List<string> { "3", "4", "10" };

            // Verify status code
            Assert.That((int)response.StatusCode, Is.EqualTo(200), "Expected status code 200, but received " + (int)response.StatusCode);

            // Assert that the expected number of records are present
            Assert.AreEqual(idsToFilter.Count, devices.Count, $"Expected {idsToFilter.Count} records with IDs {string.Join(", ", idsToFilter)}, but found {devices.Count}.");

            // Assert each filtered device with the specified IDs is present
            foreach (string id in idsToFilter)
            {
                Assert.IsTrue(devices.Any(d => d.Id == id), $"Device with ID '{id}' not found.");
            }

            // Additional assertions on the filtered devices, if needed
            foreach (GetDemo filteredDevice in devices)
            {
                Assert.IsTrue(filteredDevice.Name.Contains("Apple"), $"Device with ID '{filteredDevice.Id}' does not have 'Apple' in the name.");
                //Assert.IsTrue(filteredDevice.Data.ContainsKey("Capacity"), $"Capacity not found for device {filteredDevice.Id}.");
                //Assert.IsTrue(Convert.ToDouble(filteredDevice.Data["Screen size"]) > 0, $"Invalid screen size for device {filteredDevice.Id}.");
            }
        }

    }
}

