using System;
using System.Web.UI;

namespace PayPalIntegrationApp
{
    public partial class FormWebhookID : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Si el usuario ya ingresó un Webhook ID, mostrarlo
                txtWebhookId.Text = Session["WebhookID"] as string ?? "";
            }
        }

        protected void btnSaveWebhookId_Click(object sender, EventArgs e)
        {
            string webhookId = txtWebhookId.Text.Trim();

            if (string.IsNullOrEmpty(webhookId))
            {
                lblMessage.Text = "⚠ Debes ingresar un Webhook ID válido.";
                return;
            }

            // Guardar en la sesión para futuras solicitudes
            Session["WebhookID"] = webhookId;

            // Redirigir a la página de pagos
            Response.Redirect("FormPayment.aspx");
        }
    }
}
