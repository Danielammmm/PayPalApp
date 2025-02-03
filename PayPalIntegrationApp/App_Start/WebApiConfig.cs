using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Config;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Microsoft.AspNet.WebHooks.Properties;
using PayPal.Api;

namespace PayPalIntegrationApp.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Habilitar rutas de Web API
            config.MapHttpAttributeRoutes();

            // Habilitar WebHooks de PayPal
            config.InitializeCustomWebHooks(); // This line should work now
            config.InitializeReceivePaypalWebHooks();
        }
    }
}