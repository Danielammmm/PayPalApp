# PayPalApp - Integración de Pagos y Suscripciones con PayPal <img src="https://www.paypalobjects.com/webstatic/icon/pp258.png" width="80" />

## 🚀 Descripción del Proyecto

**PayPalApp** es una aplicación **ASP.NET Web Forms** con **.NET Framework 4.7.2** que integra **pagos y suscripciones automáticas** con **PayPal REST API**.  
Permite a los usuarios:

✅ **Autenticarse** con Client ID y Secret de PayPal.  
✅ **Crear productos y planes de suscripción** dinámicamente.  
✅ **Procesar pagos y suscripciones** con enlaces generados por PayPal.  
✅ **Manejar Webhooks** para capturar eventos como activaciones y cancelaciones.  

---

## 📌 **Tecnologías Utilizadas**

- **ASP.NET Web Forms** (Front-end)  
- **.NET Framework 4.7.2**  
- **C#** para la lógica del backend  
- **HttpClient** para las solicitudes a la API de PayPal  
- **Newtonsoft.Json** para serialización/deserialización de datos JSON  
- **ngrok** para exponer localmente el Webhook  

---

## 🛠 **Instalación y Configuración**

### 1️⃣ **Clonar el Repositorio**
```sh
git clone https://github.com/Danielammmm/PayPalApp.git
cd PayPalApp
```
### 2️⃣ **Configurar las Credenciales de PayPal**
1. Crear una cuenta en PayPal Developer.
2. Obtener Client ID y Secret en el Dashboard de PayPal.
3. Configurar las credenciales en el sistema al momento del login.

### 3️⃣ **Ejecutar la Aplicación**
1. Abrir el proyecto en Visual Studio.
2. Seleccionar PayPalIntegrationApp como proyecto de inicio.
3. Presionar F5 o ejecutar el servidor IIS Express.

---
### 🔄 **Flujo del Sistema**
  1️⃣ Login en PayPal → El usuario ingresa su Client ID y Secret para obtener un AccessToken.
  
  2️⃣ Crear Producto y Plan → Se registran productos en la API de PayPal y se generan planes de suscripción.
  
  3️⃣ Generar Enlace de Pago → Se genera un link de pago para la suscripción del usuario.
  
  4️⃣ Redirección a PayPal → El usuario es llevado a PayPal para autorizar el pago.
  
  5️⃣ Confirmación con Webhook → PayPal envía notificaciones cuando un pago se procesa correctamente.
  
---
### 📡 **Configuración del Webhook (ngrok y Azure)**  

Para recibir eventos de PayPal en desarrollo y producción, puedes usar **ngrok** para pruebas locales o configurar el Webhook en **Azure**.

#### 🛠 **Opción 1: Webhook con ngrok (baja latencia en desarrollo)**  
Si deseas probar Webhooks en **localhost**, puedes utilizar **ngrok** para exponer tu servidor local a Internet:

1. **Descargar e instalar ngrok** desde [ngrok.com](https://ngrok.com/download).  
2. **Ejecutar el siguiente comando** (ajusta el puerto según IIS Express):  

   ```sh
   ngrok http 44300
3. **Copia la URL pública generada** (https://xxxxx.ngrok.io) y configúrala en tu cuenta de PayPal como la URL del Webhook.

#### ☁ Opción 2: Webhook en Azure (para producción)
Si deseas recibir Webhooks en un entorno en la nube, puedes configurar Azure App Service:

1. Publicar tu aplicación en Azure siguiendo los pasos de despliegue.

2. Obtener la URL de tu aplicación desde el portal de Azure (por ejemplo, https://tuapp.azurewebsites.net).

3. Configurar el Webhook en PayPal con la URL de producción:
   ```
   https://tuapp.azurewebsites.net/WebhookHandler.aspx

---

## 📂 **Estructura del Proyecto**
```
📦 PayPalApp
 ┣ 📂 PayPalIntegrationApp (Proyecto principal con vistas y backend)
 ┃ ┣ 📂 styles (Contiene Site.css para los estilos)
 ┃ ┣ FormLogin.aspx
 ┃ ┃ FormPayment.aspx
 ┃ ┃ FormProducts.aspx
 ┃ ┃ FormWebhookID.aspx
 ┃ ┃ WebhookHandler.aspx (Manejo de Webhooks)
 ┃ ┗ Web.config (Configuraciones de la aplicación)
 ┣ 📂 PayPalIntegrationApp.Core (Librería de clases)
 ┃ ┣ 📂 Services (Servicios para la integración con PayPal)
 ┃ ┃ ┣ PayPalProductService.cs
 ┃ ┃ ┣ PayPalService.cs
 ┃ ┃ ┗ PayPalWebhookService.cs
 ┃ ┗ PayPalSubscriptionService.cs
```
### 🌐 Páginas Web Forms
- **FormLogin.aspx.cs**

  Permite al usuario autenticarse con su Client ID y Secret, obteniendo un Access Token de PayPal.

- **FormProducts.aspx.cs**

  Permite crear productos y planes de suscripción en PayPal usando el Access Token.

- **FormPayment.aspx.cs**

  Genera enlaces de pago para que los usuarios puedan suscribirse a un plan.

- **FormWebhookID.aspx.cs**

  Permite ingresar y almacenar un Webhook ID de PayPal en sesión.

- **WebhookHandler.aspx.cs**
  Escucha y procesa eventos enviados por los Webhooks de PayPal, mostrando el estado de pagos y suscripciones.

### 🛠 Servicios Core (Lógica de Negocio)
- **PayPalService.cs**

  Servicio base para la autenticación y creación de productos y planes en PayPal.

- **PayPalWebhookService.cs**
  
  Maneja la recepción y verificación de eventos de Webhook desde PayPal.

- **PayPalProductService.cs**

  Permite la creación y administración de productos en la plataforma de PayPal.

- **PayPalSubscriptionService.cs**
  Gestiona la creación de suscripciones y genera enlaces de pago.
  
---

