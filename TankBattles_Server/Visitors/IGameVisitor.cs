public interface IGameVisitor
{
	void Visit(Tank tank);
	void Visit(Wall wall);
	void Visit(Projectile projectile);
}
