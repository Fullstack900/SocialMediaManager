using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace SocialMedia_Backend.Utitlities;

public static class SequentialGuid
{
    private static readonly SequentialGuidValueGenerator _generator = new();

    public static Guid NewGuid() => _generator.Next(null);
}
