<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="PayPalIntegrationApp.Dashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Dashboard</title>
    <link href="Styles/Site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="dashboard-container">
            <h1>Bienvenido al Dashboard</h1>
            <p>Tu integración con PayPal está lista para usar.</p>
            <asp:Label ID="lblToken" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
