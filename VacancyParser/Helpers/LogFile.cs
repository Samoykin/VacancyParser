namespace VacancyParser.Helpers
{
    using System.IO;

    /// <summary>Логгер.</summary>
    public class LogFile
    {
        private string path = @"log.txt";

        /// <summary>Записать в лог.</summary>
        /// <param name="str">Строка.</param>
        public void WriteLog(string str)
        {
            using (var sw = new StreamWriter(this.path, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(str);
            }
        }
    }
}