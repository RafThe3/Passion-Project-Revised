using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [Min(0), SerializeField] private float chaseDistance = 1;

    [Header("Health")]
    [SerializeField] private bool isInvincible = false;
    [Min(0), SerializeField] private int startingHealth = 1;
    [Min(0), SerializeField] private int maxHealth = 100;

    [Header("Attacking")]
    [SerializeField] private AttackType attackType = AttackType.None;
    [Min(0), SerializeField] private int damage = 1;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [Min(0), SerializeField] private float projectileSpeed = 1;
    [Min(0), SerializeField] private float projectileLife = 1;
    [Min(0), SerializeField] private float shootInterval = 1;
    [Min(0), SerializeField] private float shootDistance = 1;
    [SerializeField] private AudioClip shootSFX;

    [Header("Melee")]
    [Min(0), SerializeField] private float meleeInterval = 1;

    //Internal Variables
    private NavMeshAgent agent;
    private float currentHealth = 0;
    private Slider healthBar;
    private Canvas enemyUI;
    private float meleeTimer = 0, shootTimer = 0;
    private AudioSource audioSource;
    //private Animator animator;
    private Rigidbody rb;
    private Rigidbody rigidbody;
    private GameObject player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponentInChildren<AudioSource>();
        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyUI = GetComponentInChildren<Canvas>();
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<Slider>();
        }
    }

    private void Start()
    {
        if (startingHealth > maxHealth)
        {
            startingHealth = maxHealth;
        }
        currentHealth = startingHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = startingHealth;
        enemyUI.enabled = false;
        meleeTimer = meleeInterval;
        shootTimer = shootInterval;
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        meleeTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;

        bool isMoving = Mathf.Abs(rb.linearVelocity.z) > Mathf.Epsilon;
        //animator.SetBool("isMoving", isMoving);

        MoveEnemy();
        if (attackType == AttackType.Shoot)
        {
            Shoot();
        }
    }

    private void MoveEnemy()
    {
        Vector3 playerPosition = player.transform.position - transform.position;
        bool isPlayerNear = playerPosition.magnitude < chaseDistance;

        if (isPlayerNear || currentHealth < maxHealth)
        {
            agent.destination = player.transform.position;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            return;
        }

        if (currentHealth == healthBar.maxValue)
        {
            enemyUI.enabled = true;
        }

        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        player.GetComponent<Player>().AddXP();
        Destroy(gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && meleeTimer >= meleeInterval && attackType == AttackType.Melee)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            meleeTimer = 0;
        }
    }

    private void Shoot()
    {
        Vector3 playerPosition = player.transform.position - transform.position;

        if (shootTimer >= shootInterval && playerPosition.magnitude < shootDistance)
        {
            GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            projectileClone.GetComponent<Rigidbody>().linearVelocity = 10 * projectileSpeed * playerPosition.normalized;
            shootTimer = 0;
            audioSource.PlayOneShot(shootSFX);
            Destroy(projectileClone, projectileLife);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.DrawWireSphere(transform.position, shootDistance);
    }

    public int GetDamage()
    {
        return damage;
    }

    private enum AttackType { None, Melee, Shoot }
}
