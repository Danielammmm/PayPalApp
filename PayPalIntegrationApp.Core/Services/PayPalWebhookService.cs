using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace PayPalIntegrationApp.Core.Services
{
    public class PayPalWebhookService
    {
        private readonly HttpClient _client;
        private readonly string baseUrl = "https://api.sandbox.paypal.com";

        public PayPalWebhookService(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Recupera el ID del último evento webhook relacionado con pagos o suscripciones.
        /// </summary>
        public async Task<string> GetLatestWebhookId(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Access Token no encontrado.");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string webhookListUrl = $"{baseUrl}/v1/notifications/webhooks-events";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(webhookListUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject webhookData = JObject.Parse(jsonResponse);

                    JArray events = (JArray)webhookData["events"];
                    foreach (var evt in events)
                    {
                        if (evt["event_type"]?.ToString() == "BILLING.SUBSCRIPTION.ACTIVATED" || evt["event_type"]?.ToString() == "PAYMENT.SALE.COMPLETED")
                        {
                            return evt["id"]?.ToString();
                        }
                    }
                }
                else
                {
                    throw new Exception($"Error al obtener los webhooks: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"⚠ Error al recuperar el Webhook ID: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Procesa el evento recibido del Webhook de PayPal.
        /// </summary>
        public async Task<string> ProcessWebhookEvent(string json)
        {
            try
            {
                dynamic webhookEvent = JsonConvert.DeserializeObject(json);
                string eventType = webhookEvent.event_type;
                string resourceId = webhookEvent.resource.id; // Obtener ID generado

                switch (eventType)
                {
                    case "PAYMENT.CAPTURE.COMPLETED":
                        return $"💰 Pago completado correctamente. ID: {resourceId}";

                    case "PAYMENT.CAPTURE.DENIED":
                        return $"❌ Pago denegado. ID: {resourceId}";

                    case "PAYMENT.CAPTURE.PENDING":
                        return $"⏳ Pago pendiente. ID: {resourceId}";

                    default:
                        return $"ℹ Evento no manejado: {eventType} - ID: {resourceId}";
                }
            }
            catch (Exception ex)
            {
                return $"⚠ Error interno al procesar el Webhook: {ex.Message}";
            }
        }

        /// <summary>
        /// Verifica la autenticidad del evento recibido desde PayPal.
        /// </summary>
        public async Task<bool> VerifyEvent(string json, System.Collections.Specialized.NameValueCollection headers, string accessToken, string webhookId)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Access Token no encontrado.");
            }

            if (string.IsNullOrEmpty(webhookId))
            {
                throw new Exception("Webhook ID no encontrado. Asegúrate de configurarlo.");
            }

            var verifyRequest = new
            {
                transmission_id = headers["paypal-transmission-id"],
                transmission_time = headers["paypal-transmission-time"],
                cert_url = headers["paypal-cert-url"],
                auth_algo = headers["paypal-auth-algo"],
                transmission_sig = headers["paypal-transmission-sig"],
                webhook_id = webhookId,
                webhook_event = JsonConvert.DeserializeObject(json)
            };

            var content = new StringContent(JsonConvert.SerializeObject(verifyRequest), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.PostAsync($"{baseUrl}/v1/notifications/verify-webhook-signature", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(jsonResponse);

            return result.verification_status == "SUCCESS";
        }
    }
}