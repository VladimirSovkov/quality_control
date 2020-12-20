using lab9.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace lab9
{
    public static class Api
    {
        public static string url = "http://91.210.252.240:9010/";
        public static async Task<Response<List<Product>>> GetProductsAsync()
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(url + "api/products");
            var answer = result.Content.ReadAsStringAsync().Result;
            var statusCodeStr = result.StatusCode.ToString();
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(answer);
            Response<List<Product>> response = new Response<List<Product>>();
            response.Items = products;
            response.StatusCode = statusCodeStr;
            return response;
        }

        //что то возвращается
        public static async Task<Response<Answer>> DeleteProductAsync(int productId)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage result = await httpClient.GetAsync(url + "/api/deleteproduct?id=" + productId);
            Response<Answer> response = new Response<Answer>();
            response.Items = JsonConvert.DeserializeObject<Answer>(result.Content.ReadAsStringAsync().Result);
            response.StatusCode = result.StatusCode.ToString();
            return response;
        }

        //что то возвращается
        public static async Task<Response<Answer>> AddProductAsync(Product product)
        {
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            var answer = await client.PostAsync(url + "/api/addproduct", data);
            string result = answer.Content.ReadAsStringAsync().Result;
            Response<Answer> response  = new Response<Answer>();
            response.Items = JsonConvert.DeserializeObject<Answer>(result);
            response.StatusCode = answer.StatusCode.ToString();
            return response;
        }

        public static async Task<Response<Answer>> EditProduct(Product product)
        {
            using var client = new HttpClient();
            Response<Answer> response = new Response<Answer>();
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var answer = await client.PostAsync(url + "/api/editproduct", data);
            string result = answer.Content.ReadAsStringAsync().Result;
            response.Items = JsonConvert.DeserializeObject<Answer>(result);
            response.StatusCode = answer.StatusCode.ToString();
            return response;
        }

        public static Response<Product> GetProduct(int productId)
        {
            Response<Product> response = new Response<Product>();
            var products = GetProductsAsync().Result;
            response.StatusCode = products.StatusCode;
            if (response.StatusCode.ToString() == HttpStatusCode.OK.ToString())
            {
                response.Items = products.Items.Find(x => x.Id == productId);
                response.StatusCode = products.StatusCode;
            }
            return response;
        }
    }
}
