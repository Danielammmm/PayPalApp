using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.WebHooks;
using Newtonsoft.Json.Linq;

namespace PayPalIntegrationApp.App_code
{
    public class PaypalWebHookHandler : WebHookHandler
    {
        public override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            if (receiver.Equals("paypal"))
            {
                // Obtener los datos del Webhook de PayPal
                JObject data = context.GetDataOrDefault<JObject>();

                // Extraer el tipo de evento y el ID del recurso
                string eventType = (string)data["event_type"];
                string resourceId = (string)data["resource"]["id"];

                // Manejar diferentes eventos de PayPal
                if (eventType == "PAYMENT.CAPTURE.COMPLETED")
                {
                    // Pago exitoso, procesar lógica aquí
                    System.Diagnostics.Debug.WriteLine($"✅ Pago capturado. ID: {resourceId}");
                }
                else if (eventType == "BILLING.SUBSCRIPTION.ACTIVATED")
                {
                    // Suscripción activada
                    System.Diagnostics.Debug.WriteLine($"🔔 Suscripción activada. ID: {resourceId}");
                }
                else
                {
                    // Otro evento no manejado
                    System.Diagnostics.Debug.WriteLine($"📢 Evento recibido: {eventType} - ID: {resourceId}");
                }
            }

            return Task.FromResult(true);
        }
    }

}