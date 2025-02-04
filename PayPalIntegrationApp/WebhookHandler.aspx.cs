using System;
using System.IO;
using System.Text;
using System.Web.UI;
using PayPalIntegrationApp.Core.Services;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace PayPalIntegrationApp
{
    public partial class WebhookHandler : System.Web.UI.Page
    {
        private readonly PayPalWebhookService _webhookService;

        public WebhookHandler()
        {
            _webhookService = new PayPalWebhookService(new HttpClient());
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                await ProcesarWebhook();
            }
            else if (!IsPostBack)
            {
                lblWebhookStatus.Text = "Esperando confirmación de pago...";
            }
        }

        private async Task ProcesarWebhook()
        {
            try
            {
                string json;
                using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
                {
                    json = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrEmpty(json))
                {
                    Response.StatusCode = 400;
                    Response.Write("⚠ No se recibió ningún evento válido.");
                    return;
                }

                string accessToken = Session["AccessToken"] as string;
                string webhookId = Session["WebhookID"] as string;

                if (string.IsNullOrEmpty(accessToken))
                {
                    Response.StatusCode = 401;
                    Response.Write("⚠ Access Token no encontrado. Inicia sesión primero.");
                    return;
                }

                if (string.IsNullOrEmpty(webhookId))
                {
                    Response.Redirect("FormWebhookID.aspx");
                    return;
                }

                bool isValidEvent = await _webhookService.VerifyEvent(json, Request.Headers, accessToken, webhookId);

                if (isValidEvent)
                {
                    string resultMessage = await _webhookService.ProcessWebhookEvent(json);
                    Session["WebhookResourceID"] = JObject.Parse(json)["resource"]?["id"]?.ToString();
                    Response.StatusCode = 200;
                    Response.Write(resultMessage);
                }
                else
                {
                    Response.StatusCode = 400;
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;            }
            finally
            {
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected async void btnRefresh_Click(object sender, EventArgs e)
        {
            string webhookEventId = Session["WebhookResourceID"] as string;

            if (string.IsNullOrEmpty(webhookEventId))
            {
                lblWebhookStatus.Text = "⚠ No se encontró el Webhook Event ID. Intentando recuperar...";

                try
                {
                    webhookEventId = await _webhookService.GetLatestWebhookId(Session["AccessToken"] as string);
                }
                catch (Exception ex)
                {
                    lblWebhookStatus.Text = $"⚠ Error al recuperar el Webhook ID: {ex.Message}";
                    return;
                }

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
            try
            {
                var result = await GetWebhookDetails(webhookUrl, accessToken);
                lblWebhookStatus.Text = result;
            }
            catch (Exception ex)
            {
                lblWebhookStatus.Text = $"⚠ Error: {ex.Message}";
            }
        }

        private async Task<string> GetWebhookDetails(string webhookUrl, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await client.GetAsync(webhookUrl);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error al consultar el Webhook: {response.ReasonPhrase}");
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject webhookData = JObject.Parse(jsonResponse);

                string eventType = webhookData["event_type"]?.ToString();
                string eventStatus = webhookData["resource"]?["status"]?.ToString();

                switch (eventType)
                {
                    case "BILLING.SUBSCRIPTION.ACTIVATED":
                    case "PAYMENT.SALE.COMPLETED":
                        if (eventStatus == "SUCCESS" || eventStatus == "ACTIVE")
                        {
                            return "✅ Pago realizado con éxito. Puedes consultar tu PayPal.";
                        }
                        else if (eventStatus == "APPROVAL_PENDING")
                        {
                            return "⚠ La suscripción aún está pendiente de aprobación.";
                        }
                        else
                        {
                            return $"⚠ Estado desconocido: {eventStatus}";
                        }
                    default:
                        return "⚠ No se encontró un evento de pago completado o suscripción activada.";
                }
            }
        }
    }
}