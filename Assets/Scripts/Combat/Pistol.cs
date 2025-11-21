using TMPro;
using System;
using System.Collections;
using UnityEngine;


public class Pistol : MonoBehaviour
{
    public float damage;
    public float range;
    public float fireRate;

    [Header("Ammo Settings")] 
    public int magazineSize = 12; 
    public int totalAmmo = 48; 
    public float reloadTime = 1.3f;
    public int maxAmmo = 48;
    [Header("Reload UI")]
    public  UnityEngine.UI.Slider reloadSlider;

    private int currentAmmo;
    private bool isReloading = false;

    public ParticleSystem bloodEffect;
    public GunRecoil recoil;
    private Camera fpsCam;
    public TextMeshProUGUI ammoText;

    private float nextTimeToFire;

    private void Start()
    {
        if (fpsCam == null)
            fpsCam = Camera.main;

        currentAmmo = magazineSize;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading)
            return;

        // R tuşu ile reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentAmmo < magazineSize && totalAmmo > 0)
                StartCoroutine(Reload());
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("Şarjör boş!");
                return;
            }

            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        reloadSlider.gameObject.SetActive(true);
        reloadSlider.value = 0f;

        float t = 0f;

        while (t < reloadTime)
        {
            t += Time.deltaTime;
            reloadSlider.value = t / reloadTime;   // Slider barı doldur
            yield return null;
        }

        
        reloadSlider.value = 0f;
        reloadSlider.gameObject.SetActive(false);

        int neededAmmo = magazineSize - currentAmmo;

        if (totalAmmo >= neededAmmo)
        {
            totalAmmo -= neededAmmo;
            currentAmmo = magazineSize;
        }
        else
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }

        UpdateAmmoUI();
        isReloading = false;
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        if (recoil != null)
            recoil.Fire();

        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
               
                enemy.TakeDamage(damage);

                
                if (bloodEffect != null)
                {
                    Vector3 spawnPos = hit.point + fpsCam.transform.forward * -1f;
                    ParticleSystem blood = Instantiate(bloodEffect, spawnPos, Quaternion.LookRotation(hit.normal));
                    blood.Play();
                    Destroy(blood.gameObject, 2f);
                }
            }
        }

        Debug.Log("attack");
        Debug.DrawRay(fpsCam.transform.position, fpsCam.transform.forward * range, Color.red, 1f);
    }
    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo + " / " + totalAmmo;
    }
    public bool AddAmmo(int amount)
    {
        if (totalAmmo >= maxAmmo)
            return false;
        totalAmmo += amount;
        if (totalAmmo > maxAmmo)
            totalAmmo = maxAmmo;

        UpdateAmmoUI();
        return true; 
    }
}
