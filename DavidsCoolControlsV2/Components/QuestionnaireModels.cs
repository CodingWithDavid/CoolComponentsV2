namespace DavidsCoolControlsV2.Components;

public sealed record QuestionnaireChoice(string Value, string Label, string? Description = null);

public sealed class QuestionnaireItem
{
    public required string Name { get; init; }
    public required string Prompt { get; init; }
    public string? Description { get; init; }
    public required IReadOnlyList<QuestionnaireChoice> Choices { get; init; }
    public bool Multiple { get; init; }
    public bool Required { get; init; } = true;

    /// <summary>When set, a freeform text box is shown below the choices.</summary>
    public string? InputLabel { get; init; }
    public string? InputPlaceholder { get; init; }
}

public sealed class QuestionnaireAnswer
{
    public HashSet<string> Selected { get; } = [];
    public string Text { get; set; } = "";
    public bool Skipped { get; set; }

    public bool HasValue => Selected.Count > 0 || !string.IsNullOrWhiteSpace(Text);
}
