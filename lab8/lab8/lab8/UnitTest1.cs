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

            var imposter = client.CreateHttpImposter(8080);

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
            Assert.Pass(HttpStatusCode.OK.ToString(), responseCode);

            string response = await GetResponseAsync("http://localhost:8080/rate/usd");
            Assert.Pass("91.21", response);
        }

        [Test]
        public async System.Threading.Tasks.Task RequestReturnsEurRate()
        {
            Setup();
            var responseCode = await GetResponseCodeAsync("http://localhost:8080/rate/usd");
            Assert.Pass(HttpStatusCode.OK.ToString(), responseCode);
            
            string response = await GetResponseAsync("http://localhost:8080/rate/eur");
            Assert.Pass("89.90", response);
        }

        [Test]
        public async System.Threading.Tasks.Task RequestReturnsRubRate()
        {
            Setup();
            var responseCode = await GetResponseCodeAsync("http://localhost:8080/rate/usd");
            Assert.Pass(HttpStatusCode.OK.ToString(), responseCode);
           
            string response = await GetResponseAsync("http://localhost:8080/rate/rub");
            Assert.Pass("20", response);
        }

        [Test]
        public async System.Threading.Tasks.Task RequestReturnsAnError()
        {
            Setup();
            var responseCode = await GetResponseCodeAsync("http://localhost:8080/rate/noRate");
            
            Assert.Pass(HttpStatusCode.BadRequest.ToString(), responseCode);
        }
    }
}