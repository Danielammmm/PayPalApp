<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormPayment.aspx.cs" Inherits="PayPalIntegrationApp.FormPayment"Async="true" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Realizar Pago - PayPal</title>
    <link href="styles/site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" class="form-container">
        <h2>Realizar Pago con PayPal</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="error-message"></asp:Label>

        <!-- Campo para ingresar el Plan ID -->
        <div class="form-group">
            <label for="txtPlanId">Plan ID</label>
            <asp:TextBox ID="txtPlanId" runat="server" CssClass="form-control" Placeholder="Ingresa el Plan ID"></asp:TextBox>
        </div>

        <!-- Botón para generar el enlace de pago -->
        <asp:Button ID="btnCreatePayment" runat="server" Text="Generar Enlace de Pago" OnClick="btnCreatePayment_Click" CssClass="btn btn-primary" />

        <!-- Mostrar enlace de pago generado -->
        <div class="result">
            <h3>Enlace de Pago:</h3>
            <asp:HyperLink ID="lnkPayment" runat="server" CssClass="btn btn-link" Visible="false">Ir a PayPal</asp:HyperLink>
        </div>
    </form>
</body>
</html>
