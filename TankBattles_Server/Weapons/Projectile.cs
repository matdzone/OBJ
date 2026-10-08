using System.Text.Json.Serialization;

public class Projectile
{
	public const double Radius = 0.1;

	public double X { get; private set; }
	public double Y { get; private set; }

	public double VelocityX { get; private set; }
	public double VelocityY { get; private set; }
	public bool IsPiercing { get; private set; } = false;
	private double hitCooldown = 0;

	public bool CanHit => hitCooldown <= 0;
	public int Damage { get; private set;}
	public int OwnerId { get; }

	public string Kind { get; }

	public bool HasSplit { get; set; } = false;

	public DateTime CreatedAt { get; set;} = DateTime.UtcNow;

	public TimeSpan? MaxLifetime { get; set;}

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

	public Projectile Clone()
	{
		var copy = (Projectile)MemberwiseClone();
		copy.Movement = Movement.Clone();
		copy.HasSplit = true;
		copy.CreatedAt = DateTime.UtcNow;
		copy.MaxLifetime = TimeSpan.FromSeconds(2);
		copy.hitCooldown = 0;   
		return copy;
	}

	public List<Projectile> Split(int count, double spreadAngle, double speedMultiplier = 1.0)
	{
		var projectiles = new List<Projectile>();
		for (int i = 0; i < count; i++)
		{
			var copy = Clone();
			double angle = spreadAngle * (i - (count - 1) / 2.0);
			double rad = angle * Math.PI / 180.0;

			double cos = Math.Cos(rad);
			double sin = Math.Sin(rad);

			copy.SetVelocity(
				(VelocityX * cos - VelocityY * sin) * speedMultiplier,
				(VelocityX * sin + VelocityY * cos) * speedMultiplier
			);


			projectiles.Add(copy);
		}
		return projectiles;
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
		if (hitCooldown > 0)
	{
    hitCooldown -= deltaSeconds;
	}
	}
	public void SetDamage(int damage)
	{
    Damage = damage;
	}
	public void SetPiercing(bool piercing)
	{
    IsPiercing = piercing;
	}
	public void StartHitCooldown()
	{
    hitCooldown = 0.35;
	}
}
