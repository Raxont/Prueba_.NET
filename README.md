# API RESTful para Gestión de Vuelos

Este proyecto es una API RESTful desarrollada con ASP.NET Core que permite la gestión de vuelos. La aplicación utiliza un enfoque de arquitectura MVC (Modelo-Vista-Controlador) y Entity Framework Core para interactuar con una base de datos SQL Server.

## Tecnologías usadas

**Server:** ASP.NET, C#, SQL Server y Entity Framework Core (ORM)

## Instalación del Proyecto

Para instalar y ejecutar este proyecto en tu máquina local, sigue estos pasos:

### Requisitos Previos

Asegúrate de tener instalados los siguientes componentes en tu sistema:

- [.NET SDK](https://dotnet.microsoft.com/download) (versión 6.0 o superior)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) o [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads#express)

### Clonar el Repositorio

1. Clona el repositorio en tu máquina local:
   ```bash
   git clone https://github.com/Raxont/Prueba_.NET.git

### Navega a la carpeta del proyecto

1. Navega a la carpeta /api

   ```bash
   cd ./api
   ```

### Configura la base de datos localmente

1. En appsettings.json modifica el DefaultConnection con su servidor de preferencia o utiliza el que tiene por default que es localmente

   ```json
   "DefaultConnection": "Server={"Tu servidor preferido"};Database={"Nombre base de datos"};Trusted_Connection=True;MultipleActiveResultSets=true"
   ```

2. Realiza la migracion de la base de datos

   ```bash
   dotnet ef migrations add {"Coloca el nombre de su preferencia"}
   ```

3. Aplica las migraciones para crear la estructura de la base de datos:

   ```
   dotnet ef database update
   ```

## Ejecución del proyecto

Para ejecutar mi proyecto utiliza

```bash
dotnet run
```

## Estructura de carpetas

```
|   .gitignore
|   Prueba_.sln
|   README.md
\---api
    |   api.csproj
    |   api.http
    |   appsettings.Development.json
    |   appsettings.json
    |   Program.cs
    |                 
    +---Controllers
    |       FlightController.cs
    |       JourneyController.cs
    |       TransportController.cs
    |       
    +---Data
    |       ApplicationDBContext.cs
    |       
    +---Dto
    |       FlightDto.cs
    |       JourneyDto.cs
    |       TransportDto.cs
    |       
    +---Models
    |       Flight.cs
    |       Journey.cs
    |       JourneyFlight.cs
    |       Transport.cs
    |                         
    +---Properties
    |       launchSettings.json
    |       
    \---Services
            FlightService.cs
```

## Controladores

### FlightController.cs

El controlador `FlightController` maneja las operaciones relacionadas con la gestión de vuelos en la API. A continuación, se describen los métodos disponibles:

#### Espacios de Nombres Utilizados

- `Microsoft.AspNetCore.Mvc`: Proporciona funcionalidades para desarrollar aplicaciones web con ASP.NET Core MVC.
- `api.Data`: Contiene el contexto de la base de datos.
- `api.Models`: Contiene los modelos de la API.
- `Microsoft.EntityFrameworkCore`: Proporciona funcionalidades para trabajar con Entity Framework Core.

#### Rutas del Controlador

- `[Route("api/[controller]")]`: Define la ruta base para las solicitudes, donde `[controller]` se reemplaza con el nombre del controlador (`Flight`).

#### Métodos

1. **GetFlights**

   - **Ruta**: `GET /api/flight`

   - **Descripción**: Obtiene la lista de todos los vuelos.

   - **Respuesta**: Devuelve un `IEnumerable<Flight>` con la lista de vuelos.

   - **Ejemplo de datos mostrados**:

     ```json
     Respuesta: {
       "$id": "1",
       "$values": [
         {
           "$id": "2",
           "id": 1,
           "origin": "BGA",
           "destination": "BTA",
           "price": 1000,
           "transportId": 1,
           "transport": null
         }
       ]
     }
     ```

2. **GetFlight**

   - **Ruta**: `GET /api/flight/{id}`

   - **Descripción**: Obtiene un vuelo específico basado en el `id` proporcionado.

   - **Respuesta**: Devuelve un objeto `Flight` si se encuentra, de lo contrario, devuelve un `404 Not Found`.

   - **Ejemplo de datos mostrados**:

     ```json
     Parametro: id = 1;
     Respuesta: {
       "$id": "1",
       "id": 1,
       "origin": "BGA",
       "destination": "BTA",
       "price": 1000,
       "transportId": 1,
       "transport": null
     }
     ```

3. **PostFlight**

   - **Ruta**: `POST /api/flight`

   - **Descripción**: Crea un nuevo vuelo.

   - **Respuesta**: Devuelve un `201 Created` con la ubicación del recurso creado si la operación es exitosa.

   - **Ejemplo de datos mostrados**:

     ```
     Body: {
       "origin": "CTG",
       "destination": "BGA",
       "price": 3000,
       "transportId": 19,
       "transport": {
         "flightCarrier": "AV",
         "flightNumber": "9090"
       }
     }
     Respuesta:{
       "$id": "1",
       "id": 6,
       "origin": "CTG",
       "destination": "BGA",
       "price": 3000,
       "transportId": 19,
       "transport": {
         "$id": "2",
         "id": 19,
         "flightCarrier": "AV",
         "flightNumber": "9090"
       }
     }
     ```

4. **PutFlight**

   - **Ruta**: `PUT /api/flight/{id}`
   - **Descripción**: Actualiza un vuelo existente basado en el `id` proporcionado.
   - **Respuesta**: Devuelve un `204 No Content` si la actualización es exitosa, `400 Bad Request` si los IDs no coinciden, o `404 Not Found` si el vuelo no existe.
   - **Ejemplo de datos mostrados**:

   ```json
   Body: {
     "id": 6,
     "origin": "CTG",
     "destination": "BGA",
     "price": 3100,
     "transportId": 20,
     "transport": {
       "flightCarrier": "AV",
       "flightNumber": "9090"
      }
   }
   ```

5. **DeleteFlight**

   - **Ruta**: `DELETE /api/flight/{id}`
   
   - **Descripción**: Elimina un vuelo existente basado en el `id` proporcionado.
   
   - **Respuesta**: Devuelve un `204 No Content` si la eliminación es exitosa, o `404 Not Found` si el vuelo no se encuentra.
   
   - **Ejemplo de datos mostrados**:
   
     ```json
     Parametro: id = 6;
     ```

#### Inyección de Dependencias

El controlador utiliza inyección de dependencias para obtener una instancia del `AppDbContext`, que se usa para interactuar con la base de datos.

## JourneyController.cs

El controlador `JourneyController` maneja las operaciones relacionadas con la gestión de viajes en la API. A continuación, se describen los métodos y funcionalidades disponibles:

#### Espacios de Nombres Utilizados

- `Microsoft.AspNetCore.Mvc`: Proporciona funcionalidades para desarrollar aplicaciones web con ASP.NET Core MVC.
- `api.Data`: Contiene el contexto de la base de datos.
- `api.Models`: Contiene los modelos de la API.
- `api.Services`: Contiene servicios auxiliares utilizados en la aplicación.
- `Microsoft.EntityFrameworkCore`: Proporciona funcionalidades para trabajar con Entity Framework Core.
- `Microsoft.Extensions.Logging`: Proporciona soporte para registro de logs.

#### Rutas del Controlador

- `[Route("api/[controller]")]`: Define la ruta base para las solicitudes, donde `[controller]` se reemplaza con el nombre del controlador (`Journey`).

#### Métodos

1. **CalculateJourney**

   - **Ruta**: `GET /api/journey/calculate`

   - **Descripción**: Calcula un viaje entre un origen y un destino especificados.

   - **Lógica**: 
     - Verifica si la ruta ya existe en la base de datos.
     - Si no existe, utiliza el servicio `FlightService` para obtener los vuelos disponibles y calcula una posible ruta.
     - Almacena el nuevo viaje en la base de datos si se encuentra una ruta válida.
     
   - **Respuesta**: Devuelve un objeto `Journey` con los detalles del viaje encontrado o un `404 Not Found` si no hay rutas disponibles.

   - **Ejemplo de datos mostrados**:

     ```json
     Parametros: origin=BGA&destination=BTA
     Respuesta: {
       "$id": "1",
       "id": 4,
       "origin": "BGA",
       "destination": "BTA",
       "price": 1000,
       "journeyFlights": {
         "$id": "2",
         "$values": [
           {
             "$id": "3",
             "journeyId": 4,
             "journey": {
               "$ref": "1"
             },
             "flightId": 9,
             "flight": {
               "$id": "4",
               "id": 9,
               "origin": "BGA",
               "destination": "BTA",
               "price": 1000,
               "transportId": 1,
               "transport": {
                 "$id": "5",
                 "id": 1,
                 "flightCarrier": "AV",
                 "flightNumber": "8020"
               }
             }
           }
         ]
       }
     }
     ```

2. **DeleteJourney**

   - **Ruta**: `DELETE /api/journey/{id}`
   
   - **Descripción**: Elimina un viaje existente identificado por el `id`.
   
   - **Respuesta**: Devuelve un `204 No Content` si la eliminación es exitosa o `404 Not Found` si el viaje no se encuentra.
   
   - **Ejemplo de datos mostrados**:
   
     ```json
     Parametro : id = 1;
     ```

#### Inyección de Dependencias

El controlador utiliza inyección de dependencias para obtener instancias de:
- `FlightService`: Para manejar la lógica de los vuelos.
- `AppDbContext`: Para interactuar con la base de datos.
- `ILogger<JourneyController>`: Para registrar información y errores.

#### Lógica de Negocio Privada

El controlador incluye métodos privados para calcular rutas y buscar viajes:

1. **FindJourney**
   - **Descripción**: Encuentra una posible ruta para el viaje utilizando una lista de vuelos disponible.
   - **Lógica**: 
     - Crea un nuevo objeto `Journey` y busca vuelos que conecten el origen con el destino, teniendo en cuenta un número máximo de vuelos permitidos.
     - Calcula el precio total del viaje sumando los precios de los vuelos incluidos.
   
2. **FindRoute**
   - **Descripción**: Método recursivo para encontrar una ruta válida entre el origen y el destino.
   - **Lógica**: 
     - Realiza una búsqueda recursiva que intenta conectar vuelos para llegar al destino, evitando ciclos y respetando un límite en la cantidad de vuelos.

#### Registro de Logs

El controlador usa `_logger` para registrar información clave y advertencias en los métodos, lo que facilita el seguimiento de eventos importantes y errores en la aplicación.

## TransportController.cs

El controlador `TransportController` maneja las operaciones relacionadas con la gestión de transportes en la API. A continuación, se describen los métodos y funcionalidades disponibles:

#### Espacios de Nombres Utilizados

- `Microsoft.AspNetCore.Mvc`: Proporciona funcionalidades para desarrollar aplicaciones web con ASP.NET Core MVC.
- `api.Data`: Contiene el contexto de la base de datos.
- `api.Models`: Contiene los modelos de la API.
- `Microsoft.EntityFrameworkCore`: Proporciona funcionalidades para trabajar con Entity Framework Core.

#### Rutas del Controlador

- `[Route("api/[controller]")]`: Define la ruta base para las solicitudes, donde `[controller]` se reemplaza con el nombre del controlador (`Transport`).

#### Métodos

1. **GetTransports**

   - **Ruta**: `GET /api/transport`

   - **Descripción**: Obtiene la lista de todos los transportes disponibles en la base de datos.

   - **Respuesta**: Devuelve un listado de objetos `Transport` o una lista vacía si no existen registros.

   - **Ejemplo de datos mostrados**:

     ```json
     {
       "$id": "1",
       "$values": [
         {
           "$id": "2",
           "id": 1,
           "flightCarrier": "AV",
           "flightNumber": "8020"
         },
         {
           "$id": "3",
           "id": 2,
           "flightCarrier": "AV",
           "flightNumber": "8030"
         },...
        ]
     }
     ```

2. **GetTransport**

   - **Ruta**: `GET /api/transport/{id}`

   - **Descripción**: Obtiene un transporte específico identificado por su `id`.

   - **Respuesta**: Devuelve el objeto `Transport` si es encontrado o un `404 Not Found` si no existe.

   - **Ejemplo de datos mostrados**:

     ```json
     Parametro: id = 1;
     Respuesta: {
       "$id": "1",
       "id": 1,
       "flightCarrier": "AV",
       "flightNumber": "8020"
     }
     ```

3. **PostTransport**

   - **Ruta**: `POST /api/transport`

   - **Descripción**: Crea un nuevo transporte en la base de datos.

   - **Lógica**: 
     - Agrega el nuevo objeto `Transport` al contexto y guarda los cambios en la base de datos.
     
   - **Respuesta**: Devuelve un resultado `201 Created` con la ubicación del nuevo recurso o un error si la creación falla.

   - **Ejemplo de datos mostrados**:

     ```
     Body: {
       "flightCarrier": "AV",
       "flightNumber": "9090"
     }
     Respuesta: {
       "$id": "1",
       "id": 18,
       "flightCarrier": "AV",
       "flightNumber": "9090"
     }
     ```

4. **PutTransport**

   - **Ruta**: `PUT /api/transport/{id}`

   - **Descripción**: Actualiza un transporte existente identificado por su `id`.

   - **Lógica**: 
     - Verifica si el `id` proporcionado coincide con el `id` del objeto `Transport` enviado.
     - Marca el objeto como modificado y guarda los cambios en la base de datos.
     - Maneja posibles excepciones de concurrencia si el recurso no existe o ha sido modificado por otro proceso.
     
   - **Respuesta**: Devuelve un `204 No Content` si la actualización es exitosa, `400 Bad Request` si el `id` no coincide, o `404 Not Found` si el transporte no existe.

   - **Ejemplo de datos mostrados**:

     ```
     Parametro: id = 18
     Body: {
       "flightCarrier": "AV",
       "flightNumber": "9091"
     }
     ```

5. **DeleteTransport**

   - **Ruta**: `DELETE /api/transport/{id}`
   
   - **Descripción**: Elimina un transporte específico identificado por su `id`.
   
   - **Lógica**: 
     - Busca el transporte por `id` y lo elimina si se encuentra.
     - Guarda los cambios en la base de datos.
     
   - **Respuesta**: Devuelve un `204 No Content` si la eliminación es exitosa o un `404 Not Found` si el transporte no se encuentra.
   
   - **Ejemplo de datos mostrados**:
   
     ```
     Parametro: id = 18
     ```

#### Inyección de Dependencias

El controlador utiliza inyección de dependencias para obtener una instancia de:
- `AppDbContext`: Para interactuar con la base de datos.

## Usado por:

Este proyecto puede ser usado por la siguiente compañía:

- Biinteli