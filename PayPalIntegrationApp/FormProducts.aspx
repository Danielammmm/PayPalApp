<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormProducts.aspx.cs" Inherits="PayPalIntegrationApp.FormProducts" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Crear Producto y Suscripción</title>
    <link href="styles/site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" class="form-container">
        <h2>Crear Producto y Suscripción</h2>
        <asp:Label ID="lblMessage" runat="server" CssClass="error-message"></asp:Label>
        
        <!-- Producto -->
        <div class="form-group">
            <label for="txtProductName">Nombre del Producto</label>
            <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="txtProductDescription">Descripción</label>
            <asp:TextBox ID="txtProductDescription" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="ddlProductType">Tipo de Producto</label>
            <asp:DropDownList ID="ddlProductType" runat="server" CssClass="form-control">
                <asp:ListItem Value="SERVICE" Text="Servicio"></asp:ListItem>
                <asp:ListItem Value="PHYSICAL" Text="Físico"></asp:ListItem>
                <asp:ListItem Value="DIGITAL" Text="Digital"></asp:ListItem>
            </asp:DropDownList>
        </div>

        <!-- Suscripción -->
        <h3>Suscripción</h3>
        <div class="form-group">
            <label for="txtPlanName">Nombre del Plan</label>
            <asp:TextBox ID="txtPlanName" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="txtPlanPrice">Precio</label>
            <asp:TextBox ID="txtPlanPrice" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="ddlBillingFrequency">Frecuencia de Facturación</label>
            <asp:DropDownList ID="ddlBillingFrequency" runat="server" CssClass="form-control">
                <asp:ListItem Value="MONTH" Text="Mensual"></asp:ListItem>
                <asp:ListItem Value="YEAR" Text="Anual"></asp:ListItem>
            </asp:DropDownList>
        </div>

        <!-- Botón para crear -->
        <asp:Button ID="btnCreateProductAndPlan" runat="server" Text="Crear Producto y Plan" OnClick="btnCreateProductAndPlan_Click" CssClass="btn btn-primary" />
        
        <div class="result">
            <h3>Resultado:</h3>
            <asp:Label ID="lblResult" runat="server" CssClass="success-message"></asp:Label>
        </div>
    </form>
</body>
</html>
