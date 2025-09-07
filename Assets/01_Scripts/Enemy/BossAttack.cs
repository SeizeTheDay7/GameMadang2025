using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header(" - Homing Projectile - ")]
    [SerializeField] HomingProjectile projectile;
    [SerializeField] float projectileDamage;
    [SerializeField] float projectileAttackInterval;
    float projectileAttackTimer;

    [Header(" - Laser Attack - ")]
    [SerializeField] Projectile laserPrefab;
    [SerializeField] float laserDamage;
    [SerializeField] float laserAttackInterval;
    float laserAttackTimer;
    [SerializeField] float laserDuration;
    [SerializeField] float laserRange;

    LineRenderer lineRenderer;
    Attributes player;

    private void Awake()
    {
        TryGetComponent(out lineRenderer);
        lineRenderer.enabled = false;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Attributes>();
        projectileAttackTimer = projectileAttackInterval;
    }

    private void Update()
    {
        projectileAttackTimer -= Time.deltaTime;
        if (projectileAttackTimer <= 0)
        {
            Shoot();
            projectileAttackTimer = projectileAttackInterval; // Reset the interval (you can adjust this value as needed)
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= laserRange)
        {
            laserAttackTimer -= Time.deltaTime;
            if (laserAttackTimer <= 0)
            {
                StartCoroutine(CoLaserAttack());
                laserAttackTimer = laserAttackInterval; // Reset the interval (you can adjust this value as needed)
            }
        }
    }

    IEnumerator CoLaserAttack()
    {
        lineRenderer.enabled = true;
        float timer = 0f;
        Vector3 direction = (player.Center - transform.position).normalized;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, player.Center);

        while (timer < laserDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Projectile laser = Instantiate(laserPrefab, transform.position, Quaternion.LookRotation(direction));
        laser.Init(GetComponent<Attributes>());
        lineRenderer.enabled = false;
    }

    public void Shoot()
    {
        HomingProjectile newProjectile = Instantiate(projectile, transform.position + transform.right * 2, Quaternion.identity);
        newProjectile.Init(player, projectileDamage);
    }


}
