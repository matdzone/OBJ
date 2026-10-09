public class DisguiseBox : Box
{
    private static readonly ITankFactory[] Factories =
    {
        new LightTankFactory(),
        new MediumTankFactory(),
        new HeavyTankFactory()
    };

    public override string Kind => "Disguise";

    public DisguiseBox(int x, int y)
        : base(x, y)
    {
    }

    protected override bool CanApply(Tank tank)
    {
        return !tank.IsMasked;
    }

    protected override void Apply(Tank tank)
    {
        var options = Factories
            .Select(factory => factory.CreateAppearance())
            .Where(appearance => appearance.Type != tank.Appearance?.Type)
            .ToList();

        tank.Mask(options[Random.Shared.Next(options.Count)]);
    }
}