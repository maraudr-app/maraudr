using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Planning.Application.DTOs
{
    public class CreateEventDto
    {
        public Guid AssociationId { get; set; }
        public List<Guid> ParticipantsIds { get; set; }
        public DateTime BeginningDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

    }
}
