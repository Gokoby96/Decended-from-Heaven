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
    
    [Header("Blood Effects")]
    public ParticleSystem normalBloodEffect;
    public ParticleSystem headshotBloodEffect;

    private int currentAmmo;
    public bool isReloading = false;

    
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

        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
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
            if (!gameObject.activeInHierarchy) // Silah inaktifse Coroutine'i durdur
            {
                isReloading = false;
                reloadSlider.gameObject.SetActive(false);
                if (reloadSlider != null)
                    reloadSlider.gameObject.SetActive(false);
                yield break;
            }
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
            EnemyHealth enemy = hit.transform.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                float appliedDamage = damage;
                ParticleSystem bloodToPlay = normalBloodEffect;

                // Headshot kontrolü
                if (hit.collider.CompareTag("Head"))
                {
                    appliedDamage = enemy.maxHealth; 
                    bloodToPlay = headshotBloodEffect;
                }

                enemy.TakeDamage(appliedDamage);

                if (bloodToPlay != null)
                {
                    Vector3 spawnPos = hit.point + fpsCam.transform.forward * -1f;
                    ParticleSystem blood = Instantiate(bloodToPlay, spawnPos, Quaternion.LookRotation(hit.normal));

                    blood.Play();

                    
                    float destroyTime = 2f;

                    
                    if (hit.collider.CompareTag("Head"))
                        destroyTime = 1f;  

                    Destroy(blood.gameObject, destroyTime);
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
