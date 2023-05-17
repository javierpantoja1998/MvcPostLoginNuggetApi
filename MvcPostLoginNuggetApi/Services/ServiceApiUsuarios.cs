using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuggetPostLoginJPL.Models;
using System.Net.Http.Headers;
using System.Text;

namespace MvcPostLoginNuggetApi.Services
{
    public class ServiceApiUsuarios
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;

        public ServiceApiUsuarios(IConfiguration configuration)
        {
            this.UrlApi =
                configuration.GetValue<string>("ApiUrls:ApiPostLogin");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");
        }

        //METODOS QUE SE COMUNICAN CON LA API
        public async Task<string> GetTokenAsync
            (string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    Email = email,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();

                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
