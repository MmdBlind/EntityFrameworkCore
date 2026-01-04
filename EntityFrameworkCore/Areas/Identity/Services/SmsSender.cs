using Kavenegar;
using Kavenegar.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using System.Text;

namespace EntityFrameworkCore.Areas.Identity.Services
{
    public class SmsSender: ISmsSender
    {
        public async Task<string> SendAuthSmsAsync(string code,string phoneNumber)
        {
            var ApiKey = "6D692B4E4A6F5459325845347A4D4F4E515865705A446B464C54586E6455575978394B743972754A4D63673D";
            HttpClient httpClient=new HttpClient();
            var httpResponse = await httpClient.GetAsync($"https://api.kavenegar.com/v1/{ApiKey}/verify/lookup.json?receptor={phoneNumber}&token={code}&template=AuthVerify\r\n");
            if(httpResponse.StatusCode==HttpStatusCode.OK)
            {
                return "Success";
            }
            else
            {
                return "Failed";
            }
        }

        public async Task<string> SendAuthSmsPackageAsync(List<string> phoneNumber,string message)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                //var receptors = new List<string> { phoneNumber };

                var api = new KavenegarApi("6D692B4E4A6F5459325845347A4D4F4E515865705A446B464C54586E6455575978394B743972754A4D63673D");

                var result =  api.Send("2000660110", phoneNumber, message);

                foreach (var r in result)
                {
                    Console.Write($"{r.Messageid.ToString()}");
                    
                }
                return "Success";
            }
            catch (ApiException ex)
            {
                // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                Console.Write("Message : " + ex.Message);
                return "Failed";
            }
            catch (HttpException ex)
            {
                // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                Console.Write("Message : " + ex.Message);
                return "FailedToConnect";
            }
        }
    }
}
