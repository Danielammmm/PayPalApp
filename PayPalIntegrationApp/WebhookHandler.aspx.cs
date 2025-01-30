using System;
using System.IO;
using System.Text;
using System.Web.UI;
using PayPalIntegrationApp.Core.Services;
using System.Threading.Tasks;
using System.Net.Http;

namespace PayPalIntegrationApp
{
    public partial class WebhookHandler : System.Web.UI.Page
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
                lblWebhookStatus.Text = "Verificando el estado del pago...";
            }
        }
        protected void btnProceed_Click(object sender, EventArgs e)
        {
            string resourceId = Session["WebhookResourceID"] as string;

            if (!string.IsNullOrEmpty(resourceId))
            {
                // Redirigir a la página de pagos con el Resource ID
                Response.Redirect($"FormPayment.aspx?resourceId={resourceId}");
            }
            else
            {
                lblWebhookResult.Text = "⚠ No se encontró el ID del recurso. Por favor, verifica el webhook.";
            }
        }

        protected async void btnRefresh_Click(object sender, EventArgs e)
        {
            string webhookId = Session["WebhookID"] as string;
            if (string.IsNullOrEmpty(webhookId))
            {
                lblWebhookStatus.Text = "⚠ No se encontró el Webhook ID.";
                return;
            }

            string accessToken = Session["AccessToken"] as string;
            if (string.IsNullOrEmpty(accessToken))
            {
                lblWebhookStatus.Text = "⚠ Access Token no encontrado. Inicia sesión.";
                return;
            }

            string webhookUrl = $"https://api.sandbox.paypal.com/v1/notifications/webhooks-events/{webhookId}";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _httpClient.GetAsync(webhookUrl);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (jsonResponse.Contains("\"status\": \"PENDING\""))
                {
                    lblWebhookStatus.Text = "⚠ Pago aún en proceso... inténtalo de nuevo.";
                }
                else
                {
                    lblWebhookStatus.Text = "✅ Pago confirmado.";
                    btnRefresh.Visible = false;
                }
            }
            else
            {
                lblWebhookStatus.Text = "⚠ Error al consultar el estado del pago.";
            }
        }
    }
}
