using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage = 1;

    private void Start()
    {
        damage = FindObjectOfType<Shooting>().GetDamage();
    }

    /*
    private void Update()
    {
        Ray hitDirection = new(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(hitDirection, out RaycastHit hit) && hit.collider.CompareTag("Enemy"))
        {
            hit.collider.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //DamageEnemy(other);
        }
    }

    private void DamageEnemy(Collider other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
