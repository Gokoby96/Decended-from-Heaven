using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon List")]
    public List<GameObject> weapons = new List<GameObject>(); // Tüm silahların bulunduğu yer
    private int currentWeaponIndex = 0;

    private GameObject currentWeapon;

    void Start()
    {
        if (weapons.Count == 0) return;
        EquipWeapon(0); 
    }

    void Update()
    {
        
        for (int i = 0; i < weapons.Count && i < 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                Debug.Log("Switching to weapon index: " + i);
                EquipWeapon(i);
            }
        }

     
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            NextWeapon();
        }
        else if (scroll < 0f)
        {
            PreviousWeapon();
        }
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;
        
        // Mevcut silahı kapat
        if (currentWeapon != null)
        {
            Debug.Log("Deactivating: " + currentWeapon.name);
            currentWeapon.SetActive(false);
        }
           

        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];
        Debug.Log("Activating: " + currentWeapon.name);
        currentWeapon.SetActive(true);
    }

    void NextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipWeapon(nextIndex);
    }

    void PreviousWeapon()
    {
        int prevIndex = (currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
        EquipWeapon(prevIndex);
    }

    
    public T GetCurrentWeapon<T>() where T : Component
    {
        if (currentWeapon != null)
            return currentWeapon.GetComponent<T>();
        return null;
    }
}
