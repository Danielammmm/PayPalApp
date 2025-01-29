using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PayPalIntegrationApp
{
    public partial class FormPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string accessToken = Session["AccessToken"] as string;
                string planId = Request.QueryString["planId"] ?? Session["PlanID"] as string;

                if (string.IsNullOrEmpty(accessToken))
                {
                    lblMessage.Text = "Access Token no encontrado. Por favor, inicia sesión primero.";
                    btnCreatePayment.Enabled = false;
                }
                else if (string.IsNullOrEmpty(planId))
                {
                    lblMessage.Text = "Plan ID no encontrado. Por favor, crea un plan primero.";
                    btnCreatePayment.Enabled = false;
                }
                else
                {
                    Session["PlanID"] = planId; // Guardar Plan ID en la sesión
                    txtPlanId.Text = planId;   // Mostrar Plan ID en el campo
                    lblMessage.Text = "Datos cargados. Puedes proceder a generar el enlace de pago.";
                }
            }
        }


        protected async void btnCreatePayment_Click(object sender, EventArgs e)
        {
            string planId = txtPlanId.Text.Trim();
            string accessToken = Session["AccessToken"] as string;

            if (string.IsNullOrEmpty(accessToken))
            {
                lblMessage.Text = "Access Token no encontrado. Por favor, inicia sesión primero.";
                return;
            }

            try
            {
                // Crear un enlace de pago basado en el Plan ID
                string approvalUrl = await CreateSubscription(planId, accessToken);

                // Mostrar el enlace de pago
                lnkPayment.NavigateUrl = approvalUrl;
                lnkPayment.Visible = true;
                lblMessage.Text = "Haz clic en el enlace para completar el pago.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
            }
        }

        private async Task<string> CreateSubscription(string planId, string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                // Crear el cuerpo de la solicitud para la suscripción
                var subscription = new
                {
                    plan_id = planId,
                    application_context = new
                    {
                        brand_name = "Mi App de Suscripciones",
                        return_url = "https://localhost:44358/WebhookHandler.aspx",
                        cancel_url = "https://localhost:44358/FormProducts.aspx"
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(subscription), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://api-m.sandbox.paypal.com/v1/billing/subscriptions", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine("Respuesta de PayPal: " + jsonResponse);

                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    // Buscar el enlace de aprobación en los enlaces devueltos
                    foreach (var link in result.links)
                    {
                        if (link.rel == "approve")
                        {
                            return link.href; // Enlace de aprobación
                        }
                    }

                    throw new Exception("No se encontró el enlace de aprobación en la respuesta.");
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al crear la suscripción: {errorResponse}");
                }

            }
        }

    }
}
