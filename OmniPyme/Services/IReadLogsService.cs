using System.Text.RegularExpressions;
using OmniPyme.Web.DTOs;

namespace OmniPyme.Web.Services
{
    public interface IReadLogsService
    {
        public List<LogDTO> GetLogs(DateTime? date = null);
    }

    public class ReadPlainTextLogsService : IReadLogsService
    {
        private readonly string _logDirectory = "logs";

        public List<LogDTO> GetLogs(DateTime? date = null)
        {
            string logFileName = date?.ToString("yyyyMMdd") ?? DateTime.Now.ToString("yyyyMMdd");
            string logFilePath = Path.Combine(_logDirectory, $"log{logFileName}.log");

            return GetLogsByPath(logFilePath);
        }

        private List<LogDTO> GetLogsByPath(string logFilePath)
        {
            List<LogDTO> logs = new List<LogDTO>();

            if (!File.Exists(logFilePath))
            {
                return logs;
            }

            using FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader reader = new StreamReader(fs);

            string? line;
            List<string> currentLogLines = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                // Detectar inicio de nueva entrada
                if (Regex.IsMatch(line, @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3} [\+\-]\d{2}:\d{2} \[[A-Z]+\]"))
                {
                    // Procesar log anterior si existía
                    if (currentLogLines.Count > 0)
                    {
                        ParseAndAddLog(currentLogLines, logs);
                        currentLogLines.Clear();
                    }
                }

                currentLogLines.Add(line);
            }

            // Procesar último log pendiente
            if (currentLogLines.Count > 0)
            {
                ParseAndAddLog(currentLogLines, logs);
            }

            return logs.OrderByDescending(x => x.Timestamp).ToList();
        }

        private void ParseAndAddLog(List<string> lines, List<LogDTO> targetList)
        {
            var fullLog = string.Join("\n", lines);
            var match = Regex.Match(lines[0], @"^(?<timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}) [\+\-]\d{2}:\d{2} \[(?<level>[A-Z]+)\] (?<message>.*)");


            if (match.Success)
            {
                var timestamp = DateTime.Parse(match.Groups["timestamp"].Value);
                var level = match.Groups["level"].Value;
                var messageStart = match.Groups["message"].Value;

                // Todo el mensaje: encabezado + posibles líneas adicionales
                var fullMessage = messageStart;
                if (lines.Count > 1)
                {
                    fullMessage += "\n" + string.Join("\n", lines.Skip(1));
                }

                targetList.Add(new LogDTO
                {
                    Timestamp = timestamp,
                    Level = level,
                    Message = fullMessage,
                });
            }
        }
    }
}