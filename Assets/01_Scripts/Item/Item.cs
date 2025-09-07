using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] float healAmount = 10f;
    [SerializeField] ParticleSystem particle;

    float startZ;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Attributes attributes))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (particle)
                {
                    particle.Play();
                    particle.transform.SetParent(null);
                }
                attributes.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }

    private void Awake()
    {
        startZ = transform.position.z;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, startZ + Mathf.Sin(Time.time) * 0.1f + 1f);
    }
}
