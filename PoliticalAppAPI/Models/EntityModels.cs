
namespace PoliticalAppAPI.Models
{
    public record Uuid(string Value)
    {
        public static implicit operator string(Uuid id) => id.Value;
        public static implicit operator Uuid(string v) => new(v);
    }
}
