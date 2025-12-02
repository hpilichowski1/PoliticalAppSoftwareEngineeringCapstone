namespace PoliticalAppAPI.DTOs.Reps;

public class RepresentativeDto
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Party { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;

    public double ConsistencyScore { get; set; }
    public int YearsInOffice { get; set; }
    public string Level { get; set; } = "Federal";
}