﻿namespace CotdQualifierRank.Web.DTOs;

public class NadeoAuthTokens
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AuthTime { get; set; }
}
