public abstract class TankState
{
	public virtual double SpeedMultiplier => 1.0;
	public virtual bool CanShoot => true;

	public virtual void TakeDamage(Tank tank, int damage)
	{
		tank.ApplyDamage(damage);
		if (!tank.IsAlive) tank.TransitionTo(new DestroyedState());
		else if (IsCritical(tank)) tank.TransitionTo(new CriticalState());
	}

	public virtual void Heal(Tank tank, int amount) => tank.ApplyHeal(amount);

	public virtual void Update(Tank tank, double deltaSeconds) { }

	protected static bool IsCritical(Tank tank) => tank.Health <= tank.MaxHealth * 0.3;
}
