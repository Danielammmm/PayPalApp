using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PayPalIntegrationApp.Core.Services
{
    public class PayPalSubscriptionService
    {
        private readonly string baseUrl = "https://api-m.sandbox.paypal.com";

        public async Task<string> CreateSubscription(string planId, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                // Crear el cuerpo de la solicitud para la suscripción
                var subscription = new
                {
                    plan_id = planId,
                    application_context = new
                    {
                        brand_name = "Mi App de Suscripciones",
                        return_url = "https://paypalintegrationapp20250206104741-a6d5axhde5cqbxbu.eastus-01.azurewebsites.net/WebhookHandler",
                        cancel_url = "https://paypalintegrationapp20250206104741-a6d5axhde5cqbxbu.eastus-01.azurewebsites.net/FormProducts"
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(subscription), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{baseUrl}/v1/billing/subscriptions", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("Respuesta de PayPal: " + jsonResponse);

                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    // Buscar el enlace de aprobación en los enlaces devueltos
                    foreach (var link in result.links)
                    {
                        if (link.rel == "approve")
                        {
                            return link.href; // Enlace de aprobación
                        }
                    }

                    throw new Exception("No se encontró el enlace de aprobación en la respuesta.");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al crear la suscripción: {errorResponse}");
                }
            }
        }
    }
}