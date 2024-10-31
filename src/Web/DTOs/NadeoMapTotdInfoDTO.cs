using CotdQualifierRank.Domain.DomainPrimitives;

namespace CotdQualifierRank.Web.DTOs;

public class NadeoMapTotdInfoDTO
{
    public int TotdYear { get; set; }
    public int Week { get; set; }
    public List<MapUid>? TotdMaps { get; set; }
}
