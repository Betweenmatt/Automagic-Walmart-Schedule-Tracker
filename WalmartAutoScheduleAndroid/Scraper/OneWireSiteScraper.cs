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
    class OneWireSiteScraper
    {
        private SettingsObject _settings;
        public OneWireSiteScraper(SettingsObject settings)
        {
            _settings = settings;
        }

        public SiteScraperReturnObject LoginCheck(string page = null)
        {
            System.Diagnostics.Debug.WriteLine("starting login");
            if (page == null)
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
            const string loginUrl = "https://pfedprod.wal-mart.com/idp/SSO.saml2?SAMLRequest=hZLNTsMwEIRfJfI9P04JpVZbKaQgVSpQNcCBC7KcLVhy7OB1Wnh77FSF9gInS7sz3m%2FWniJvVcfK3r3rDXz0gC76bJVGNjRmpLeaGY4SmeYtIHOC1eXdiuVJxjprnBFGkRPL3w6OCNZJo0m0XMzIKx1d0yta5NXNxXVZXl7QcZ7fjGkxKSpaXFYZiZ7BotfPiLd7E2IPS42Oa%2BdLGZ3EWRFn9DHPGc3ZaPRCooXPIDV3g%2BvduQ5ZmnZbaDxuk%2By5iltuXSJMm8qmS%2Bv6IQngOYkqoxHCxX9FEAcRE721%2Foxl2ykppCPRrbEChk3OyJYrhMC79pHlDn4q5XEDYVjfgq3B7qSAp83ql9ZoCKA%2FnIHvVZk3qdPOoNsAdoGCzKehw4at2PnRLc7M0%2FRUMz28972PtVysjSf%2FCtwt%2Fyd1qMgm3g5S5izXKH16n0cps68scOczOtsDSeeHkee%2Fav4N&RelayState=https%3A%2F%2Fone.walmart.com%2Fcontent%2Fuswire%2Fen_us.html&attempts=1&skip.simultaneous.authn.req.check=Resubmit";
            const string endUrl = "https://one.walmart.com/content/uswire/en_us.html";
            var uri = new Uri(loginUrl);

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.AllowAutoRedirect = true;

            var payload = new Payload(_settings.UserName, _settings.Password);

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
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
                WwwEncode = new Dictionary<string, string>()
                {
                    {"pf.username","mja002o.s01292@us.walmart.com" },
                    {"pf.pass","M1292A" },
                    {"domainName","US" },
                    {"BU","st" },
                    {"$store","1292" },
                    {"pf.ok","clicked" },
                    {"pf.cancel","" },
                    {"g-recaptcha-response","03AOLTBLQhsMMENA1_TEqz-HgplSYQYgI_0VLCVJ28kx8jjfMHz-Akq_Kk2pdp_0bqpytEq0TVDEYAxjs6O2wdYEAz3Ope5Ze9V0IMY25AZNDySz2MqgycBaK3_nd2nuonNXo-BvaE4SF2vS139zQ2svp_UBdIvgLUENnQytXaE4nxdWhKwT8tJgZkGKkj8i5IaJp5hqbEdyLJ_ziLRZyB606KjYwQkcfD7Ljgh7a0y9YwjVdtpbnFDdT44cFGZ-Y_OWtKGEtJqIyyuz0mCfoXbxyeNy6cFSmJUqK9J3lMDK8Ur21SxPhp8tTxxF0PpkkSGsiV-szUfTfDh13UX0_BYjMNRYyV8Uer7g" },
                    {"pf.adapterId","AdpFormUPNReCaptcha" }
                };
            }
        }
    }
}