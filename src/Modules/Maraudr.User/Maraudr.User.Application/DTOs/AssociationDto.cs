namespace Application.DTOs;

public class AssociationDto
{
    public Guid id { get; set; }
    public Guid managerId { get; set; }
    public List<string> members { get; set; }
    public string country { get; set; }
    public string city { get; set; }
    public string name { get; set; }
}