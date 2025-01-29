using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PayPalIntegrationApp.Core.Services
{
    public class PayPalWebhookService
    {
        private readonly string baseUrl = "https://api.sandbox.paypal.com";

        /// <summary>
        /// Procesa el evento recibido del Webhook de PayPal.
        /// </summary>
        /// <param name="json">Cuerpo de la solicitud en formato JSON</param>
        /// <returns>Mensaje indicando el resultado del procesamiento</returns>
        public async Task<string> ProcessWebhookEvent(string json)
        {
            try
            {
                dynamic webhookEvent = JsonConvert.DeserializeObject(json);

                switch ((string)webhookEvent.event_type)
                {
                    case "BILLING.SUBSCRIPTION.ACTIVATED":
                        return "Suscripción activada correctamente.";

                    case "BILLING.SUBSCRIPTION.CANCELLED":
                        return "Suscripción cancelada.";

                    case "PAYMENT.SALE.COMPLETED":
                        return "Pago completado correctamente.";

                    default:
                        return $"Evento no manejado: {webhookEvent.event_type}";
                }
            }
            catch (Exception ex)
            {
                return $"Error interno al procesar el Webhook: {ex.Message}";
            }
        }

        /// <summary>
        /// Verifica la autenticidad del evento recibido desde PayPal.
        /// </summary>
        /// <param name="json">Cuerpo del webhook recibido</param>
        /// <param name="headers">Encabezados de la solicitud HTTP</param>
        /// <param name="accessToken">Token de acceso para autenticación</param>
        /// <returns>True si el evento es válido, false si no lo es</returns>
        public async Task<bool> VerifyEvent(string json, System.Collections.Specialized.NameValueCollection headers, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Access Token no encontrado.");
            }

            var verifyRequest = new
            {
                transmission_id = headers["paypal-transmission-id"],
                transmission_time = headers["paypal-transmission-time"],
                cert_url = headers["paypal-cert-url"],
                auth_algo = headers["paypal-auth-algo"],
                transmission_sig = headers["paypal-transmission-sig"],
                webhook_id = "6M4565033P393972M", // Cambia esto con tu Webhook ID de PayPal
                webhook_event = JsonConvert.DeserializeObject(json)
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = new StringContent(JsonConvert.SerializeObject(verifyRequest), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{baseUrl}/v1/notifications/verify-webhook-signature", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                return result.verification_status == "SUCCESS";
            }
        }
    }
}
