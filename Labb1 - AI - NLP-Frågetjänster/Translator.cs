using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmptyBot
{
    public class Translator
{
        //Translator keys
        public string translatorKey;
        public string translatorEndpoint;
        public  readonly string location = "northeurope";

        //Analytics keys
        public AzureKeyCredential credentail;
        public Uri analyticsEndpoint;


     
      public async Task<string> LanguageDetection(TextAnalyticsClient client, string inputString, string inputLanguage)
        {
            DetectedLanguage detectedLanguage =  client.DetectLanguage(inputString);
             inputLanguage =  detectedLanguage.Iso6391Name;
              return  inputLanguage;
        }
       public async Task <string>TranslateToEnglish(string textToTranslate, string inputLanguage)
        {
            string routeToBot = $"/translate?api-version=3.0&from={inputLanguage}&to=en";
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request translator.

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(translatorEndpoint + routeToBot);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", translatorKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                // Send the request and get response translator.

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.

                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
                return result;
            }
        }
        public async Task<string> TranslateToInput(string textToTranslate, string inputLanguage)
        {
            string routeToBot = $"/translate?api-version=3.0&from=en&to={inputLanguage}";
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request translator.

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(translatorEndpoint + routeToBot);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", translatorKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", location);

                // Send the request and get response translator.

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.

                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
                return result;
            }
        }
    }
}
