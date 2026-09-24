using System.Text.Json.Serialization;

public class Projectile
{
	public const double Radius = 0.1;

	public double X { get; private set; }
	public double Y { get; private set; }

	public double VelocityX { get; private set; }
	public double VelocityY { get; private set; }

	public int Damage { get; }
	public int OwnerId { get; }

	public string Kind { get; }

	public DateTime CreatedAt { get; } = DateTime.UtcNow;

	public TimeSpan? MaxLifetime { get; }

	[JsonIgnore]
	public IMovementStrategy Movement { get; set; }

	[JsonIgnore]
	public bool IsExpired =>
		MaxLifetime != null &&
		DateTime.UtcNow - CreatedAt > MaxLifetime;

	[JsonIgnore]
	public int TileX => (int)Math.Floor(X);

	[JsonIgnore]
	public int TileY => (int)Math.Floor(Y);

	[JsonIgnore]
	public double Speed => Math.Sqrt(VelocityX * VelocityX + VelocityY * VelocityY);

	public Projectile(
		double x,
		double y,
		double velocityX,
		double velocityY,
		int damage,
		int ownerId,
		string kind = "Shell",
		IMovementStrategy? movement = null,
		TimeSpan? maxLifetime = null)
	{
		X = x;
		Y = y;
		VelocityX = velocityX;
		VelocityY = velocityY;
		Damage = damage;
		OwnerId = ownerId;
		Kind = kind;
		Movement = movement ?? new StraightMovement();
		MaxLifetime = maxLifetime;
	}

	public void SetVelocity(double velocityX, double velocityY)
	{
		VelocityX = velocityX;
		VelocityY = velocityY;
	}

	public void MoveBy(double deltaX, double deltaY)
	{
		X += deltaX;
		Y += deltaY;
	}

	public void Move(double deltaSeconds)
	{
		Movement.Move(this, deltaSeconds);
	}
}
