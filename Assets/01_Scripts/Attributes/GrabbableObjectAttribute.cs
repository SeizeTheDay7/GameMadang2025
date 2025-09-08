using UnityEngine;

public class GrabbableObjectAttribute : Attributes
{
    [Header(" - GrabbableObject Attribute - ")]
    [SerializeField] float respawnTime = 5f;

    public void FullHealth()
    {
        currentHealth = maxHealth;
    }

    // public override void TakeDamage(float damage)
    // {
    //     currentHealth = Mathf.Max(currentHealth - 1, 0);
    //     Debug.Log("Current Health: " + currentHealth);

    //     if (currentHealth == 0)
    //     {
    //         Die();
    //     }
    // }

    // protected override void Die()
    // {
    //     Debug.Log(gameObject.name + " disabled. respawn after " + respawnTime + " seconds.");
    //     gameObject.SetActive(false);
    // }
}