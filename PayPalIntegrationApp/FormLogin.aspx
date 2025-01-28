<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormLogin.aspx.cs" Inherits="PayPalIntegrationApp.FormLogin" Async="true" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Obtener Access Token - PayPal</title>
    <link href="styles/site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" class="form-container">
        <h2>Integración con PayPal</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="error-message"></asp:Label>
        <div class="form-group">
            <label for="txtClientId">Client ID</label>
            <asp:TextBox ID="txtClientId" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="txtClientSecret">Client Secret</label>
            <asp:TextBox ID="txtClientSecret" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
        </div>
        <asp:Button ID="btnGetToken" runat="server" Text="Obtener Token" OnClick="btnGetToken_Click" CssClass="btn btn-primary" />
        <div class="result">
            <h3>Access Token:</h3>
            <asp:Label ID="lblToken" runat="server" CssClass="success-message"></asp:Label>
        </div>
    </form>
</body>
</html>