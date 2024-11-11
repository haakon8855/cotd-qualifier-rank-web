namespace CotdQualifierRank.Application.DTOs;

public class NadeoCompetitionDTO
{
    public int Id { get; set; }
    public string? liveId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int NbPlayers { get; set; }
}