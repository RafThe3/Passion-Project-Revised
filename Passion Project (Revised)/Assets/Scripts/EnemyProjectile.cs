using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage = 1;

    private void Start()
    {
        damage = FindObjectOfType<Enemy>().GetDamage();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
        }
    }
}
