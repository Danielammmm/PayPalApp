<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebhookHandler.aspx.cs" Inherits="PayPalIntegrationApp.WebhookHandler" Async="true" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Webhook Handler</title>
    <link href="styles/site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" class="form-container">
        <h2>Eventos Recibidos del Webhook</h2>
        
        <asp:Label ID="lblWebhookResult" runat="server" CssClass="result-message"></asp:Label>

        <h3>Eventos Recibidos:</h3>
        <asp:BulletedList ID="lstWebhookEvents" runat="server" CssClass="event-list"></asp:BulletedList>

        <asp:Button ID="btnProceed" runat="server" Text="Proceder al Pago" CssClass="btn btn-primary" OnClick="btnProceed_Click" Visible="false" />
    </form>
</body>
</html>
