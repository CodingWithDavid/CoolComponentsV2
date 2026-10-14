namespace DavidsCoolControlsV2.Components;

public sealed record CarouselSlide(
    string ImageUrl,
    string AltText,
    string Title,
    string Caption,
    string Credit,
    string License,
    string SourceUrl);
