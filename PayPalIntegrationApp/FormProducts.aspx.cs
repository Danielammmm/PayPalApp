using System;
using PayPalIntegrationApp.Core.Services;

namespace PayPalIntegrationApp
{
    public partial class FormProducts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Obtener el Access Token desde la sesión o QueryString
                string accessToken = Session["AccessToken"] as string ?? Request.QueryString["accessToken"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    // Redirigir al login si no hay token
                    Response.Redirect("FormLogin.aspx");
                }
                else
                {
                    // Guardar el token en la sesión si viene desde el QueryString
                    Session["AccessToken"] = accessToken;
                    lblMessage.Text = "Access Token válido. Puedes continuar.";
                }
            }
        }

        protected async void btnCreateProductAndPlan_Click(object sender, EventArgs e)
        {
            string productName = txtProductName.Text.Trim();
            string productDescription = txtProductDescription.Text.Trim();
            string productType = ddlProductType.SelectedValue;
            string planName = txtPlanName.Text.Trim();
            string planPrice = txtPlanPrice.Text.Trim();
            string billingFrequency = ddlBillingFrequency.SelectedValue;

            try
            {
                // Obtener el Access Token desde la sesión
                string accessToken = Session["AccessToken"].ToString();

                // Crear instancia del nuevo servicio
                var payPalProductService = new PayPalProductService();

                // Llamar al servicio para crear producto y plan
                string planId = await payPalProductService.CreateProductAndPlan(
                    accessToken, productName, productDescription, productType, planName, planPrice, billingFrequency);

                // Mostrar resultados
                lblResult.Text = $"Producto y Plan creados con éxito. Plan ID: {planId}";
                lblMessage.Text = string.Empty;

                // Guardar el Plan ID en la sesión y mostrar el botón de pago
                Session["PlanID"] = planId;
                btnGoToPayment.Visible = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
                lblResult.Text = string.Empty;
            }
        }

        protected void btnGoToPayment_Click(object sender, EventArgs e)
        {
            // Redirigir al formulario de pagos con el Plan ID
            string planId = Session["PlanID"] as string;
            Response.Redirect($"FormPayment.aspx?planId={planId}");
        }
    }
}
