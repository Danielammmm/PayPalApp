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
        private readonly PayPalWebhookService _webhookService;

        public WebhookHandler()
        {
            _webhookService = new PayPalWebhookService(new HttpClient());
        }
        protected void btnProceed_Click(object sender, EventArgs e)
        {
            // Lógica para proceder al pago
            string resourceId = Session["WebhookResourceID"] as string;

            if (!string.IsNullOrEmpty(resourceId))
            {
                // Redirigir al formulario de pagos u otra acción
                Response.Redirect($"FormPayment.aspx?resourceId={resourceId}");
            }
            else
            {
                lblWebhookResult.Text = "⚠ No se encontró el ID del recurso. Por favor, verifica el webhook.";
            }
        }


        protected async void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                try
                {
                    string json;
                    using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
                    {
                        json = reader.ReadToEnd();
                    }

                    if (string.IsNullOrEmpty(json))
                    {
                        lblWebhookResult.Text = "⚠ No se recibió ningún evento válido.";
                        Response.StatusCode = 400;
                        return;
                    }

                    string accessToken = Session["AccessToken"] as string;
                    string webhookId = Session["WebhookID"] as string;

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        lblWebhookResult.Text = "⚠ Access Token no encontrado. Inicia sesión primero.";
                        Response.StatusCode = 401;
                        return;
                    }

                    if (string.IsNullOrEmpty(webhookId))
                    {
                        lblWebhookResult.Text = "⚠ Webhook ID no encontrado. Configúralo primero.";
                        Response.Redirect("FormWebhookID.aspx");
                        return;
                    }

                    bool isValidEvent = await _webhookService.VerifyEvent(json, Request.Headers, accessToken, webhookId);

                    if (isValidEvent)
                    {
                        string resultMessage = await _webhookService.ProcessWebhookEvent(json);
                        lblWebhookResult.Text = resultMessage;
                        btnProceed.Visible = true;
                        Response.StatusCode = 200;
                    }
                    else
                    {
                        lblWebhookResult.Text = "⚠ Evento no válido. Firma no verificada.";
                        Response.StatusCode = 400;
                    }
                }
                catch (Exception ex)
                {
                    lblWebhookResult.Text = $"⚠ Error interno: {ex.Message}";
                    Response.StatusCode = 500;
                }
                finally
                {
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            else
            {
                lblWebhookResult.Text = "⚠ Solo se aceptan solicitudes POST.";
                Response.StatusCode = 405;
                Context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
