namespace OmniPyme.Web.DTOs
{
    public class LogViewerDTO
    {
        public List<LogDTO> Logs { get; set; } = new();
        public DateTime? SelectedDate { get; set; }
    }
}
