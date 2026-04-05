using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "b1294f37b00413eaecafa76f575b260b";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&appid={chave}";

            // Verifica conexão com a internet antes de fazer a requisição.
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new Exception("Sem conexão com a internet. Verifique sua conexão e tente novamente.");
            }

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp = await client.GetAsync(url);

                // Verifica o StatusCode da resposta.
                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"Cidade \"{cidade}\" não encontrada. Verifique o nome e tente novamente.");
                }

                if (resp.IsSuccessStatusCode)
                {
                    string json = await resp.Content.ReadAsStringAsync();

                    var rascunho = JObject.Parse(json);

                    DateTime time = new();
                    DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                    DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                    t = new()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        lon = (double)rascunho["coord"]["lon"],
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"],
                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        speed = (double)rascunho["wind"]["speed"],
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString(),
                        sunset = sunset.ToString()
                    }; // Fecha obj Tempo.
                } // Fecha if se o status do servidor foi de sucesso
            } //Fecha laço using

            return t;
        }
    }
}
