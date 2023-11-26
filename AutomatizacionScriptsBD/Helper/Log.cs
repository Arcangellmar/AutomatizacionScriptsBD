namespace AutomatizacionScriptsBD.Helper
{
    public class Log
    {
        public static void RegistroLog(String message)
        {
            try
            {
                String nameFile = DateTime.Now.ToString("dd-MM-yyyy");
                String baseAppDirectory = AppDomain.CurrentDomain.BaseDirectory;
                String dirLog = "Logs";

                String path = Path.Combine(baseAppDirectory, dirLog, nameFile + ".txt");

                if (!Directory.Exists(Path.Combine(baseAppDirectory, dirLog))) Directory.CreateDirectory(Path.Combine(baseAppDirectory, dirLog));

                if (!System.IO.File.Exists(path)) System.IO.File.Create(path).Dispose();

                StreamWriter file;
                file = System.IO.File.AppendText(path);
                file.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                file.WriteLine(message);
                file.WriteLine();
                file.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
