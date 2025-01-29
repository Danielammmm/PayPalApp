using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PayPalIntegrationApp.Core.Services
{
    public class PayPalService
    {
        private readonly string baseUrl = "https://api-m.sandbox.paypal.com";

        public async Task<string> CreateProduct(string accessToken, string name, string description, string type)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var product = new
                {
                    name = name,
                    description = description,
                    type = type
                };

                var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{baseUrl}/v1/catalogs/products", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(responseData);
                    return result.id;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al crear el producto: {error}");
                }
            }
        }

        public async Task<string> CreatePlan(string accessToken, string productId, string name, string price, string frequency)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var plan = new
                {
                    product_id = productId,
                    name = name,
                    billing_cycles = new[]
                    {
                new
                {
                    frequency = new { interval_unit = frequency, interval_count = 1 },
                    tenure_type = "REGULAR",
                    sequence = 1,
                    total_cycles = 0,
                    pricing_scheme = new { fixed_price = new { value = price, currency_code = "USD" } }
                }
            },
                    payment_preferences = new
                    {
                        auto_bill_outstanding = true,
                        setup_fee = new { value = "0", currency_code = "USD" },
                        setup_fee_failure_action = "CONTINUE",
                        payment_failure_threshold = 3
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(plan), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{baseUrl}/v1/billing/plans", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(responseData);
                    return result.id;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al crear el plan: {error}");
                }
            }
        }


        public async Task<string> GetAccessToken(string clientId, string clientSecret)
        {
            using (var client = new HttpClient())
            {
                string endpoint = "/v1/oauth2/token";

                var encodedAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", encodedAuth);

                var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                try
                {
                    var response = await client.PostAsync($"{baseUrl}{endpoint}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

                        // Depuración: Verificar el token obtenido
                        System.Diagnostics.Debug.WriteLine("Access Token obtenido: " + tokenData["access_token"]);

                        return tokenData["access_token"];
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Error en la respuesta: {response.StatusCode} - {errorResponse}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error en la solicitud al obtener el token: {ex.Message}");
                }
            }
        }
    }
}
