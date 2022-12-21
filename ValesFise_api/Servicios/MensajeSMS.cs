using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static ValesFise_api.Modelos.Lyric;

namespace ValesFise_api.Servicios
{
    public class MensajeSMS
    {
        public MensajeSMS(){}

        /// <summary>
        /// Envia un mensaje de texto por Lyric
        /// </summary>
        /// <param name="telefono"></param>
        /// <param name="mensaje"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public  static async Task<string> Enviar(string telefono, string mensaje, IConfiguration config)
        {
            var WebUser = config.GetSection("SMS:WebUser").Value;
            var WebPass = config.GetSection("SMS:WebPass").Value;
            var ApiUser = config.GetSection("SMS:ApiUser").Value;
            var ApiPass = config.GetSection("SMS:ApiPass").Value;
            var Host = config.GetSection("SMS:Host").Value;

            string Url = "http://" + WebUser + ":" + WebPass + "@" + Host + "/cgi-bin/exec?cmd=api_get_version";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("C# App");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var headerVal = Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUser + ":" + WebPass));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", headerVal);

            HttpResponseMessage response = client.GetAsync(Url).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var version = JsonConvert.DeserializeObject<VersionSMS>(responseBody);

            Url = "http://" + WebUser + ":" + WebPass + "@" + Host + "/cgi-bin/exec?cmd=api_queue_sms&username=";
            Url += ApiUser + "&password=" + ApiPass + "&content=" + mensaje.PadRight(160).Substring(0, 160).Trim();
            Url += "&destination=" + telefono + "&api_version=" + version.api_version;

            response = client.GetAsync(Url).Result;
            response.EnsureSuccessStatusCode();
            responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

    }
}
