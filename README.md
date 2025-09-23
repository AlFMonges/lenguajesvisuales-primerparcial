ApiMyStore
📌 Descripción del proyecto

ApiMyStore es un servicio web RESTful desarrollado en ASP.NET Core que permite la gestión de un sistema de inventario y órdenes.
Incluye autenticación basada en JWT para proteger los recursos sensibles y proporciona operaciones CRUD para productos, categorías y órdenes de compra.

El sistema está diseñado para servir como backend de aplicaciones de e-commerce o de gestión empresarial, facilitando el manejo de inventario, registro de usuarios, login seguro y procesamiento de pedidos.

⚙️ Instrucciones de instalación y ejecución
1. Clonar el repositorio

       git clone https://github.com/AlFMonges/lenguajesvisuales-primerparcial

       cd ApiMyStore

2. Configurar la base de datos

Editar el archivo appsettings.json para establecer la cadena de conexión a SQL Server (o la base de datos que utilices):

      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=ApiMyStoreDb;Trusted_Connection=True;TrustServerCertificate=True;"
      }

3. Aplicar migraciones y crear la base de datos
   
        dotnet ef database update

5. Ejecutar la aplicación

        dotnet run

🔑 Datos de prueba (usuarios de login)

Para las pruebas es necesaria la creacion de usuario para la autenticación JWT.
La misma puede realizarse desde los endpoints que se encuentran en:

    POST /api/usuarios
