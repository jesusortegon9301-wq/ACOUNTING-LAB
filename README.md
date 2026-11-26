# AccountingLab

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![C#](https://img.shields.io/badge/C%23-backend-239120)
![MySQL](https://img.shields.io/badge/MySQL-8.4-4479A1)
![React](https://img.shields.io/badge/React-19-61DAFB)
![TypeScript](https://img.shields.io/badge/TypeScript-5.8-3178C6)

**AccountingLab** es un laboratorio universitario de software orientado
a procesos de información contable empresarial. El proyecto integra una
API REST desarrollada con **C# y .NET 8**, persistencia relacional en
**MySQL** y una aplicación web independiente construida con **React y
TypeScript**.

El objetivo académico es aplicar y documentar **22 patrones de diseño**
en escenarios relacionados con procesos contables, respetando los
principios **SOLID**, la arquitectura limpia, los contratos públicos y
las reglas de integridad del sistema.

> Este repositorio tiene fines académicos y utiliza datos ficticios. No
> está diseñado para procesar información financiera real ni para
> sustituir un sistema contable certificado.

------------------------------------------------------------------------

## Tabla de contenido

-   [Características principales](#características-principales)
-   [Tecnologías](#tecnologías)
-   [Arquitectura](#arquitectura)
-   [Catálogo de patrones](#catálogo-de-patrones)
-   [Reglas técnicas y contables](#reglas-técnicas-y-contables)
-   [Requisitos previos](#requisitos-previos)
-   [Configuración y ejecución con
    Docker](#configuración-y-ejecución-con-docker)
-   [Direcciones locales](#direcciones-locales)
-   [Ejecución del backend sin
    Docker](#ejecución-del-backend-sin-docker)
-   [Pruebas automatizadas](#pruebas-automatizadas)
-   [Frontend](#frontend)
-   [Autenticación y empresa activa](#autenticación-y-empresa-activa)
-   [Estructura del repositorio](#estructura-del-repositorio)
-   [Documentación del proyecto](#documentación-del-proyecto)
-   [Estado de validación](#estado-de-validación)
-   [Seguridad y configuración
    sensible](#seguridad-y-configuración-sensible)
-   [Alcance académico](#alcance-académico)

## Características principales

-   API REST con ASP.NET Core y documentación Swagger/OpenAPI.
-   Separación de responsabilidades mediante Clean Architecture.
-   Persistencia MySQL administrada con Entity Framework Core y
    migraciones.
-   Aplicación web React/TypeScript que consume la API por HTTP.
-   Autenticación y autorización por usuario, membresía, empresa activa
    y rol.
-   Veinte módulos funcionales básicos descritos por el proyecto.
-   Veintidós desafíos académicos de patrones de diseño.
-   Pruebas unitarias, de patrones, arquitectura e integración.
-   Entorno local reproducible mediante Docker Compose.

## Tecnologías

  Componente              Tecnología
  ----------------------- -----------------------------------
  Lenguaje backend        C#
  Plataforma backend      .NET 8 / ASP.NET Core
  Persistencia            Entity Framework Core 8
  Proveedor MySQL         Pomelo.EntityFrameworkCore.MySql
  Base de datos Docker    MySQL 8.4
  Frontend                React 19 y TypeScript
  Herramientas frontend   Vite, Tailwind CSS y React Router
  Contratos HTTP          REST / JSON
  Documentación API       Swagger / OpenAPI
  Contenedores            Docker y Docker Compose v2
  Pruebas backend         xUnit
  Pruebas frontend        npm scripts del proyecto

Las versiones concretas de dependencias se encuentran registradas en los
archivos de proyecto, `packages.lock.json` y
`frontend/package-lock.json`.

## Arquitectura

El backend aplica una separación por capas inspirada en **Clean
Architecture**:

``` text
                    ┌─────────────────────────┐
                    │       Frontend          │
                    │     React / TypeScript  │
                    └────────────┬────────────┘
                                 │ HTTP / JSON
                                 ▼
                    ┌─────────────────────────┐
                    │       API REST          │
                    │ ASP.NET Core / Swagger  │
                    └────────────┬────────────┘
                                 ▼
                    ┌─────────────────────────┐
                    │      Application       │
                    │ Casos de uso y flujos   │
                    └────────────┬────────────┘
                                 ▼
                    ┌─────────────────────────┐
                    │        Domain           │
                    │ Reglas y contratos      │
                    └─────────────────────────┘
                                 ▲
                                 │ Implementa contratos
                    ┌────────────┴────────────┐
                    │     Infrastructure      │
                    │ EF Core, MySQL y        │
                    │ adaptadores técnicos   │
                    └────────────┬────────────┘
                                 ▼
                              MySQL
```

### Responsabilidades por capa

  -----------------------------------------------------------------------
  Capa                                Responsabilidad
  ----------------------------------- -----------------------------------
  `AccountingLab.Domain`              Entidades, objetos de valor, reglas
                                      de negocio, contratos e
                                      invariantes.

  `AccountingLab.Application`         Casos de uso, validación de
                                      solicitudes, coordinación y
                                      orquestación.

  `AccountingLab.Infrastructure`      Persistencia, EF Core, MySQL,
                                      repositorios concretos y
                                      adaptadores técnicos.

  `AccountingLab.Api`                 Endpoints HTTP, autenticación,
                                      autorización, serialización y
                                      respuestas.

  `frontend`                          Interfaz web, navegación,
                                      formularios y consumo HTTP de la
                                      API.
  -----------------------------------------------------------------------

La interfaz web no accede directamente a la base de datos. Las reglas de
negocio no deben depender de controladores ni de detalles de
infraestructura.

## Catálogo de patrones

El proyecto contempla 22 patrones, organizados en las tres familias
clásicas.

### Creacionales

  -----------------------------------------------------------------------
  Código                  Patrón                  Propósito dentro del
                                                  laboratorio
  ----------------------- ----------------------- -----------------------
  DP-01                   Factory Method          Centralizar la creación
                                                  de comprobantes
                                                  contables.

  DP-02                   Abstract Factory        Crear familias
                                                  compatibles de
                                                  servicios contables.

  DP-03                   Builder                 Construir informes u
                                                  objetos complejos
                                                  mediante pasos
                                                  controlados.

  DP-04                   Prototype               Crear nuevos objetos a
                                                  partir de prototipos.

  DP-05                   Singleton               Administrar una
                                                  instancia compartida
                                                  dentro del proceso.
  -----------------------------------------------------------------------

### Estructurales

  -----------------------------------------------------------------------
  Código                  Patrón                  Propósito dentro del
                                                  laboratorio
  ----------------------- ----------------------- -----------------------
  DP-06                   Adapter                 Adaptar interfaces
                                                  externas a contratos
                                                  internos.

  DP-07                   Bridge                  Separar una abstracción
                                                  de sus
                                                  implementaciones.

  DP-08                   Composite               Representar jerarquías,
                                                  como estructuras de
                                                  cuentas.

  DP-09                   Decorator               Incorporar
                                                  responsabilidades
                                                  adicionales a objetos.

  DP-10                   Facade                  Simplificar la
                                                  coordinación de
                                                  procesos contables.

  DP-11                   Flyweight               Compartir metadatos de
                                                  clasificación para
                                                  reducir duplicación.

  DP-12                   Proxy                   Controlar el acceso a
                                                  recursos o reportes
                                                  protegidos.
  -----------------------------------------------------------------------

### De comportamiento

  -----------------------------------------------------------------------
  Código                  Patrón                  Propósito dentro del
                                                  laboratorio
  ----------------------- ----------------------- -----------------------
  DP-13                   Chain of Responsibility Encadenar validaciones
                                                  o etapas de aprobación.

  DP-14                   Command                 Representar operaciones
                                                  como solicitudes
                                                  independientes.

  DP-15                   Iterator                Recorrer colecciones
                                                  sin exponer su
                                                  representación interna.

  DP-16                   Mediator                Coordinar la
                                                  comunicación entre
                                                  componentes.

  DP-17                   Memento                 Conservar y restaurar
                                                  estados controlados.

  DP-18                   Observer                Notificar cambios y
                                                  eventos financieros.

  DP-19                   State                   Variar el
                                                  comportamiento según el
                                                  estado de un proceso.

  DP-20                   Strategy                Intercambiar algoritmos
                                                  o reglas de cálculo.

  DP-21                   Template Method         Estandarizar el flujo
                                                  de conciliación
                                                  bancaria.

  DP-22                   Visitor                 Aplicar operaciones de
                                                  análisis sobre
                                                  documentos
                                                  heterogéneos.
  -----------------------------------------------------------------------

> La ubicación exacta de cada implementación y sus participantes debe
> consultarse en el código fuente y en la documentación específica de
> cada desafío. Los diagramas UML del informe académico son modelos
> conceptuales y deben contrastarse con la versión final del
> repositorio.

## Reglas técnicas y contables

Las implementaciones deben respetar, como mínimo, los siguientes
criterios establecidos para el proyecto:

-   Representar importes monetarios en C# mediante `decimal`; no
    utilizar `float` ni `double` para cálculos financieros.
-   Persistir importes en MySQL mediante columnas `DECIMAL` con
    precisión y escala adecuadas.
-   Mantener la precisión definida por los contratos del proyecto, hasta
    cuatro posiciones decimales en los escenarios establecidos.
-   Evitar pérdidas de precisión en el transporte JSON; los importes se
    manejan como cadenas decimales cuando así lo establece el contrato.
-   Verificar que los débitos y créditos de los asientos estén
    equilibrados.
-   Aislar la información por empresa y comprobar la membresía del
    usuario antes de permitir operaciones.
-   Aplicar autorización de acuerdo con los permisos del rol.
-   Prevenir operaciones duplicadas y operaciones financieras parciales.
-   Conservar la historia de hechos contables ya contabilizados.
-   Ejecutar las operaciones que requieran atomicidad mediante
    transacciones.

## Requisitos previos

Para ejecutar el entorno completo con Docker:

-   Docker Desktop o Docker Engine con Docker Compose v2.
-   Puertos locales disponibles: `5173`, `5208` y `3308`.

Para ejecutar pruebas backend desde Windows:

-   PowerShell 7 (`pwsh`).
-   SDK .NET 8.
-   MySQL desechable para pruebas en el puerto `3309`.

Para ejecutar el frontend fuera de Docker:

-   Node.js 22.12 o superior.
-   npm.

El repositorio puede incluir una instalación local del SDK en
`.tools/dotnet/`. Los scripts del proyecto priorizan esa instalación
cuando está disponible.

## Configuración y ejecución con Docker

Ejecuta los siguientes comandos desde la raíz del repositorio.

### 1. Crear la configuración local

``` powershell
if (-not (Test-Path -LiteralPath .env)) {
    Copy-Item .env.example .env
}
```

Abre `.env` y reemplaza los valores de ejemplo para las contraseñas y la
clave de firma JWT. `JWT_SIGNING_KEY` debe tener al menos 32 caracteres.

Utiliza credenciales locales de desarrollo. No publiques el archivo
`.env`, contraseñas, tokens ni claves privadas.

### 2. Validar la configuración

``` powershell
docker compose --env-file .env config --quiet
```

### 3. Construir e iniciar los servicios

``` powershell
docker compose --env-file .env up --build -d --wait mysql api frontend
```

### 4. Revisar el estado

``` powershell
docker compose --env-file .env ps --all
```

### 5. Detener los servicios

``` powershell
docker compose --env-file .env down
```

Para eliminar también los volúmenes persistentes de Docker se requiere
una acción adicional y deliberada. No elimines volúmenes si necesitas
conservar los datos locales.

## Direcciones locales

  Servicio                Dirección
  ----------------------- ------------------------------------
  Aplicación web          http://localhost:5173
  API                     http://localhost:5208
  Swagger / OpenAPI       http://localhost:5208/swagger
  Health check            http://localhost:5208/health/live
  Readiness check         http://localhost:5208/health/ready
  MySQL de demostración   `127.0.0.1:3308`
  MySQL Server local      `127.0.0.1:3307` (MySQL Workbench)

La base de datos de demostración utiliza el servicio Docker configurado
por el proyecto. El perfil de pruebas utiliza una instancia y una base
separadas. El MySQL Server 8.0.46 local se administra con MySQL Workbench
en el puerto `3307` y es independiente de Docker y del MariaDB de XAMPP en
`3306`. Consulta [MySQL Server local y Workbench](docs/MYSQL_WORKBENCH_LOCAL.md)
para la conexión, la verificación con `SELECT VERSION(), @@port` y la base
desechable de integración.

## Ejecución del backend sin Docker

Configura las variables de entorno requeridas por la API, incluyendo:

-   `ConnectionStrings__AccountingLab`
-   `Demo__Email`
-   `Demo__Password`
-   `Authentication__SigningKey`
-   `Authentication__Issuer=AccountingLab`
-   `Authentication__Audience=AccountingLab.Web`

La cadena de conexión debe apuntar a la base de demostración
correspondiente. En la configuración Docker documentada, MySQL se
publica en el puerto `3308`.

Desde la raíz:

``` powershell
dotnet restore AccountingLab.sln
dotnet run --project src/AccountingLab.Api -- --migrate-only
dotnet run --project src/AccountingLab.Api --urls http://localhost:5208
```

El comando `--migrate-only` aplica el esquema y los datos iniciales y
termina. No ejecutes simultáneamente dos procesos responsables de
aplicar migraciones sobre la misma base de datos.

Si utilizas el SDK incluido en `.tools/dotnet/`, invoca
`.tools/dotnet/dotnet.exe` en lugar de `dotnet`, o agrega esa carpeta al
`PATH` de la sesión.

## Pruebas automatizadas

Las pruebas están organizadas por categorías. Desde PowerShell 7,
ejecuta:

### Pruebas base

``` powershell
pwsh -NoProfile -File scripts/Invoke-Tests.ps1 -Category Base
```

### Pruebas de arquitectura

``` powershell
pwsh -NoProfile -File scripts/Invoke-Tests.ps1 -Category Architecture
```

### Pruebas académicas de los patrones

``` powershell
pwsh -NoProfile -File scripts/Invoke-Tests.ps1 -Category Academic
```

### Prueba de un patrón específico

Ejemplo para DP-03:

``` powershell
pwsh -NoProfile -File scripts/Invoke-Tests.ps1 -Category Academic -ChallengeId DP-03
```

La selección por `ChallengeId` ejecuta únicamente ese desafío; no
reemplaza la ejecución completa.

### Pruebas de integración con MySQL

Las pruebas de integración requieren una base desechable separada de la
base de demostración. El perfil `tests` prepara MySQL en
`127.0.0.1:3309`.

``` powershell
docker compose --env-file .env --profile tests up -d --wait mysql-test
```

Configura la conexión de pruebas en la terminal actual. La contraseña
debe coincidir con `MYSQL_TEST_PASSWORD` definido localmente en `.env`:

``` powershell
$testPassword = Read-Host 'MYSQL_TEST_PASSWORD configurada en .env' -MaskInput
$env:ConnectionStrings__AccountingLabTest = "Server=127.0.0.1;Port=3309;Database=accountinglab_tests;User=accountinglab_test;Password=$testPassword;"
```

Ejecuta las pruebas de integración:

``` powershell
dotnet test tests/AccountingLab.IntegrationTests/AccountingLab.IntegrationTests.csproj --logger "console;verbosity=normal"
```

**Importante:** nunca asignes la cadena de conexión de demostración a
`ConnectionStrings__AccountingLabTest`. Las pruebas deben utilizar
exclusivamente una base desechable cuyo nombre identifique claramente
que es de pruebas.

### Ejecución global y comprobaciones de integridad

``` powershell
npm --prefix frontend ci
pwsh -NoProfile -File scripts/Invoke-Tests.ps1 -Category All -Delivery
pwsh -NoProfile -File scripts/Test-Integrity.ps1
```

Para la comprobación de integridad del escenario de preparación docente:

``` powershell
pwsh -NoProfile -File scripts/Test-Integrity.ps1 -Delivery
```

Los scripts de pruebas conservan resultados TRX, salida de consola,
conteos y código de salida en un directorio nuevo dentro de
`artifacts/test-results/`.

La aprobación de una categoría no demuestra por sí sola la corrección de
todas las capas. Se deben revisar las evidencias de patrones,
arquitectura, integración, frontend y compilación por separado.

## Frontend

Para instalar dependencias y ejecutar pruebas de la interfaz:

``` powershell
npm --prefix frontend ci
npm --prefix frontend test
npm --prefix frontend run build
```

El frontend se comunica con el backend mediante HTTP. No debe acceder
directamente a MySQL ni depender de clases internas del backend.

## Autenticación y empresa activa

El proyecto utiliza autenticación Bearer. El usuario de demostración
documentado es `docente@accountinglab.local`; la contraseña se obtiene
de la variable local `DEMO_PASSWORD`.

Flujo general:

1.  El usuario inicia sesión mediante el endpoint de autenticación.
2.  La API devuelve el token y las empresas a las que el usuario tiene
    acceso.
3.  Las solicitudes protegidas envían `Authorization: Bearer <token>`.
4.  Las operaciones de negocio incluyen el encabezado `X-Company-Id`.
5.  El backend valida la membresía y los permisos antes de ejecutar la
    operación.

No publiques credenciales reales ni tokens en capturas, ejemplos, issues
o commits.

## Estructura del repositorio

Estructura lógica principal:

``` text
AccountingLab/
├── src/
│   ├── AccountingLab.Domain/
│   ├── AccountingLab.Application/
│   ├── AccountingLab.Infrastructure/
│   └── AccountingLab.Api/
├── tests/
│   ├── AccountingLab.PatternChallenges/
│   └── AccountingLab.IntegrationTests/
├── frontend/
├── docs/
├── scripts/
├── artifacts/
├── AccountingLab.sln
├── docker-compose.yml
├── .env.example
├── global.json
└── README.md
```

La estructura puede contener otros proyectos, archivos de configuración
y recursos adicionales. Consulta el repositorio para conocer el árbol
completo vigente.

## Documentación del proyecto

La documentación complementaria se encuentra en `docs/`:

-   [Actividad del estudiante](docs/ACTIVIDAD_ESTUDIANTES.md)
-   [Arquitectura](docs/ARQUITECTURA.md)
-   [Contratos de API](docs/CONTRATOS_API.md)
-   [Catálogo de los 22 patrones](docs/CATALOGO_22_PATRONES.md)
-   [Matriz de trazabilidad](docs/MATRIZ_TRAZABILIDAD.md)
-   [Criterios de evaluación](docs/CRITERIOS_EVALUACION.md)
-   [Política de uso de inteligencia
    artificial](docs/POLITICA_USO_IA.md)
-   [Estado y evidencia de entrega](docs/ENTREGA.md)
-   [MySQL Server local y Workbench (puerto 3307)](docs/MYSQL_WORKBENCH_LOCAL.md)
-   [Informe técnico y académico (Markdown)](docs/INFORME_TECNICO_ACADEMICO.md)
-   [Informe maestro original (DOCX)](docs/Informe_Maestro_AccountingLab_2026_Unificado.docx)

El [informe técnico y académico](docs/INFORME_TECNICO_ACADEMICO.md) es la
versión en Markdown del documento Word unificado: contiene el contexto,
las 17 secciones, los 22 patrones con sus diagramas UML y el estado de
validación. Los diagramas son modelos conceptuales y deben contrastarse
con el código definitivo del repositorio.

## Estado de validación

El estado de validación debe actualizarse con los resultados reales de
cada ejecución. No se debe marcar una suite como aprobada basándose
únicamente en ejecuciones parciales o en resultados históricos.

Al momento de consolidar la documentación del proyecto se registraron
los siguientes puntos:

-   Se comunicaron avances satisfactorios en pruebas de varios desafíos
    académicos.
-   Se reportaron ejecuciones satisfactorias de pruebas de DP-21
    (Template Method) y DP-22 (Visitor).
-   En una revisión de integración se reportaron 76 fallos distribuidos
    entre `BaseRoutesTests`, `ConflictResponsesTests`,
    `FinancialGuaranteesTests`, `IdentityAndRuntimeTests`,
    `PendingTransportTests` y `PublishedDraftGuardTests`.
-   Una ejecución filtrada de integración se detuvo durante la
    inicialización porque no estaba definida
    `ConnectionStrings__AccountingLabTest`; por tanto, esa ejecución no
    llegó a validar el endpoint.
-   La suite completa debe volver a ejecutarse después de configurar
    correctamente MySQL de pruebas y resolver los fallos encontrados.

Estos datos son un registro del proceso de validación, no una afirmación
de que el estado actual del repositorio sea exitoso o fallido en su
totalidad. Antes de publicar una versión de entrega, actualiza esta
sección con la fecha, el comando ejecutado, el total de pruebas,
aprobadas, fallidas y omitidas, y adjunta los reportes TRX
correspondientes.

## Seguridad y configuración sensible

-   No subas `.env` al repositorio.
-   Mantén `.env.example` sin contraseñas reales, tokens ni claves
    privadas.
-   No publiques cadenas de conexión con credenciales.
-   No reutilices la base de demostración para pruebas de integración.
-   Utiliza datos ficticios en desarrollo y documentación.
-   No incluyas información personal, financiera o empresarial real en
    evidencias.
-   Revisa los cambios antes de hacer `git commit` y `git push`.

## Alcance académico

AccountingLab es un laboratorio para estudiar arquitectura de software,
patrones de diseño, reglas de dominio, persistencia y pruebas
automatizadas en un contexto contable ficticio.

El trabajo de los 22 patrones debe justificar para cada caso:

1.  El problema de diseño que se busca resolver.
2.  La intención del patrón y la razón de su selección.
3.  Las clases, interfaces y responsabilidades participantes.
4.  El flujo de colaboración entre los componentes.
5.  La relación con SOLID y Clean Architecture.
6.  Las pruebas y escenarios de validación.
7.  Las ventajas, costos y limitaciones de la solución.
8.  El diagrama UML correspondiente, contrastado con el código
    implementado.

------------------------------------------------------------------------

**Proyecto académico AccountingLab --- 2026**
