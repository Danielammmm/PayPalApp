using System;

namespace PayPalIntegrationApp
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["AccessToken"] != null)
                {
                    lblToken.Text = $"Access Token: {Session["AccessToken"]}";
                }
                else
                {
                    lblToken.Text = "No se pudo obtener el Access Token.";
                }
            }
        }

    }
}
