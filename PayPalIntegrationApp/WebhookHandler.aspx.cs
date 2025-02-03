using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using PayPalIntegrationApp.Core.Services;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace PayPalIntegrationApp
{
    public partial class WebhookHandler : Page
    {
        private readonly PayPalWebhookService _webhookService;
        private string connectionString = "Data Source=DESKTOP-LIO9C0K\\SQLEXPRESS;Initial Catalog=PayPalWebhooksDB;Integrated Security=True;\r\n";

        public WebhookHandler()
        {
            _webhookService = new PayPalWebhookService(new HttpClient());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.HttpMethod == "POST")
            {
                HandleWebhookAsync().GetAwaiter().GetResult();
                return;
            }
        }

        private async Task HandleWebhookAsync()
        {
            try
            {
                string jsonPayload;
                using (StreamReader reader = new StreamReader(Request.InputStream, Encoding.UTF8))
                {
                    jsonPayload = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                if (string.IsNullOrEmpty(jsonPayload))
                {
                    Response.StatusCode = 400;
                    Response.Write("No se recibió un cuerpo en la solicitud.");
                    Response.Flush();
                    return;
                }

                string accessToken = Session["AccessToken"] as string;
                string webhookId = Session["WebhookID"] as string;

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(webhookId))
                {
                    Response.StatusCode = 401;
                    Response.Write("Access Token o Webhook ID no disponible.");
                    Response.Flush();
                    return;
                }

                bool isValid = await _webhookService.VerifyEvent(jsonPayload, Request.Headers, accessToken, webhookId).ConfigureAwait(false);

                if (!isValid)
                {
                    Response.StatusCode = 403;
                    Response.Write("La firma del webhook no es válida.");
                    Response.Flush();
                    return;
                }

                Response.StatusCode = 200;
                Response.Write("Webhook recibido.");
                Response.Flush();

                _ = ProcessWebhookAsync(jsonPayload);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                Response.Write($"Error interno: {ex.Message}");
                Response.Flush();
            }
        }

        private async Task ProcessWebhookAsync(string jsonPayload)
        {
            try
            {
                WebhookEvent webhookEvent = JsonConvert.DeserializeObject<WebhookEvent>(jsonPayload);
                string eventType = webhookEvent.event_type;
                string resourceId = webhookEvent.resource.id;
                string status = webhookEvent.resource.status;
                string webhookId = webhookEvent.id;

                await SaveWebhookToDatabase(webhookId, eventType, resourceId, status, jsonPayload);
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Error JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar el Webhook: {ex.Message}");
            }
        }

        private async Task SaveWebhookToDatabase(string webhookId, string eventType, string resourceId, string status, string jsonPayload)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string query = "INSERT INTO WebhookLogs (WebhookId, EventType, ResourceId, Status, Payload, ReceivedAt) VALUES (@WebhookId, @EventType, @ResourceId, @Status, @Payload, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@WebhookId", webhookId);
                    cmd.Parameters.AddWithValue("@EventType", eventType);
                    cmd.Parameters.AddWithValue("@ResourceId", resourceId);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Payload", jsonPayload);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
