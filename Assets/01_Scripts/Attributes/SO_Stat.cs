using UnityEngine;

[CreateAssetMenu(fileName = "New Stat", menuName = "Stats/Stat")]
public class SO_Stat : ScriptableObject
{
    [field: SerializeField, Min(0)] public float Damage { get; private set; }
    [field: SerializeField, Min(0)] public float AttackCooldown { get; private set; }
    public WaitForSeconds AttackCooldownTime { get; private set; }
    [field: SerializeField] public Projectile Projectile { get; private set; }


    public void Init()
    {
        AttackCooldownTime = new WaitForSeconds(AttackCooldown);
    }
}
