using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PayPalIntegrationApp.Services; // Ensure the namespace and class exist


namespace PayPalIntegrationApp
{
    public partial class FormLogin : System.Web.UI.Page
    {
        protected async void btnGetToken_Click(object sender, EventArgs e)
        {
            string clientId = txtClientId.Text.Trim();
            string clientSecret = txtClientSecret.Text.Trim();

            try
            {
                var payPalService = new PayPalService();
                string accessToken = await payPalService.GetAccessToken(clientId, clientSecret);

                if (!string.IsNullOrEmpty(accessToken))
                {
                    // Guardar el Access Token en la sesión
                    Session["AccessToken"] = accessToken;

                    // Depuración: Verificar que se guarda en la sesión
                    System.Diagnostics.Debug.WriteLine("Access Token guardado en la sesión: " + Session["AccessToken"]);

                    // Redirigir al formulario de productos
                    Response.Redirect("FormProducts.aspx");
                }
                else
                {
                    lblMessage.Text = "No se pudo obtener el Access Token.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
            }
        }

    }
}
