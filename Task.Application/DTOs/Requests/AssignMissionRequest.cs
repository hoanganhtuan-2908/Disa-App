using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs.Requests;

public class AssignMissionRequest
{
    public Guid UserId { get; set; }

    public Guid TripId { get; set; }

    public Guid PlaceId { get; set; }

    public Guid TemplateId { get; set; }
}
