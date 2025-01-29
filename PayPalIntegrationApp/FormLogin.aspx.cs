using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using PayPalIntegrationApp.Core.Services;

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

                    // Mostrar el Access Token
                    lblToken.Text = accessToken;
                    lblMessage.Text = string.Empty;

                    // Hacer visible el botón de productos con el token en el QueryString
                    lnkGoToProducts.NavigateUrl = $"FormProducts.aspx?accessToken={HttpUtility.UrlEncode(accessToken)}";
                    lnkGoToProducts.Visible = true;
                }
                else
                {
                    lblMessage.Text = "No se pudo obtener el Access Token.";
                    lnkGoToProducts.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
                lnkGoToProducts.Visible = false;
            }
        }
    }
}
