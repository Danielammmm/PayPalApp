using System;
using System.Threading.Tasks;
using PayPalIntegrationApp.Core.Services;

namespace PayPalIntegrationApp.Core.Services
{
    public class PayPalProductService
    {
        private readonly PayPalService _payPalService;

        public PayPalProductService()
        {
            _payPalService = new PayPalService();
        }

        /// <summary>
        /// Crea un producto y un plan en PayPal.
        /// </summary>
        /// <param name="accessToken">Token de acceso de PayPal</param>
        /// <param name="productName">Nombre del producto</param>
        /// <param name="productDescription">Descripción del producto</param>
        /// <param name="productType">Tipo de producto</param>
        /// <param name="planName">Nombre del plan</param>
        /// <param name="planPrice">Precio del plan</param>
        /// <param name="billingFrequency">Frecuencia de facturación</param>
        /// <returns>Retorna el Plan ID creado</returns>
        public async Task<string> CreateProductAndPlan(
            string accessToken,
            string productName,
            string productDescription,
            string productType,
            string planName,
            string planPrice,
            string billingFrequency)
        {
            try
            {
                // Crear producto
                string productId = await _payPalService.CreateProduct(accessToken, productName, productDescription, productType);

                // Crear plan
                string planId = await _payPalService.CreatePlan(accessToken, productId, planName, planPrice, billingFrequency);

                return planId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el producto y plan: {ex.Message}");
            }
        }
    }
}
