namespace Maraudr.EmailSender.Application.Dtos;

public class SendNotificationBatchQuery
{
    public List<string> userData { get; set; }
    public string eventTitle  { get; set; }
    public string eventDescription  { get; set; }
}