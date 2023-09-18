namespace Game.Behaviours;

[GlobalClass]
public sealed partial class Damageable: Node
{
    /// <summary>
    /// Triggered when the damageable's health has been updated.
    /// <para>
    /// Parameters:
    /// 0 (float): new health fac
    /// </para>
    /// </summary>
    public Action<float> OnHealthChanged;

    /// <summary>
    /// Triggered whenever the damageable takes damage.
    /// </summary>
    public Action OnDamageTaken;

    /// <summary>
    /// Triggered when the damageable runs out of health.
    /// </summary>
    public Action OnDeath;

    [Export]
    public float MaxHealth = 10;

    [Export]
    public float Armour = 0;

    private float _currentHealth;

    #region Main Functions

    public void TakeDamage(float amount, bool ignoreArmour = false)
    {
        float damageTaken = amount;

        if (!ignoreArmour) {
            damageTaken *= (damageTaken / (damageTaken + Armour));
        }

        _currentHealth -= damageTaken;
        OnHealthChanged?.Invoke(GetHealthFac());

        if (_currentHealth > 0.0f)
            return;

        OnDeath?.Invoke();
    }

    #endregion

    #region Setters/Getters

    public void SetMaxHealth(float maxHealth)
    {
        float healthFac = _currentHealth / MaxHealth;
        MaxHealth = maxHealth;

        _currentHealth = maxHealth * healthFac;
        OnHealthChanged?.Invoke(healthFac);
    }

    public float GetHealth()
    {
        return _currentHealth;
    }

    public float GetHealthFac()
    {
        return _currentHealth / MaxHealth;
    }

    #endregion
}