<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebhookHandler.aspx.cs" Inherits="PayPalIntegrationApp.WebhookHandler" Async="true" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Confirmación de Pago</title>
    <link href="styles/site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" class="form-container">
        <h2>Estado del Pago</h2>

        <!-- Mensaje de estado del pago -->
        <asp:Label ID="lblWebhookStatus" runat="server" CssClass="result-message"></asp:Label>

        <!-- Botón para actualizar el estado del pago -->
        <asp:Button ID="btnRefresh" runat="server" Text="Actualizar Estado" CssClass="btn" OnClick="btnRefresh_Click" />

        <!-- Mensaje de carga -->
        <asp:Label ID="lblLoading" runat="server" CssClass="loading-message" Visible="false"></asp:Label>
    </form>
</body>
</html>