using ADCore.ApiReader.Models;
using ADCore.ApiReader.Services;
using ADCore.Kafka.Attributes;
using ADCore.Kafka.Messaging.Publisher;
using ADCore.Router.CallManager.Models;
using ADCore.Router.CallManager.Util;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RequestModel = ADCore.Router.CallManager.Models.RequestModel;
using ResponseModel = ADCore.Router.CallManager.Models.ResponseModel;

namespace ADCore.Router.CallManager.Jobs
{
    
    [Worker, DisallowConcurrentExecution]
    public class CallJob : IJob
    {
        private readonly AppSettings _appSettings;
        private readonly IMessagePublisher _messagePublisher;

        public bool IsRunning { get; set; }
        public static List<RequestModel> RequestQueue { get; set; }

        public IApiClient _ApiClient { get; set; }
        

        public CallJob(IApiClient apiClient, IOptions<AppSettings> appSettingsOptions, IMessagePublisher messagePublisher)
        {
            _ApiClient = apiClient;
            _appSettings = appSettingsOptions.Value;
            _messagePublisher = messagePublisher;

        }

        public async Task Execute(IJobExecutionContext context)
        {

            if (RequestQueue != null && RequestQueue.Count > 0 && !IsRunning)
                RunCallManager();
            
        }

        private void RunCallManager()
        {

            IsRunning = true;

            while (RequestQueue.Count > 0)
            {
                var request = GetFirstRequestFromRequestQueue();
                var baseHost = request.Url.GetBaseHostNameFromLink();

                var routerConfigSupplierResponseResult = _ApiClient.GetAsync(
                  new ApiReader.Models.RequestModel
                  {
                      Url = $"{_appSettings.RouterConfigSupplierUrl}?key={baseHost}",
                  }).Result;

                var routerConfigSupplierResponse = JsonConvert.DeserializeObject<RouterConfigSupplierResponse>(routerConfigSupplierResponseResult.Data);

                DesideToCallRequest(request, routerConfigSupplierResponse);

            }
            IsRunning = false;

        }

        private void DesideToCallRequest(RequestModel request, RouterConfigSupplierResponse routerConfigSupplierResponse)
        {

            if (!routerConfigSupplierResponse.FindKey)
            {
                SendRequestRegularly(request);
                return;
            }

            if (routerConfigSupplierResponse.UseProxy ||
                routerConfigSupplierResponse.RateLimitPeriod !=
                Models.enums.RateLimitPeriod.None)
            {
                SendRequestByProxy(request, routerConfigSupplierResponse);
                return;
            }

            if (routerConfigSupplierResponse.UseDns)
            {
                SendRequestByDns(request, routerConfigSupplierResponse);
                return;
            }

            SendRequestRegularly(request);

        }

        private void SendRequestByDns(RequestModel request, RouterConfigSupplierResponse routerConfigSupplierResponse)
        {
            var baseHost = request.Url.GetBaseHostNameFromLink();

            var routerDnsResourceManagerResponseResult = _ApiClient.GetAsync(
                 new ApiReader.Models.RequestModel
                 {
                     Url = $"{_appSettings.RouterDNSResourceManagerUrl}?domain={baseHost}&&RateLimitPeriod={routerConfigSupplierResponse.RateLimitPeriod}&&RateLimitCoun={routerConfigSupplierResponse.RateLimitCount}",
                 }).Result;

            var dnsResponse = JsonConvert.DeserializeObject<RouterDNSResourceManagerResponse>(routerDnsResourceManagerResponseResult.Data);

            var response = new ResponseModel
            {
                Url = request.Url,
                Domain = request.Url.GetBaseHostNameFromLink(),
                TopicName = request.TopicName,
            };

            if (!dnsResponse.IsSuccess)
            {
                response.CallStatus = "Call Rejected. No more Dns Server resource find with this RateLimit. Overflow!!! ";
                SendToKafka(response);
                return;
            }


            var res = _ApiClient.GetAsync(
                new ApiReader.Models.RequestModel
                {
                    Url = $"{dnsResponse.CalExecuterUrl}/Execute?url={request.Url}",
                    HeaderStrings = request.HeaderStrings
                }).Result;

            response.IsSuccessCall = res.IsSuccess;

            if (res.IsSuccess)
                response.Data = res.Data;
            else
                response.CallStatus = "Call Rejected. ApiClient Can't Call Api ";


            SendToKafka(response);


        }

        private void SendToKafka(ResponseModel response)
        {
            Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>send to kafka (topic {response.TopicName})------------------");
            _messagePublisher.PublishAsync(response);
            //_messagePublisher.PublishAsync(response,response.TopicName);

        }

        private void SendRequestByProxy(RequestModel request, RouterConfigSupplierResponse routerConfigSupplierResponse)
        {

            var baseHost = request.Url.GetBaseHostNameFromLink();

            var routerProxyResourceManagerResponseResult = _ApiClient.GetAsync(
                 new ApiReader.Models.RequestModel
                 {
                     Url = $"{_appSettings.RouterProxyResourceManagerUrl}?domain={baseHost}&&RateLimitPeriod={routerConfigSupplierResponse.RateLimitPeriod}&&RateLimitCoun={routerConfigSupplierResponse.RateLimitCount}",
                 }).Result;

            var ProxyResponse = JsonConvert.DeserializeObject<RouterProxyResourceManagerResponse>(routerProxyResourceManagerResponseResult.Data);

            var response = new ResponseModel
            {
                Url = request.Url,
                Domain = request.Url.GetBaseHostNameFromLink(),
                TopicName = request.TopicName,
            };

            if (!ProxyResponse.IsSuccess)
            {
                response.CallStatus = "Call Rejected. No more Proxy resource find with this RateLimit. Overflow!!! ";
                SendToKafka(response);
                return;
            }
            ;
            var res = _ApiClient.GetAsync(
                new ApiReader.Models.RequestModel
                {
                    Url = request.Url,
                    HeaderStrings = request.HeaderStrings
                }, new Proxy()
                {
                    ProxyUrl = ProxyResponse.ProxyUrl,
                    UserName = ProxyResponse.ProxyUserName,
                    Password = ProxyResponse.ProxyPassword
                }).Result;


            response.IsSuccessCall = res.IsSuccess;

            if (res.IsSuccess)
                response.Data = res.Data;
            else
                response.CallStatus = $"Call Rejected. ApiClient Can't Call Api-{res.Exception}";


            SendToKafka(response);

        }

        private void SendRequestRegularly(RequestModel request)
        {
            var res = _ApiClient.GetAsync(
                 new ApiReader.Models.RequestModel
                 {
                     Url = request.Url,
                     HeaderStrings = request.HeaderStrings
                 }).Result;


            var response = new ResponseModel
            {
                Url = request.Url,
                Domain = request.Url.GetBaseHostNameFromLink(),
                TopicName = request.TopicName,
                IsSuccessCall = res.IsSuccess
            };

            if (res.IsSuccess)
            {
                response.Data = res.Data;
            }
            else
            {
                response.CallStatus = "Call Rejected. ApiClient Can't Call Api ";
            }

            SendToKafka(response);


        }

        private RequestModel GetFirstRequestFromRequestQueue()
        {
            var ret = RequestQueue[0];
            RequestQueue.RemoveAt(0);
            return ret;
        }
    }



}
