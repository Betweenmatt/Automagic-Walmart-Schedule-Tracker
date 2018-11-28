using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WalmartAutoScheduleAndroid.Scraper
{
    class SiteScraper
    {
        private SettingsObject _settings;
        public SiteScraper(SettingsObject settings)
        {
            _settings = settings;
        }
        /// <summary>
        /// Returns only the login status. Use Execute for returning data.
        /// </summary>
        /// <returns></returns>
        public SiteScraperReturnObject LoginCheck(string page = null)
        {
            System.Diagnostics.Debug.WriteLine("starting login");
            if(page == null)
                page = GetSiteData();
            //Console.WriteLine(page);
            System.Diagnostics.Debug.WriteLine("login complete");

            //scrape checks to ensure the correct page was loaded
            if (page.Contains("WalmartOne - Associate Login"))
            {
                return new SiteScraperReturnObject(SiteScraperReturnStatus.WrongLogin, null);
            }
            else if (page.Contains("OnlineSchedule Error Page"))
            {
                return new SiteScraperReturnObject(SiteScraperReturnStatus.Error, null);
            }
            return new SiteScraperReturnObject(SiteScraperReturnStatus.Success, null);
        }

        public SiteScraperReturnObject Execute()
        {
            string page = GetSiteData();
            var login = LoginCheck(page);
            if (login.Status == SiteScraperReturnStatus.Error ||
                login.Status == SiteScraperReturnStatus.WrongLogin)
                return login;
            //HAP voodoo
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);
            var collection = doc.DocumentNode.SelectNodes("//table[@class='weekTable']//tr");
            List<Day> dayList = new List<Day>();
            foreach (var x in collection)
            {
                //Console.WriteLine(x.InnerHtml);
                string times = x.SelectNodes(".//span[@class='schedTime']")?[0]?.SelectSingleNode(".//b").InnerText;
                string desc = x.SelectNodes(".//span[@class='schedTime']")?[0]?.InnerHtml;
                if (times != null && times != "" && times != " ")
                {
                    string[] descsplit = desc.Replace(times, "")
                            .Replace("<b>", "")
                            .Replace("</b>", "")
                            .Replace("&nbsp;", "")
                            .Replace("<br>", "\n")
                            .Replace("</span>", "").Split(new string[] { "<span>" }, StringSplitOptions.None);
                    var day = new Day(x.GetAttributeValue("dataDate", ""), times, descsplit[0].TrimStart(' '), descsplit[1]);
                    dayList.Add(day);
                }
                else
                {
                    var errcheck = x.SelectNodes(".//span[@class='todaySched']")?[0]?.SelectSingleNode(".//span").InnerHtml;
                    if (errcheck != null && errcheck != "" && errcheck != " ")
                    {
                        dayList.Add(new Day(x.GetAttributeValue("dataDate", "")));
                    }
                }
            }
            return new SiteScraperReturnObject(SiteScraperReturnStatus.Success, dayList);
        }

        private string GetSiteData()
        {
            const string loginUrl = "https://authn.walmartone.com/login.aspx";
            const string endUrl = "https://login.walmartone.com/onlineschedule/schedule/FullSchedule.aspx#";
            var uri = new Uri(loginUrl);

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.AllowAutoRedirect = true;

            var payload = new Payload(_settings.UserName, _settings.Password);
            
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("Location", "https://login.walmartone.com:443/onlineschedule/schedule/FullSchedule.aspx");
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.BaseAddress = uri;

                var req = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new FormUrlEncodedContent(payload.WwwEncode) };

                var resLogin = client.SendAsync(req).Result;
                if (resLogin.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine("Could not login -> StatusCode = " + resLogin.StatusCode);
                else
                    Console.WriteLine("Login Successfull.");


                var resData = client.GetAsync(endUrl).Result;
                if (resData.StatusCode != HttpStatusCode.OK)
                    Console.WriteLine("Could not get data html -> StatusCode = " + resData.StatusCode);
                else
                    Console.WriteLine("Html grab successfull.");

                var html = resData.Content.ReadAsStringAsync().Result;

                return html;
            }
        }

        /// <summary>
        /// Originally this was made to be a json object, but wm1 didnt want to accept json
        /// so i lazymode changed to form url encoded
        /// </summary>
        private class Payload
        {
            public string ViewState { get; set; }
            public string ViewStateGenerator { get; set; }
            public string EventValidation { get; set; }
            public string UxAuthMode { get; set; }
            public string UxOrigUrl { get; set; }
            public bool UxOverrideUri { get; set; }
            public string Auth_Mode { get; set; }
            public string UxUserName { get; set; }
            public string UxPassword { get; set; }
            public string SubmitCreds { get; set; }

            public Dictionary<string, string> WwwEncode { get; }

            public Payload(string un, string pw)
            {
                //everything besides the un/pw is static from the login page. Not sure why wm even included it lol
                //it looks like they were contemplating some sort of verification to prevent this type of activity,
                //but gave up and hoped no one would notice.
                ViewState = "/wEPDwULLTE0Nzk0MzA2OTJkZNkubljnaWdLhqt5LEbAyeznQvbh";
                ViewStateGenerator = "C2EE9ABB";
                EventValidation = "/wEdAAd5aKSokxuDiA+kBoNTrYp3rPjPsZEmBA3lX/H8NzbUjNQdyisHXQnOlgTBHd9s2IRPMSqB5MU0KLJ9HlkE5WOPGsmeGjk+hGqgXfcVwVSOnsCe+TIxyc238WqcckSintNvfha9fiJK+VUv37ujy4mQcrIktyb9eu1FaRzdcFSlJq5Xte0=";
                UxAuthMode = "BASIC";
                UxOrigUrl = "";
                UxOverrideUri = false;
                Auth_Mode = "basic";
                SubmitCreds = "Login";
                UxUserName = un;
                UxPassword = pw;

                WwwEncode = new Dictionary<string, string>
                {
                    { "__VIEWSTATE", ViewState },
                    { "__VIEWSTATEGENERATOR", ViewStateGenerator },
                    { "__EVENTVALIDATION", EventValidation },
                    { "uxAuthMode", UxAuthMode },
                    { "uxOrigUrl", UxOrigUrl },
                    { "uxOverrideUri", UxOverrideUri.ToString().ToLower() },
                    { "auth_mode", Auth_Mode },
                    { "uxUserName", UxUserName },
                    { "uxPassword", UxPassword },
                    { "SubmitCreds", SubmitCreds }
                };

            }
        }
    }

    class SiteScraperReturnObject
    {
        public SiteScraperReturnStatus Status { get; }
        public List<Day> Data { get; }
        public SiteScraperReturnObject(SiteScraperReturnStatus status, List<Day> data)
        {
            Status = status;
            Data = data;
        }
    }
}