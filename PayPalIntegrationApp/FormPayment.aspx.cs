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
using PayPalIntegrationApp.Core.Services;

namespace PayPalIntegrationApp
{
    public partial class FormPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Validar que el Webhook ID esté almacenado
                if (Session["WebhookID"] == null)
                {
                    Response.Redirect("FormWebhookID.aspx");
                    return;
                }

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
                    Session["PlanID"] = planId;
                    txtPlanId.Text = planId;
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
                var payPalSubscriptionService = new PayPalSubscriptionService();
                string approvalUrl = await payPalSubscriptionService.CreateSubscription(planId, accessToken);

                lnkPayment.NavigateUrl = approvalUrl;
                lnkPayment.Visible = true;
                lblMessage.Text = "Haz clic en el enlace para completar el pago.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
            }
        }
    }
}
