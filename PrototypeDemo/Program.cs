using System;
using System.Runtime.CompilerServices;

static string Addr(object o) => "0x" + Unsafe.As<object, IntPtr>(ref o).ToString("X");

var original = new Projectile(5, 5, 1, 0, new AcceleratingMovement());
var shallow  = original.ShallowCopy();
var deep     = original.Clone();

Console.WriteLine("=== Memory addresses ===");
Console.WriteLine($"original  Projectile: {Addr(original)}  Movement: {Addr(original.Movement)}");
Console.WriteLine($"shallow   Projectile: {Addr(shallow)}  Movement: {Addr(shallow.Movement)}");
Console.WriteLine($"deep      Projectile: {Addr(deep)}  Movement: {Addr(deep.Movement)}");

Console.WriteLine();
Console.WriteLine($"original.Movement same object as shallow.Movement? {ReferenceEquals(original.Movement, shallow.Movement)}");
Console.WriteLine($"original.Movement same object as deep.Movement?    {ReferenceEquals(original.Movement, deep.Movement)}");

// Advance only the original for 50 frames
for (int i = 0; i < 50; i++)
    original.Movement.Move(original, 0.016);

Console.WriteLine();
Console.WriteLine("=== Speed multiplier after moving ONLY the original ===");
Console.WriteLine($"original: {((AcceleratingMovement)original.Movement).Multiplier:F2}");
Console.WriteLine($"shallow:  {((AcceleratingMovement)shallow.Movement).Multiplier:F2}");
Console.WriteLine($"deep:     {((AcceleratingMovement)deep.Movement).Multiplier:F2}");

public interface IMovementStrategy
{
    void Move(Projectile p, double dt);
    IMovementStrategy Clone();
}

public class AcceleratingMovement : IMovementStrategy
{
    public double Multiplier { get; private set; } = 1.0;

    public void Move(Projectile p, double dt)
    {
        Multiplier += dt;
        p.X += p.VelocityX * Multiplier * dt;
        p.Y += p.VelocityY * Multiplier * dt;
    }

    public IMovementStrategy Clone() =>
        new AcceleratingMovement { Multiplier = Multiplier };
}

public class Projectile
{
    public double X, Y, VelocityX, VelocityY;
    public IMovementStrategy Movement { get; private set; }

    public Projectile(double x, double y, double vx, double vy, IMovementStrategy movement)
    {
        X = x; Y = y; VelocityX = vx; VelocityY = vy; Movement = movement;
    }

    public Projectile ShallowCopy() => (Projectile)MemberwiseClone();

    public Projectile Clone()
    {
        var copy = (Projectile)MemberwiseClone();
        copy.Movement = Movement.Clone();
        return copy;
    }
}