using TowerDefence.Enemies;
using UnityEngine;

public class ballisticBullet : MonoBehaviour
{
    [SerializeField] private float startSpeed;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float explosionRadius;
    [SerializeField] private int explosionDamage;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem particles;

    private GameObject target;

    private float targetX = Mathf.Infinity;
    private float targetY = Mathf.Infinity;

    private float startingX = Mathf.Infinity;
    private float startingY = Mathf.Infinity;

    float fireLerp = 1;
    bool exploded;

    private void Start()
    {
        startingX = transform.position.x;
        startingY = transform.position.y;
        targetX = target.transform.position.x;
        targetY = target.transform.position.y;
        fireLerp = 0;
    }

    private void Update()
    {
        if (target)
        {
            targetX = target.transform.position.x;
            targetY = target.transform.position.y;
        }


        if (fireLerp < 1)
        {
            Vector3 newProjectilePos = CalculateTrajectory(new Vector3(startingX, startingY, 0), new Vector3(targetX, targetY, 0), fireLerp);
            transform.position = newProjectilePos;

            fireLerp += projectileSpeed * Time.deltaTime;
        }

        if (fireLerp >= 1 && !exploded)
        {
            exploded = true;
            Explode();
        }
    }

    public void InitTarget(GameObject targetGameObject)
    {
        target = targetGameObject;
    }

    Vector3 CalculateTrajectory(Vector3 firePos, Vector3 targetPos, float t)
    {
        Vector3 linearProgress = Vector3.Lerp(firePos, targetPos, t);
        float perspectiveOffset = Mathf.Sin(t * Mathf.PI) * startSpeed;

        Vector3 trajectoryPos = linearProgress + (Vector3.up * perspectiveOffset);
        return trajectoryPos;
    }

    private void Explode()
    {
        particles.Play();
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);

        if (!target) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in colliders)
        {
            var enemyHealth = collider.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth is not null)
            {
                enemyHealth.Damage(explosionDamage);
            }
        }
    }
}
