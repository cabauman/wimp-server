using System;
using WIMP_Server.Dtos.Esi;

namespace WIMP_Server.Dtos;

public class ReadIntelDto
{
    public int Id { get; set; }

    public EsiNameIdPairDto Character { get; set; }

    public EsiNameIdPairDto StarSystem { get; set; }

    public EsiNameIdPairDto Ship { get; set; }

    public DateTime Timestamp { get; set; }

    public bool IsSpike { get; set; }

    public bool IsClear { get; set; }
}
