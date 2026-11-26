// public class Squad : ITankUnit
// {
//     private readonly List<ITankUnit> _members = new();

//     public TankAppearance? Appearance { get; private set; }

//     public int Id { get; }
//     public string Name { get; }
//     public IReadOnlyList<ITankUnit> Members => _members;

//     public Squad(int id, string name) { Id = id; Name = name; }
 
//     public void Add(ITankUnit unit) => _members.Add(unit);
//     public void Remove(ITankUnit unit) => _members.Remove(unit);

//     public bool IsAlive => _members.Any(m => m.IsAlive);

//     private IEnumerable<ITankUnit> Alive => _members.Where(m => m.IsAlive);

//     public void Heal(int amount)                 { foreach (var m in Alive) m.Heal(amount); }
//     public void AddShield(int amount)            { foreach (var m in Alive) m.AddShield(amount); }
//     public void UpgradeWeapon()                  { foreach (var m in Alive) m.UpgradeWeapon(); }
//     public void EquipWeapon(Weapon weapon)       { foreach (var m in Alive) m.EquipWeapon(weapon); }
//     public void Mask(TankAppearance disguise)    { foreach (var m in Alive) m.Mask(disguise); }
// }