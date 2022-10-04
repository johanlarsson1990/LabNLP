// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EmptyBot .NET Template version v4.17.1

using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyBot
{
    public class EmptyBot : ActivityHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmptyBot> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

     
        string inputLanguage = "";
        public EmptyBot(IConfiguration configuration, ILogger<EmptyBot> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
          

            var translator = new Translator()
            {
                translatorKey = _configuration["TranslatorKey"],
                translatorEndpoint = _configuration["TranslatorEndpoint"],
                credentail = new AzureKeyCredential(_configuration["KeyCredential"]),
                analyticsEndpoint = new Uri(_configuration["AnalyticsEndpoint"])
            };
            
            var detectClient = new TextAnalyticsClient(translator.analyticsEndpoint, translator.credentail);
        await translator.LanguageDetection(detectClient, turnContext.Activity.Text, inputLanguage);
          string translatedMessage =  await translator.TranslateToEnglish(turnContext.Activity.Text, inputLanguage);
            turnContext.Activity.Text = translatedMessage;
            var httpClient = _httpClientFactory.CreateClient();
            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                EndpointKey = _configuration["QnAEndpointKey"],
                Host = _configuration["QnAEndpointHostName"]
            },
            null,
            httpClient);

            _logger.LogInformation("Calling QnA Maker");

            var options = new QnAMakerOptions { Top = 1 };
           

            // The actual call to the QnA Maker service.
            var response = await qnaMaker.GetAnswersAsync(turnContext, options);
            if (response != null && response.Length > 0)
            {
             
                await turnContext.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
     
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("No QnA Maker answers were found."), cancellationToken);
            }
        }
    }
}
