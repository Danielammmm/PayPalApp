<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormWebhookID.aspx.cs" Inherits="PayPalIntegrationApp.FormWebhookID" Async="true"%>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Ingresar Webhook ID</title>
    <link href="styles/site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" class="form-container">
        <h2>Configurar Webhook ID</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="error-message"></asp:Label>

        <div class="form-group">
            <label for="txtWebhookId">Webhook ID</label>
            <asp:TextBox ID="txtWebhookId" runat="server" CssClass="form-control" Placeholder="Ingresa tu Webhook ID de PayPal"></asp:TextBox>
        </div>

        <asp:Button ID="btnSaveWebhookId" runat="server" Text="Guardar" OnClick="btnSaveWebhookId_Click" CssClass="btn btn-primary" />
    </form>
</body>
</html>