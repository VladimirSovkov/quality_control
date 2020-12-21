using MbDotNet;
using MbDotNet.Enums;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace lab8
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var client = new MountebankClient();
            const int port = 8080;
            client.DeleteImposter(port);

            var imposter = client.CreateHttpImposter(port);

            imposter.AddStub().ReturnsJson(HttpStatusCode.OK, 91.21).OnPathAndMethodEqual($"/rate/usd", Method.Get);
            imposter.AddStub().ReturnsJson(HttpStatusCode.OK, 89.90).OnPathAndMethodEqual($"/rate/eur", Method.Get);
            imposter.AddStub().ReturnsJson(HttpStatusCode.OK, 20).OnPathAndMethodEqual($"/rate/rub", Method.Get);
            imposter.AddStub().ReturnsJson(HttpStatusCode.OK, 10.23).OnPathAndMethodEqual($"/rate/kaz", Method.Get);
            imposter.AddStub().ReturnsStatus(HttpStatusCode.BadRequest);

            client.Submit(imposter);
        }

        public async System.Threading.Tasks.Task<string> GetResponseAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(url);
            return result.Content.ReadAsStringAsync().Result;
        }

        public async System.Threading.Tasks.Task<string> GetResponseCodeAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(url);
            return result.StatusCode.ToString();
        }

        [Test]
        public async System.Threading.Tasks.Task RequestReturnsUsdRate()
        {
            Setup();
            var responseCode = await GetResponseCodeAsync("http://localhost:8080/rate/usd");
            Assert.AreEqual(HttpStatusCode.OK.ToString(), responseCode);

            string response = await GetResponseAsync("http://localhost:8080/rate/usd");
            Assert.AreEqual("91.21", response);
        }

        [Test]
        public async System.Threading.Tasks.Task RequestReturnsEurRate()
        {
            Setup();
            var responseCode = await GetResponseCodeAsync("http://localhost:8080/rate/usd");
            Assert.AreEqual(HttpStatusCode.OK.ToString(), responseCode);
            
            string response = await GetResponseAsync("http://localhost:8080/rate/eur");
            Assert.AreEqual("89.9", response);
        }

        [Test]
        public async System.Threading.Tasks.Task RequestReturnsRubRate()
        {
            Setup();
            var responseCode = await GetResponseCodeAsync("http://localhost:8080/rate/usd");
            Assert.AreEqual(HttpStatusCode.OK.ToString(), responseCode);
           
            string response = await GetResponseAsync("http://localhost:8080/rate/rub");
            Assert.AreEqual("20", response);
        }

        [Test]
        public async System.Threading.Tasks.Task RequestReturnsAnError()
        {
            Setup();
            var responseCode = await GetResponseCodeAsync("http://localhost:8080/rate/noRate");
            
            Assert.AreEqual(HttpStatusCode.BadRequest.ToString(), responseCode);
        }
    }
}