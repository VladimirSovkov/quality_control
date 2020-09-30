using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace CheckBrokenLinks
{
    public class LinkSearch
    {
        private readonly Uri _mainLink;
        List<string> _visitedLinks = new List<string>();
        List<LinkResponseCode> _validLink = new List<LinkResponseCode>();
        List<LinkResponseCode> _invalidLink = new List<LinkResponseCode>();
        DateTime _checkTime;

        public LinkSearch(string mainLink)
        {
            _mainLink = new Uri(mainLink);
        }

        public struct LinkResponseCode
        {
            public string Url { get; set; }
            public HttpStatusCode Code { get; set; }
        }
        struct WebSiteResponse
        {
            public HttpStatusCode Code { get; set; }
            public string Html { get; set; }
        }

        private WebSiteResponse LoadPage(string url)
        {
            WebSiteResponse webSiteResponse = new WebSiteResponse();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var streamResponse = response.GetResponseStream();
                    StreamReader stream = new StreamReader(streamResponse);
                    string result = stream.ReadToEnd();
                    webSiteResponse.Html = result;
                    webSiteResponse.Code = response.StatusCode;
                }
                else 
                {
                    webSiteResponse.Code = response.StatusCode;
                }
            }
            catch (WebException exception)
            {
                webSiteResponse.Code = ((HttpWebResponse)exception.Response).StatusCode;
            }
            catch (Exception)
            {
                throw new Exception("Unknown errore!");
            }
            return webSiteResponse;
        }

        private List<string> GetAllLinksFromPage(string html)
        {
            List<string> linkList = new List<string>();
            HtmlDocument htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(html);
            var collection = htmlSnippet.DocumentNode.SelectNodes("//a[@href]");
            foreach (HtmlNode link in collection)
            {
                //если нет href то он просто выдаст пустую строку 
                string attribute = link.GetAttributeValue("href", "");
                string url = "";
                if (Uri.IsWellFormedUriString(attribute, UriKind.Absolute))
                {
                    url = new Uri(attribute, UriKind.Absolute).AbsoluteUri;
                }
                else if (Uri.IsWellFormedUriString(attribute, UriKind.Relative))
                {
                    Uri relative = new Uri(attribute, UriKind.Relative);
                    url = new Uri(_mainLink, relative).AbsoluteUri;
                }

                if (!linkList.Contains(url) && (url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("ftp://")))
                {
                    linkList.Add(url);
                }

            }
            return linkList;
        }

        private void FindAndSortLinks(string url)
        {
            var response = LoadPage(url);
            _visitedLinks.Add(url);
            if (response.Code == HttpStatusCode.OK)
            {
                _validLink.Add(new LinkResponseCode { Code = response.Code, Url = url });
                List<string> linkList = new List<string>();
                if (IsCorrectDomain(url))
                {
                    linkList = GetAllLinksFromPage(response.Html);
                }
                foreach (var item in linkList)
                {
                    if (!_visitedLinks.Contains(item))
                    {
                        FindAndSortLinks(item);
                    }
                }
            }
            else
            {
                _invalidLink.Add(new LinkResponseCode { Code = response.Code, Url = url });
            }
        }

        private bool IsCorrectDomain(string item)
        {
            try
            {
                Uri url = new Uri(item);
                return url.Host == _mainLink.Host;
            }
            catch (Exception)
            {
                throw new Exception("Invalid url!");
            }
        }

        public void OutputDataToFile(string pathToFileValidData, string pathToFileInvalidData)
        {
            try
            {
                StreamWriter fileValidData = new StreamWriter("../../../" + pathToFileValidData, false);
                foreach (var item in _validLink)
                {
                    fileValidData.WriteLine(item.Url + " " + (int)item.Code);
                }
                fileValidData.WriteLine($"Kоличество ссылок: { _validLink.Count}");
                fileValidData.WriteLine($"Дата проверки: { _checkTime}");
                fileValidData.Close();


                StreamWriter fileInvalidData = new StreamWriter("../../../" + pathToFileInvalidData, false);
                foreach (var item in _invalidLink)
                {
                    fileInvalidData.WriteLine(item.Url + " " + (int)item.Code);
                }
                fileInvalidData.WriteLine($"Kоличество ссылок: { _invalidLink.Count}");
                fileInvalidData.WriteLine($"Дата проверки: { _checkTime}");
                fileInvalidData.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void FindLinks()
        {
            _invalidLink.Clear();
            _validLink.Clear();
            FindAndSortLinks(_mainLink.AbsoluteUri);
            _checkTime = DateTime.Now;
            _visitedLinks.Clear();
        }

        public List<LinkResponseCode> GetValidLink()
        {
            return _validLink;
        }

        public List<LinkResponseCode> GetInvalidLink()
        {
            return _invalidLink;
        }

    }

    class Program
    {
        static void Main()
        {
            LinkSearch linkSearch = new LinkSearch("http://91.210.252.240/broken-links/");
            linkSearch.FindLinks();
            linkSearch.OutputDataToFile("valid.txt", "invalid.txt");
            return;
        }
    }
}
