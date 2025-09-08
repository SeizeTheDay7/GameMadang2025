using UnityEngine;

[CreateAssetMenu(fileName = "New Stat", menuName = "Stats/Stat")]
public class SO_Stat : ScriptableObject
{
    [field: SerializeField, Min(0)] public float Damage { get; private set; }
    [field: SerializeField, Min(0)] public float AttackCooldown { get; private set; }
    public WaitForSeconds AttackCooldownTime { get; private set; }
    [field: SerializeField, Min(0)] public float IdleTime { get; private set; }
    public WaitForSeconds IdleTimeWait { get; private set; }
    [field: SerializeField] public Projectile Projectile { get; private set; }
    [field: SerializeField, Range(0f, 1f)] public float ItemDropPercentage { get; private set; }
    [field: SerializeField] public Item Item { get; private set; }
    [field: SerializeField] public float exp { get; private set; }


    public void Init()
    {
        AttackCooldownTime = new WaitForSeconds(AttackCooldown * 3);
        IdleTimeWait = new WaitForSeconds(IdleTime);
    }
}
