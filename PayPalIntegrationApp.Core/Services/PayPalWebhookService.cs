using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;

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
        /// Procesa el evento recibido del Webhook de PayPal.
        /// </summary>
        public async Task<string> ProcessWebhookEvent(string json)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(json))
                    {
                        throw new ArgumentNullException(nameof(json), "El cuerpo del JSON está vacío.");
                    }

                    WebhookEvent webhookEvent = JsonConvert.DeserializeObject<WebhookEvent>(json);
                    string eventType = webhookEvent.event_type;
                    string resourceId = webhookEvent.resource.id; // Obtener ID generado

                    switch (eventType)
                    {
                        case "BILLING.SUBSCRIPTION.ACTIVATED":
                            return $"✅ Suscripción activada correctamente. ID: {resourceId}";

                        case "BILLING.SUBSCRIPTION.CANCELLED":
                            return $"❌ Suscripción cancelada. ID: {resourceId}";

                        case "PAYMENT.SALE.COMPLETED":
                            return $"💰 Pago completado correctamente. ID: {resourceId}";

                        default:
                            return $"ℹ Evento no manejado: {eventType} - ID: {resourceId}";
                    }
                }
                catch (JsonReaderException ex)
                {
                    return $"⚠ Error al analizar el JSON: {ex.Message}";
                }
                catch (Exception ex)
                {
                    return $"⚠ Error interno al procesar el Webhook: {ex.Message}";
                }
            });
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

            try
            {
                // Validar los encabezados
                if (headers == null || headers.Count == 0)
                {
                    throw new Exception("Encabezados HTTP no encontrados o están vacíos.");
                }

                var verifyRequest = new
                {
                    transmission_id = headers["paypal-transmission-id"],
                    transmission_time = headers["paypal-transmission-time"],
                    cert_url = headers["paypal-cert-url"],
                    auth_algo = headers["paypal-auth-algo"],
                    transmission_sig = headers["paypal-transmission-sig"],
                    webhook_id = webhookId, // ✅ Webhook ID obtenido dinámicamente
                    webhook_event = JsonConvert.DeserializeObject<WebhookEvent>(json)
                };

                var content = new StringContent(JsonConvert.SerializeObject(verifyRequest), Encoding.UTF8, "application/json");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _client.PostAsync($"{baseUrl}/v1/notifications/verify-webhook-signature", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Respuesta de verificación: " + jsonResponse);

                dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                return result.verification_status == "SUCCESS";
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine("Error al analizar el JSON en VerifyEvent: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error interno en VerifyEvent: " + ex.Message);
                return false;
            }
        }
    }
}
