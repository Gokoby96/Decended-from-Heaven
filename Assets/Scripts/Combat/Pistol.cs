using System;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public float damage ;
    public float range ;
    public float fireRate ;
    private Camera fpsCam;

    private float nextTimeToFire ;

    private void Start()
    {
        if (fpsCam == null)
            fpsCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
Debug.Log("attack");
            Debug.DrawRay(fpsCam.transform.position, fpsCam.transform.forward * range, Color.red, 1f);
        }
    }
}
