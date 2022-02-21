using System;
using System.Net.Http;
using System.Text;
using DataAccess.DaRunner;
using DTO.Constants;
using DTO.Messages;
using FluentScheduler;

namespace BusinessLogic.Execution.Jobs
{
    public class PendingBalance : IJob
    {
        private readonly DaRunner _daRunner;
        private readonly HttpClient _httpClient;
        
        public PendingBalance(DaRunner daRunner, HttpClient httpClient)
        {
            _daRunner = daRunner;
            _httpClient = httpClient;
        }

        public void Execute()
        {
            var correlationId = Guid.NewGuid();
            MessageDto underProcess = null;

            do
            {
                try
                {
                    _daRunner.Run(da =>
                    {
                        var pending = da.MessageRepository().GetPending();
                        foreach (var message in pending)
                        {
                            underProcess = message;
                            HttpResponseMessage result;
                            
                            switch (underProcess.Type)
                            {
                                case MessageTypes.CreateBalance:
                                    result = BalanceCreate(underProcess, correlationId);
                                    break;
                                case MessageTypes.UpdateBalance:
                                    result = BalanceUpdate(underProcess, correlationId);
                                    break;
                                default:
                                    da.MessageRepository().UpdateStatus(underProcess.Id, correlationId, Status.Unknown);
                                    continue;
                            }

                            da.MessageRepository().UpdateStatus(underProcess.Id, correlationId, result.IsSuccessStatusCode ? 
                                    Status.Completed : 
                                    Status.Failed);
                        }
                    });
                }
                catch (Exception e)
                {
                    if (underProcess != null)
                    {
                        _daRunner.Run(da => da.MessageRepository().UpdateStatus(underProcess.Id, correlationId, Status.Failed));
                    }
                }
            } while (underProcess != null);
        }

        private HttpResponseMessage BalanceCreate(MessageDto message, Guid correlationId)
        {
            var url = $"balance/new/{message.Id}";
            return SendRequest(message.Content, url, correlationId);
        }

        private HttpResponseMessage BalanceUpdate(MessageDto message, Guid correlationId)
        {
            var url = $"balance/update/{message.Id}";
            return SendRequest(message.Content, url, correlationId);
        }

        private HttpResponseMessage SendRequest(string content, string url, Guid correlationId)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(url),
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("CorrelationId", correlationId.ToString());
            
            return _httpClient.Send(request);
        }
    }
}