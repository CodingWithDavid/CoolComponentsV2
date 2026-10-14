# David's Cool Components v2

A standalone .NET 10 Blazor WebAssembly solution styled after the supplied
`modern_blue_poc_home_page.html` reference. Open `DavidsCoolControlsV2.sln` in Visual Studio
with the ASP.NET and web development workload, or use the .NET 10 SDK.

## Run

```powershell
dotnet run --project .\DavidsCoolControlsV2 --launch-profile http
```

Open `http://localhost:5298`. All application code runs in the browser; no
ASP.NET Core backend is included.

## Build and publish

```powershell
dotnet build .\DavidsCoolControlsV2.sln
dotnet publish .\DavidsCoolControlsV2 -c Release
```

Deploy the contents of `DavidsCoolControlsV2\bin\Release\net10.0\publish\wwwroot` to a
static web host. Configure a fallback to `index.html` for client-side routes.
For subdirectory hosting, update the base href in `wwwroot\index.html`.

## Design and behavior

- Blue gradient hero with a Batman questionnaire, and a dark footer. The menu
  links to the Questionnaire (home), Carousel, Credit Card, and Hover Card pages.
- Responsive desktop/mobile navigation, keyboard focus styles, accessible
  progress indicators, and reduced-motion support.
- Footer information pages explain the boundaries of the proof of concept.

Content, metrics, and certification claims are illustrative
material from the supplied reference, not verified enterprise capabilities.
Connect real services and replace demonstration claims before production use.

Styling lives in `DavidsCoolControlsV2\wwwroot\css\app.css` with no Tailwind or Bootstrap
dependency. Google Fonts supplies the reference's Inter typeface; local
fallbacks are used if it is unavailable. Initial avatars and icons are rendered
locally, with no external image dependency.

## Questionnaire

`Components\Questionnaire.razor` is a reusable, step-by-step questionnaire
inspired by the shadcn/ui questionnaire. The home page (`Pages\Home.razor`) uses
it for a six-question Batman quiz with a scored results screen.

```razor
<Questionnaire Items="Questions" SubmitText="Reveal my score" OnSubmit="ShowResults" />

@code {
    private static readonly QuestionnaireItem[] Questions =
    [
        new() { Name = "city", Prompt = "Which city does Batman protect?",
                Choices = [new("gotham", "Gotham City"), new("metropolis", "Metropolis")] },
        new() { Name = "robins", Prompt = "Who has worn the Robin costume?", Multiple = true,
                Choices = [new("dick", "Dick Grayson"), new("jason", "Jason Todd")] },
        new() { Name = "villain", Prompt = "Greatest villain?", Required = false,
                InputLabel = "Other villain", InputPlaceholder = "Name another villain...",
                Choices = [new("joker", "The Joker")] }
    ];

    private void ShowResults(IReadOnlyDictionary<string, QuestionnaireAnswer> answers) { }
}
```

- Single choice (radio), multiple choice (`Multiple`), and optional freeform
  text (set `InputLabel`; `InputPlaceholder` is optional). Typing replaces a single choice.
- Optional questions (`Required = false`) show a Skip button; required ones
  show an inline validation error.
- Progress bar, Previous/Next, and keyboard shortcuts: letters `A`–`Z` select
  choices and `Enter` advances. Focus moves to each new question.
- Pure Blazor and scoped CSS; no JavaScript.

## Carousel

The **Carousel** menu item opens `/carousel`, which hosts the reusable
`Components\CssCarousel.razor` component. It uses no JavaScript and no C# state:

- Each slide is a hidden radio input followed by a `<figure>`. The
  `:checked + .carousel-slide` selector shows the active slide with a crossfade.
- Arrow and dot `<label>` elements select radios. The radio group also supports
  keyboard arrow keys. Each component instance uses a unique group name.
- Animations are disabled when `prefers-reduced-motion` is set.

Usage:

```razor
<CssCarousel Slides="mySlides" Label="Photo gallery" />
```

Dog photos in `wwwroot\images\dogs` come from Wikimedia Commons featured
pictures. They are resized to 1600px wide. Credits and license links appear on
the Carousel page:

| File | Photographer | License |
|---|---|---|
| dalmatian-fetch.jpg | Lucasbosch | CC BY-SA 4.0 |
| bulldogs-swimming.jpg | W.carter | CC0 |
| golden-retriever.jpg | Dietmar Rabich | CC BY-SA 4.0 |
| puppies-playing.jpg | Basile Morin | CC BY-SA 4.0 |
| golden-hour-run.jpg | Basile Morin | CC BY-SA 4.0 |
| dog-in-water.jpg | Basile Morin | CC BY-SA 4.0 |

## Credit card form

The **Credit Card** menu item opens `/credit-card`, an interactive payment card
form inspired by `react-credit-cards`, built with C# and CSS only:

- `Components\CreditCardForm.razor` holds the inputs, formatting, and validation.
  `OnValidSubmit` returns a masked `CardSummary` (brand, last four digits, name,
  expiry), never the full number or CVC.
- `Components\CreditCardPreview.razor` renders the live card. It flips in 3D for
  the CVC; American Express shows its code on the front.
- `Components\CardBrand.cs` detects Visa, Mastercard, American Express, Discover,
  Diners Club, JCB, and UnionPay, groups digits per brand, and runs Luhn,
  length, expiry, and CVC checks.
- Brand marks are styled text, not official logos. Test-card buttons fill
  well-known public test numbers.

The form is a demo: nothing is sent or stored. For real payments, use a payment
provider's hosted fields to stay PCI compliant.

## Hover card

The **Hover Card** menu item opens `/hover-card`, a demo of a reusable
`Components\HoverCard.razor` inspired by the shadcn/ui hover card. It is built with
C# and CSS only, with no JavaScript:

```razor
<HoverCard Side="HoverCardSide.Top" Align="HoverCardAlign.Start">
    <Trigger><a href="https://nextjs.org">@nextjs</a></Trigger>
    <ChildContent>Profile preview content…</ChildContent>
</HoverCard>
```

- `Side` (`Top`, `Right`, `Bottom`, `Left`) and `Align` (`Start`, `Center`, `End`)
  position the card. `SideOffset` sets the gap and `Width` sets the card width.
- `OpenDelay` (700 ms) and `CloseDelay` (300 ms) match shadcn. Moving the pointer
  from the trigger onto the card keeps it open.
- Keyboard focus opens the card immediately and Escape closes it. `OpenChanged`
  reports the state.
- On small screens, left and right cards open below the trigger. Animations
  respect reduced-motion settings.
