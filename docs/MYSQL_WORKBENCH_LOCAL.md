# MySQL Server local y MySQL Workbench (puerto 3307)

Este documento registra el entorno MySQL que utilizó el estudiante para el
laboratorio, además del entorno Docker descrito en el README. **MySQL
Workbench es el cliente gráfico y MySQL Server es el motor de base de
datos**: son dos piezas distintas que trabajan juntas.

## 1. Servicios que pueden existir en el equipo

| Elemento | Qué es | Puerto | Uso en AccountingLab |
|---|---|---|---|
| MySQL Workbench | Cliente gráfico (GUI) para administrar MySQL | No escucha; se conecta a un servidor | Crear la conexión, ejecutar SQL y verificar versión/puerto |
| MySQL Server 8.0.46 local | Motor de base de datos instalado de forma independiente | **3307** | Servidor utilizado por el estudiante con Workbench |
| MariaDB de XAMPP 10.4.32 | Otro motor ya instalado en el equipo | 3306 | No debe confundirse con MySQL Server 8.0.46 |
| MySQL 8.4 de Docker | Servicio del `docker-compose.yml` del proyecto | 3308 (demo) y 3309 (pruebas) | Entorno reproducible documentado en el README |

> Regla: una misma cadena de conexión debe apuntar a **un solo** servidor.
> No mezclar la configuración local de 3307 con los puertos Docker 3308/3309.

## 2. Conexión en MySQL Workbench

En Workbench: **Database → Manage Connections → New Connection** y usar:

| Campo | Valor |
|---|---|
| Connection Name | `AccountingLab MySQL Server 8.0.46` |
| Connection Method | Standard TCP/IP |
| Hostname | `127.0.0.1` |
| Port | **3307** |
| Username | `root` |
| Password | La contraseña definida localmente; **no escribirla en el repositorio** |
| SSL | `Not Enabled` para el entorno local, si así quedó configurado |

Probar la conexión con **Test Connection** antes de ejecutar migraciones o
pruebas.

## 3. Verificación del servidor

En una pestaña SQL de Workbench ejecutar:

```sql
SELECT VERSION(), @@port, @@version_comment;
```

- Resultado esperado: versión `8.0.46` y puerto `3307`.
- Si devuelve `10.4.32-MariaDB`, la conexión está apuntando a XAMPP (3306),
  no al MySQL Server independiente.

Conservar esa salida como evidencia: la versión y el puerto fueron
informados/configurados por el estudiante, pero la comprobación SQL es la
que cierra la verificación (ver Anexo D del
[informe técnico](INFORME_TECNICO_ACADEMICO.md)).

## 4. Base desechable para integración

```sql
CREATE DATABASE IF NOT EXISTS accountinglab_test;
```

El fixture de pruebas exige que el nombre de la base contenga la palabra
`test`. Nunca apuntar la conexión de pruebas a la base de demostración
`accountinglab` ni a una base productiva.

## 5. Variable de entorno para pruebas (PowerShell)

Solo en la ventana de PowerShell donde se ejecutarán las pruebas:

```powershell
$testPassword = Read-Host 'Contraseña local de MySQL Server 8.0.46' -MaskInput
$env:ConnectionStrings__AccountingLabTest = "Server=127.0.0.1;Port=3307;Database=accountinglab_test;User ID=root;Password=$testPassword;"
```

Verificar que la variable existe sin mostrar la contraseña:

```powershell
if ($env:ConnectionStrings__AccountingLabTest) {
    "ConnectionStrings__AccountingLabTest está configurada."
} else {
    "Falta ConnectionStrings__AccountingLabTest."
}
```

Luego ejecutar la prueba filtrada:

```powershell
dotnet test tests/AccountingLab.IntegrationTests/AccountingLab.IntegrationTests.csproj `
  --filter "FullyQualifiedName~CompanyCreation_AddsMembershipWithoutAssumingSeedOrder" `
  --logger "console;verbosity=detailed"
```

Si se usa en su lugar el perfil Docker del README, la conexión es
`127.0.0.1:3309` con el usuario `accountinglab_test` y la base
`accountinglab_tests`. No combinar ambas configuraciones.

## 6. Compatibilidad del proveedor EF Core

`MySqlApiFixture` configura Pomelo con
`new MySqlServerVersion(new Version(8, 4, 0))`, porque la línea base del
proyecto usa MySQL 8.4 en Docker. Antes de ejecutar integración contra el
servidor local 8.0.46 en el puerto 3307, revisar esa diferencia y alinear
el proveedor con la versión real del servidor seleccionado. No declarar
compatibilidad por suposición: confirmar servidor, migrar y ejecutar la
suite.

## 7. Seguridad

- No publicar contraseñas, tokens, `JWT_SIGNING_KEY` ni cadenas de conexión
  completas.
- No subir `.env` al repositorio; `.env.example` solo contiene marcadores.
- No incluir capturas de Workbench donde se vean credenciales o datos reales.
- Usar exclusivamente datos ficticios en documentación y evidencias.
