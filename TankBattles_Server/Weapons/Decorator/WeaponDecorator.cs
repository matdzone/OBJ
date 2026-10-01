public abstract class WeaponDecorator : Weapon
{
    protected Weapon weapon;

    public WeaponDecorator(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public override string Name => weapon.Name;

    public override int Damage => weapon.Damage;

    public override bool IsSingleUse => weapon.IsSingleUse;
}