
public static class FlyweightMemoryTest
{
    private const int WallCount = 100;
    private const int TextureSize = 1024 * 1024; // 1 MiB

    // Eksperimentinė siena BE Flyweight.
    // Kiekviena siena turi atskirą tekstūros masyvą.
    private sealed class WallWithoutFlyweight
    {
        public int X { get; }
        public int Y { get; }
        public byte[] TextureData { get; }

        public WallWithoutFlyweight(int x, int y)
        {
            X = x;
            Y = y;
            TextureData = new byte[TextureSize];
        }
    }

    public static void Run()
    {
        Console.WriteLine("=== Flyweight atminties palyginimas ===");
        Console.WriteLine($"Sienu skaicius: {WallCount}");
        Console.WriteLine($"Teksturos dydis: {TextureSize / 1024 / 1024} MiB");

        // Pirmiausia matuojame sienas BE Flyweight.
        long withoutFlyweight = MeasureWithoutFlyweight();

        // Tada matuojame sienas SU Flyweight.
        long withFlyweight = MeasureWithFlyweight();

        double savedMiB =
            (withoutFlyweight - withFlyweight) / 1024.0 / 1024.0;

        double savedPercent = withoutFlyweight > 0
            ? (1.0 - (double)withFlyweight / withoutFlyweight) * 100
            : 0;

        Console.WriteLine();
        Console.WriteLine(
            $"Be Flyweight: {ToMiB(withoutFlyweight):F2} MiB"
        );
        Console.WriteLine(
            $"Su Flyweight: {ToMiB(withFlyweight):F2} MiB"
        );
        Console.WriteLine(
            $"Sutaupyta: {savedMiB:F2} MiB ({savedPercent:F2} %)"
        );
    }

    private static long MeasureWithoutFlyweight()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long before = GC.GetTotalMemory(true);

        var walls = new List<WallWithoutFlyweight>(WallCount);

        for (int i = 0; i < WallCount; i++)
        {
            walls.Add(new WallWithoutFlyweight(i, 0));
        }

        long after = GC.GetTotalMemory(true);

        // Užtikriname, kad sienos išliktų gyvos matavimo metu.
        GC.KeepAlive(walls);

        return after - before;
    }

    private static long MeasureWithFlyweight()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long before = GC.GetTotalMemory(true);

        var factory = new WallTypeFactory();
        var walls = new List<Wall>(WallCount);

        for (int i = 0; i < WallCount; i++)
        {
            WallType type = factory.GetWallType("Stone", false);
            walls.Add(new Wall(i, 0, type));
        }

        long after = GC.GetTotalMemory(true);

        // Patikriname, ar sienos tikrai dalijasi tipu.
        bool shared = ReferenceEquals(
            walls[0].Type,
            walls[1].Type
        );

        Console.WriteLine($"Bendras WallType objektas: {shared}");

        GC.KeepAlive(walls);
        GC.KeepAlive(factory);

        return after - before;
    }

    private static double ToMiB(long bytes)
    {
        return bytes / 1024.0 / 1024.0;
    }
}
