using System;
using System.Web.Services.Description;
using PayPalIntegrationApp.Services;


namespace PayPalIntegrationApp
{
    public partial class FormProducts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string accessToken = Session["AccessToken"] as string;

                // Depuración: Verificar si el token está disponible
                System.Diagnostics.Debug.WriteLine("Access Token leído desde la sesión: " + accessToken);

                if (string.IsNullOrEmpty(accessToken))
                {
                    lblMessage.Text = "Access Token no encontrado. Por favor, inicia sesión primero.";
                    btnCreateProductAndPlan.Enabled = false;
                }
                else
                {
                    lblMessage.Text = string.Empty;
                    btnCreateProductAndPlan.Enabled = true;
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

                // Crear instancia del servicio
                var payPalService = new PayPalService();

                // Crear producto
                string productId = await payPalService.CreateProduct(accessToken, productName, productDescription, productType);

                // Crear suscripción
                string planId = await payPalService.CreatePlan(accessToken, productId, planName, planPrice, billingFrequency);

                lblResult.Text = $"Producto y Plan creados con éxito: Producto ID - {productId}, Plan ID - {planId}";
                lblMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
                lblResult.Text = string.Empty;
            }
        }
    }
}
