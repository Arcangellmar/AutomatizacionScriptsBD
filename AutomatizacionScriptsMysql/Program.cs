using AutomatizacionScriptsMysql.Helper;
using MySql.Data.MySqlClient;
using System.Text;
using System.Text.Json;

// Variables globales
String numeroEjecucionesGlobal = "";

try
{

    // Obtengo datos de configuracion

    // Ruta del archivo JSON
    String rutaBase = AppDomain.CurrentDomain.BaseDirectory;
    string rutaArchivo = Path.Combine(rutaBase, "config.json");

    // Lee todo el contenido del archivo JSON
    string contenidoJson = File.ReadAllText(rutaArchivo);

    // Deserializa el contenido JSON en un objeto dynamic
    Config? datos = JsonSerializer.Deserialize<Config?>(contenidoJson);

    if (datos == null) throw new Exception("Configuracion no encontrada");

    string? ServerDB = datos.ServerDB;
    string? DatabaseBD = datos.DatabaseBD;
    string? UserIDDB = datos.UserIDDB;
    string? PasswordDB = datos.PasswordDB;
    string? DirectorioArchivos = datos.DirectorioArchivos;


    // Validaciones iniciales
    if (String.IsNullOrEmpty(DirectorioArchivos)) throw new Exception("Directorio de archivos vacio o nulo");
    if (String.IsNullOrEmpty(ServerDB)) throw new Exception("Servidor BD vacio o nulo");
    if (String.IsNullOrEmpty(DatabaseBD)) throw new Exception("Base de datos vacia o nulo");
    if (String.IsNullOrEmpty(UserIDDB) || String.IsNullOrEmpty(PasswordDB)) throw new Exception("Usuario o contraseña vacia o nula");


    // Generar output
    bool directoriosCreados = CrearDirectorioGenerado();
    if (!directoriosCreados) throw new Exception("No se creo el directorio de generados");


    // Obtener lista de archivos
    DirectoryInfo directorioPase = new(DirectorioArchivos);
    FileInfo[] archivos = directorioPase.GetFiles("*.sql");

    // Ordenar alpha numericamente
    Array.Sort(archivos, (a, b) => AlphaNumericOrden.AlphanumericCompare(a.Name, b.Name));

    // Contador por ejecucion
    int cantidadArchivos = archivos.Length;
    int contadorExitoso = 0;

    // Ejecutamos todos los archivos
    foreach (FileInfo archivo in archivos)
    {

        try
        {
            // Leemos el script a ejecutar
            string script = File.ReadAllText(archivo.FullName, Encoding.Default);

            MySqlConnectionStringBuilder connectionBuilder = new()
            {
                Server = ServerDB,
                Database = DatabaseBD,
                UserID = UserIDDB,
                Password = PasswordDB,
                //SslMode = MySqlSslMode.Required
            };

            using (MySqlConnection cn = new(connectionBuilder.ConnectionString))
            {
                cn.Open();

                // Lo ejecutamos como transaccion en caso de error hacemos rollback
                using (MySqlTransaction transaction = cn.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand command = new MySqlCommand(script, cn))
                        {
                            command.Transaction = transaction;

                            command.ExecuteNonQuery();

                            transaction.Commit();
                            transaction.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        transaction.Dispose();

                        throw new Exception(ex.Message);
                    }
                }


                cn.Close();
                cn.Dispose();
            }

            Log.RegistroLog("Archivo ejecutado correctamente: " + archivo.FullName);

            CrearGenerado(true, archivo);
            contadorExitoso++;
        }
        catch (Exception ex)
        {
            Log.RegistroLog("Error en archivo: " + archivo.FullName + "\nError al registrar script: " + ex.Message);
            CrearGenerado(false, archivo);
        }

    }

    // Cerramos la ejecucion
    Log.RegistroLog("Ruta ejecutada " + directorioPase.FullName + "\nTotal de archivos completados exitosamente: " + contadorExitoso + " / " + cantidadArchivos);

}
catch (Exception ex)
{
    Log.RegistroLog(ex.Message);
}

Log.RegistroLog("*************************************************************************\n");

bool CrearDirectorioGenerado()
{
    try
    {
        String folderTemp = AppDomain.CurrentDomain.BaseDirectory;

        String dirLogExit = "Generado/Exitoso";
        String dirLogFall = "Generado/Fallido";

        if (!Directory.Exists(Path.Combine(folderTemp, dirLogExit))) Directory.CreateDirectory(Path.Combine(folderTemp, dirLogExit));

        if (!Directory.Exists(Path.Combine(folderTemp, dirLogFall))) Directory.CreateDirectory(Path.Combine(folderTemp, dirLogFall));

        DirectoryInfo directory = new(Path.Combine(folderTemp, dirLogExit));
        DirectoryInfo[] archivos = directory.GetDirectories();
        numeroEjecucionesGlobal = archivos.Length.ToString();

        if (!Directory.Exists(Path.Combine(folderTemp, dirLogExit, numeroEjecucionesGlobal))) Directory.CreateDirectory(Path.Combine(folderTemp, dirLogExit, numeroEjecucionesGlobal));
        if (!Directory.Exists(Path.Combine(folderTemp, dirLogFall, numeroEjecucionesGlobal))) Directory.CreateDirectory(Path.Combine(folderTemp, dirLogFall, numeroEjecucionesGlobal));

        return true;
    }
    catch (Exception ex)
    {
        Log.RegistroLog("Error al crear directorios generados: " + ex.Message);
        return false;
    }
}
void CrearGenerado(bool exito, FileInfo archivo)
{
    try
    {
        String baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        String dirLog = exito ? "Generado/Exitoso" : "Generado/Fallido";

        String path = Path.Combine(baseDirectory, dirLog + "/" + numeroEjecucionesGlobal, archivo.Name);

        System.IO.File.Copy(archivo.FullName, path);
    }
    catch (Exception ex)
    {
        Log.RegistroLog("Error al crear archivos generados " + ex.Message);
    }
}