# PayPalApp - Integración de Pagos y Suscripciones con PayPal

![PayPal Integration](https://www.paypalobjects.com/webstatic/icon/pp258.png)

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
2. Obtenr Client ID y Secret en el Dashboard de PayPal.
3. Configurar web.config en la sección <appSettings>:
   ```
   <appSettings>
    <add key="PayPalClientID" value="TU_CLIENT_ID_AQUI" />
    <add key="PayPalClientSecret" value="TU_SECRET_AQUI" />
    </appSettings>
   ```
### 3️⃣ **Ejecutar la Aplicación**
1. Abrir el proyecto en Visual Studio.
2. Seleccionar PayPalIntegrationApp como proyecto de inicio.
3. Presionar F5 o ejecuta el servidor IIS Express.

---
### 🔄 **Flujo del Sistema**
  1️⃣ Login en PayPal → El usuario ingresa su Client ID y Secret para obtener un AccessToken.
  
  2️⃣ Crear Producto y Plan → Se registran productos en la API de PayPal y se generan planes de suscripción.
  
  3️⃣ Generar Enlace de Pago → Se genera un link de pago para la suscripción del usuario.
  
  4️⃣ Redirección a PayPal → El usuario es llevado a PayPal para autorizar el pago.
  
  5️⃣ Confirmación con Webhook → PayPal envía notificaciones cuando un pago se procesa correctamente.
---
### 📡 **Configuración del Webhook con ngrok**
1. Para probar Webhooks en localhost, es necesario ngrok:
2. Descargar e instalar ngrok.
   Ejecutar este comando (ajustar el puerto según IIS Express):
  ```
   ngrok http 44358
```
3. Copiar la URL pública (https://random-id.ngrok.io/WebhookHandler.aspx).
4. Registrar el Webhook en PayPal en PayPal Developer Console.
---

