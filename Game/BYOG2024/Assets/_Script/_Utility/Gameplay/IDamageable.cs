namespace AstekUtility.Gameplay
{
	public interface IDamageable
	{
		float MaxHp { get; }
		float CurrentHp { get; }

		void Damage(float amount);
		void Heal(float amount);
		void OnDeath();
	}
}