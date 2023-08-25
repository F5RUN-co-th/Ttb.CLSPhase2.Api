using System;
using System.Collections.Generic;

namespace CLSPhase2.Dal.Entities;

public partial class TempRequestBatch
{
    public int Id { get; set; }

    public string ReferenceCode { get; set; } = null!;

    public string? JsonDocOutboundRequest { get; set; }

    public string? JsonDocOutboundResponse { get; set; }

    public DateOnly? CreatedAt { get; set; }
}
