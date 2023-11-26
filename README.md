
# AutomatizacionScriptsBD

Este batch de automatizacion de Scripts de BD te permite ejecutar un folder de scripts .sql en una base de datos, generando un log y reporte de ejecucion.
## Instalacion

Descarga el compilado y modifica el archivo de configuracion

```bash
{
  "ConnectionString": "<Tu ConnectionString>",
  "DirectorioArchivos": "<Tu folder de path scripts>",
  "ProviderSQL": "<1: Mysql | 2: SQL Server | Other: NotImplementedException >"
}
```
Luego ejecuta el archivo .exe y se generaran una carpeta Log y Generado a nivel del proyecto.
