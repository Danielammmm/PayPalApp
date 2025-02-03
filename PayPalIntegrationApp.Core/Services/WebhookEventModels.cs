using System;
using System.Collections.Generic;

namespace PayPalIntegrationApp.Core.Services
{
    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class SellerProtection
    {
        public string status { get; set; }
        public List<string> dispute_categories { get; set; }
    }

    public class RelatedIds
    {
        public string order_id { get; set; }
    }

    public class SupplementaryData
    {
        public RelatedIds related_ids { get; set; }
    }

    public class SellerReceivableBreakdown
    {
        public Amount gross_amount { get; set; }
        public Amount paypal_fee { get; set; }
        public List<PlatformFee> platform_fees { get; set; }
        public Amount net_amount { get; set; }
    }

    public class PlatformFee
    {
        public Amount amount { get; set; }
        public Payee payee { get; set; }
    }

    public class Payee
    {
        public string merchant_id { get; set; }
    }

    public class Resource
    {
        public string disbursement_mode { get; set; }
        public Amount amount { get; set; }
        public SellerProtection seller_protection { get; set; }
        public string create_time { get; set; }
        public string custom_id { get; set; }
        public SupplementaryData supplementary_data { get; set; }
        public string update_time { get; set; }
        public bool final_capture { get; set; }
        public SellerReceivableBreakdown seller_receivable_breakdown { get; set; }
        public string invoice_id { get; set; }
        public List<Link> links { get; set; }
        public string id { get; set; }
        public string status { get; set; }
    }

    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

    public class WebhookEvent
    {
        public string id { get; set; }
        public string create_time { get; set; }
        public string resource_type { get; set; }
        public string event_type { get; set; }
        public string summary { get; set; }
        public Resource resource { get; set; }
        public List<Link> links { get; set; }
        public string event_version { get; set; }
        public string resource_version { get; set; }
    }

    public class AmountDetails
    {
        public string subtotal { get; set; }
    }

}
