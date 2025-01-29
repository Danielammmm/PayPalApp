using System;
using System.IO;
using System.Text;
using System.Web.UI;
using PayPalIntegrationApp.Core.Services;

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
                    string json;
                    using (var reader = new StreamReader(Request.InputStream, Encoding.UTF8))
                    {
                        json = reader.ReadToEnd();
                    }

                    string accessToken = Session["AccessToken"] as string;
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        lblWebhookResult.Text = "Access Token no encontrado. Por favor, inicia sesión primero.";
                        Response.StatusCode = 400; // Bad Request
                        return;
                    }

                    var webhookService = new PayPalWebhookService();
                    bool isValidEvent = await webhookService.VerifyEvent(json, Request.Headers, accessToken);

                    if (isValidEvent)
                    {
                        string resultMessage = await webhookService.ProcessWebhookEvent(json);
                        lblWebhookResult.Text = resultMessage;
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
    }
}
