using NUnit.Framework;
using lab9.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace lab9
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public bool Equals(Product product1, Product product2)
        {
            return product1.Id == product2.Id
                && product1.Category_id == product2.Category_id
                && product1.Title == product2.Title
                && product1.Alias == product2.Alias
                && product1.Content == product2.Content
                && product1.Price == product2.Price
                && product1.Old_price == product2.Old_price
                && product1.Status == product2.Status
                && product1.Keywords == product2.Keywords
                && product1.Description == product2.Description
                && product1.Img == product2.Img
                && product1.Hit == product2.Hit
                && product1.Cat == product2.Cat;
        }

        public Response<Answer> AddProduct(string path)
        {
            Product product;
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                product = JsonConvert.DeserializeObject<Product>(json);
            }
            var response = Api.AddProductAsync(product);
            return response.Result;
        }

        public void DeleteProduct(int id)
        {
            var response = Api.DeleteProductAsync(id);
            Assert.AreEqual(
                    HttpStatusCode.OK.ToString(),
                    response.Result.StatusCode,
                    $"Полученный HttpStatusCode не совпадает с ожидаемым. Продукт с id = { id }. Не был удален");
            Assert.AreEqual(1, response.Result.Items.Status, "Статус ответа на изменение продукта на совпадает с ожидаемым.  Продукт с id = { id }. Не был удален");

        }

        public async Task EditProductTestAsync(Product product)
        {
            var responseEdit = await Api.EditProduct(product);
            Assert.AreEqual(
                HttpStatusCode.OK.ToString(),
                responseEdit.StatusCode,
                "Полученный HttpStatusCode при изменении продукта не совпадает с ожидаемым");
            Assert.AreEqual(1, responseEdit.Items.Status, "Статус ответа при изменении продукта на совпадает с ожидаемым");
        }

        public int AddProduct()
        {
            var response = AddProduct(@"Product.json");
            int id = response.Items.Id;

            Assert.AreEqual(HttpStatusCode.OK.ToString(), response.StatusCode, "Полученный при добавлении HttpStatusCode не совпадает с ожидаемым");
            Assert.AreEqual(1, response.Items.Status, "Статус ответа при добавлени продукта на совпадает с ожидаемым");

            return id;
        }

        [Test]
        public async Task GetProducts()
        {
            var answer = await Api.GetProductsAsync();
            List<Product> sample;
            using (StreamReader r = new StreamReader(@"AllProducts.json"))
            {
                string json = r.ReadToEnd();
                sample = JsonConvert.DeserializeObject<List<Product>>(json);
            }

            //Assert.AreEqual(sample, answer.Result.Items);
            CollectionAssert.AreEqual(sample, answer.Items, $"Список образцов не совпадает с реальным списком продуктов.\n" +
                $"Count({sample.Count} == {answer.Items.Count})");
            Assert.AreEqual(HttpStatusCode.OK.ToString(), answer.StatusCode, "Полученный HttpStatusCode при получении списка продуктов не совпадает с ожидаемым");
        }

        [Test]
        public void AddProductAsync()
        {
            Product product;
            using (StreamReader r = new StreamReader(@"Product.json"))
            {
                string json = r.ReadToEnd();
                product = JsonConvert.DeserializeObject<Product>(json);
            }
            var response = Api.AddProductAsync(product);

            Assert.AreEqual(HttpStatusCode.OK.ToString(), response.Result.StatusCode);
            var responseDelet = Api.DeleteProductAsync(response.Result.Items.Id);
            Assert.AreEqual(HttpStatusCode.OK.ToString(), responseDelet.Result.StatusCode);
        }

        [Test]
        public void AddIncorrectProduct()
        {
            Product product;
            using (StreamReader r = new StreamReader(@"Product.json"))
            {
                string json = r.ReadToEnd();
                product = JsonConvert.DeserializeObject<Product>(json);
            }
            product.Category_id = 17;
            product.Status = 2;
            product.Hit = 2;
            var response = Api.AddProductAsync(product);
            Assert.AreEqual(HttpStatusCode.OK.ToString(), response.Result.StatusCode);
            Assert.AreEqual(1, response.Result.Items.Status);

            DeleteProduct(response.Result.Items.Id);
        }

        [Test]
        public void DeleteProduct()
        {
            var id = AddProduct();
            var response = Api.DeleteProductAsync(id);
            Assert.AreEqual(HttpStatusCode.OK.ToString(), response.Result.StatusCode);
            Assert.AreEqual(1, response.Result.Items.Status);
            Assert.AreEqual(0, response.Result.Items.Id);
        }

        [Test]
        public void DeleteProductWithNonExistentId()
        {
            var response = Api.DeleteProductAsync(9999999);
            Assert.AreEqual(HttpStatusCode.OK.ToString(), response.Result.StatusCode);
            Assert.AreEqual(0, response.Result.Items.Status);
            Assert.AreEqual(0, response.Result.Items.Id);
        }

        [Test]
        public async Task ChangeProductAlias()
        {
            var id = AddProduct();
            Product product = Api.GetProduct(id).Items;
            product.Alias = "string";
            product.Id = id;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            Assert.True(Equals(product, answer.Items), "Ожидаемый продукт не совпадает с добавленным");
        }

        [Test]
        public async Task ChangeProductContent()
        {
            var id = AddProduct();
            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Content = "No content";
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            Assert.True(Equals(product, answer.Items), $"при измении Content на {product.Content} образец не совпадает с реальным продуктом.\n" +
            $" Content({product.Content} == {answer.Items.Content}) Alias({product.Alias} == {product.Alias})");
        }

        [Test]
        public async Task ChangeProductPrice()
        {
            var id = AddProduct();
            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Price = 999999;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            Assert.True(Equals(product, answer.Items), $"при измении Price на {product.Price} образец не совпадает с реальным продуктом." +
            $" Price({product.Price} == {answer.Items.Price}) Alias({product.Alias} == {product.Alias})");
        }

        [Test]
        public async Task ChangeProductOldPrice()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Old_price = 100;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            Assert.True(Equals(product, answer.Items), $"при измении Old_price на {product.Old_price} образец не совпадает с реальным продуктом." +
            $" Old_price({product.Old_price} == {answer.Items.Old_price}) Alias({product.Alias} == {product.Alias})");
        }


        [Test]
        public async Task ChangeProdutStatus()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Status = 1;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            Assert.True(Equals(product, answer.Items), $"при измении status на {product.Status} образец не совпадает с реальным продуктом." +
            $" Status({product.Status} == {answer.Items.Status}) Alias({product.Alias} == {product.Alias})");
        }

        [Test]
        public async Task ChangeProductIncorrectStatus()
        {
            // > 1
            {
                var id = AddProduct();
                Product product = Api.GetProduct(id).Items;
                product.Status = 2;
                await EditProductTestAsync(product);

                var answer = Api.GetProduct(id);
                DeleteProduct(id);

                product.Alias = "testim-" + id.ToString();
                product.Status = 1;
                Assert.True(Equals(product, answer.Items), $"при измении status на {product.Status} образец не совпадает с реальным продуктом." +
                $" Status({product.Status} == {answer.Items.Status}) Alias({product.Alias} == {product.Alias})");
            }

            // < 0
            {
                var id = AddProduct();
                Product product = Api.GetProduct(id).Items;
                product.Status = -1;
                await EditProductTestAsync(product);

                var answer = Api.GetProduct(id);
                DeleteProduct(id);

                product.Alias = "testim-" + id.ToString();
                product.Status = 1;
                Assert.True(Equals(product, answer.Items), $"при измении status на {product.Status} образец не совпадает с реальным продуктом." +
                $" Status({product.Status} == {answer.Items.Status}) Alias({product.Alias} == {product.Alias})");
            }
        }

        [Test]
        public async Task ChangeProductKeywords()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Keywords = "Key words test";
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            Assert.True(Equals(product, answer.Items), $"При изменении описания, образец не совпадает с реальным продуктом");
        }

        [Test]
        public async Task ChangeProductDescription()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Description = "описание русском";
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            Assert.True(Equals(product, answer.Items), "При изменении описания, образец не совпадает с реальным продуктом");
        }

        [Test]
        public async Task ChangeProductImage()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Img = "image123.png";
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);
            product.Alias = "testim-" + id.ToString();
            product.Img = "no_image.jpg";
            Assert.True(Equals(product, answer.Items), $"Образец не совпадает с реальным продуктом." +
                $" Img({product.Img} == {answer.Items.Img}) Alias({product.Alias} == {product.Alias})");
        }

        [Test]
        public async Task ChangeProductHit()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Hit = 0;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            Assert.True(Equals(product, answer.Items), $"Образец не совпадает с реальным продуктом." +
                $" Hit({product.Hit} == {answer.Items.Hit}) Alias({product.Alias} == {product.Alias})");
        }


        [Test]
        public async Task ChangeProductCategoryId()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Category_id = 5;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            product.Cat = "Механические";
            Assert.True(Equals(product, answer.Items), $"Образец не совпадает с реальным продуктом." +
                $" Cat({product.Cat} == {answer.Items.Cat}) Alias({product.Alias} == {product.Alias})");
        }

        [Test]
        public async Task ChangeProductIncorrectCategoryId()
        {
            var id = AddProduct();
            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Category_id = 17;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            Assert.AreEqual(null, answer.Items, $"Реальный продукт не равен null");
        }

        [Test]
        public async Task ChangeProductCat()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Id = id;
            product.Cat = "Механические";
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            DeleteProduct(id);

            product.Alias = "testim-" + id.ToString();
            product.Cat = "Men";
            Assert.True(Equals(product, answer.Items),
                $"Образец не совпадает с реальным продуктом." +
                $" Cat({product.Cat} == {answer.Items.Cat}) Alias({product.Alias} == {product.Alias})");
        }

        [Test]
        public async Task ChangeProductTitle()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Title = "string";
            product.Id = id;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            Assert.AreEqual(HttpStatusCode.OK.ToString(), answer.StatusCode);

            DeleteProduct(id);

            product.Alias = "string";
            Assert.True(Equals(product, answer.Items), $"Alias не совпадает с образцом. {product.Alias} == {answer.Items.Alias}");
        }

        [Test]
        public async Task ChangeToEmptyProductTitle()
        {
            var id = AddProduct();

            Product product = Api.GetProduct(id).Items;
            product.Title = "";
            product.Id = id;
            await EditProductTestAsync(product);

            var answer = Api.GetProduct(id);
            Assert.AreEqual(HttpStatusCode.OK.ToString(), answer.StatusCode);

            DeleteProduct(id);

            product.Alias = "";
            Assert.True(Equals(product, answer.Items), $"Alias не совпадает с образцом. {product.Alias} == {answer.Items.Alias}");
        }
    }
}