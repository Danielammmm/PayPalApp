using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json.Linq; // Necesario para manejar JSON

namespace PayPalIntegrationApp
{
    public partial class WebhookHandler : Page
    {
        private readonly HttpClient _httpClient;

        public WebhookHandler()
        {
            _httpClient = new HttpClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblWebhookStatus.Text = "Esperando confirmación de pago...";
            }
        }

        protected async void btnRefresh_Click(object sender, EventArgs e)
        {
            string webhookEventId = Session["WebhookEventID"] as string;

            if (string.IsNullOrEmpty(webhookEventId))
            {
                lblWebhookStatus.Text = "⚠ No se encontró el Webhook Event ID. Intentando recuperar...";
                webhookEventId = await GetLatestWebhookId();

                if (string.IsNullOrEmpty(webhookEventId))
                {
                    lblWebhookStatus.Text = "⚠ No se pudo recuperar un Webhook Event ID.";
                    return;
                }
            }

            string accessToken = Session["AccessToken"] as string;
            if (string.IsNullOrEmpty(accessToken))
            {
                lblWebhookStatus.Text = "⚠ Access Token no encontrado. Inicia sesión.";
                return;
            }

            string webhookUrl = $"https://api.sandbox.paypal.com/v1/notifications/webhooks-events/{webhookEventId}";
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(webhookUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject webhookData = JObject.Parse(jsonResponse);

                    // Obtener el nombre del evento
                    string eventType = webhookData["event_type"]?.ToString();
                    string eventStatus = webhookData["resource"]?["status"]?.ToString();
                    Console.WriteLine($"Evento: {eventType} - Estado: {eventStatus}");

                    if (eventType == "BILLING.SUBSCRIPTION.ACTIVATED" || eventType == "PAYMENT.SALE.COMPLETED")
                    {
                        if (eventStatus == "SUCCESS" || eventStatus == "ACTIVE")
                        {
                            lblWebhookStatus.Text = "✅ Pago realizado con éxito. Puedes consultar tu PayPal.";
                        }
                        else if (eventStatus == "APPROVAL_PENDING")
                        {
                            lblWebhookStatus.Text = "⚠ La suscripción aún está pendiente de aprobación.";
                        }
                        else
                        {
                            lblWebhookStatus.Text = $"⚠ Estado desconocido: {eventStatus}";
                        }
                    }
                    else
                    {
                        lblWebhookStatus.Text = "⚠ No se encontró un evento de pago completado o suscripción activada.";
                    }
                }
                else
                {
                    lblWebhookStatus.Text = "⚠ Error al consultar el estado del pago.";
                }
            }
            catch (Exception ex)
            {
                lblWebhookStatus.Text = $"⚠ Error: {ex.Message}";
            }
        }

        private async Task<string> GetLatestWebhookId()
        {
            string accessToken = Session["AccessToken"] as string;
            if (string.IsNullOrEmpty(accessToken)) return null;

            string webhookListUrl = "https://api.sandbox.paypal.com/v1/notifications/webhooks-events";
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(webhookListUrl);
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
            }
            catch (Exception ex)
            {
                lblWebhookStatus.Text = $"⚠ Error al recuperar el Webhook ID: {ex.Message}";
            }
            return null;
        }
    }
}