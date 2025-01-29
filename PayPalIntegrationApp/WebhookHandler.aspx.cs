using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PayPalIntegrationApp
{
    public partial class WebhookHandler : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                try
                {
                    // Leer el cuerpo del Webhook
                    string json;
                    using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
                    {
                        json = reader.ReadToEnd();
                    }

                    // Verificar la firma del evento recibido
                    var isValidEvent = await VerifyEvent(json, Request.Headers);

                    if (isValidEvent)
                    {
                        dynamic webhookEvent = JsonConvert.DeserializeObject(json);

                        // Manejar diferentes tipos de eventos
                        switch ((string)webhookEvent.event_type)
                        {
                            case "BILLING.SUBSCRIPTION.ACTIVATED":
                                // Lógica para activar suscripciones
                                lblWebhookResult.Text = "Suscripción activada correctamente.";
                                break;

                            case "BILLING.SUBSCRIPTION.CANCELLED":
                                // Lógica para cancelar suscripciones
                                lblWebhookResult.Text = "Suscripción cancelada.";
                                break;

                            case "PAYMENT.SALE.COMPLETED":
                                // Lógica para pagos completados
                                lblWebhookResult.Text = "Pago completado correctamente.";
                                break;

                            default:
                                lblWebhookResult.Text = $"Evento no manejado: {webhookEvent.event_type}";
                                break;
                        }

                        Response.StatusCode = 200; // OK
                    }
                    else
                    {
                        lblWebhookResult.Text = "Evento no válido. Firma no verificada.";
                        Response.StatusCode = 400; // Bad Request
                    }
                }
                catch (Exception ex)
                {
                    lblWebhookResult.Text = $"Error interno: {ex.Message}";
                    Response.StatusCode = 500; // Internal Server Error
                }
                finally
                {
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            else
            {
                lblWebhookResult.Text = "⚠ Solo se aceptan solicitudes POST.";
                Response.StatusCode = 405; // Method Not Allowed
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private async Task<bool> VerifyEvent(string json, System.Collections.Specialized.NameValueCollection headers)
        {
            string accessToken = Session["AccessToken"] as string;

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Access Token no encontrado.");
            }

            // Crear la solicitud para verificar la firma
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

                var response = await client.PostAsync("https://api.sandbox.paypal.com/v1/notifications/verify-webhook-signature", content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                return result.verification_status == "SUCCESS";
            }
        }
    }
}
