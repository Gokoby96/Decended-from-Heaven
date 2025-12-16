using UnityEngine;

public class Katana : MonoBehaviour
{
  
        public float damage;
        public float attackRange;
        public float attackRate;
        private Camera fpsCam; 
        private KatanaSlash slash;

        private float nextAttackTime = 0f;
        
        private void Start()
        {
            slash = GetComponent<KatanaSlash>();
            if (fpsCam == null)
                fpsCam = Camera.main;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackRate;
                slash.PlaySlash();
                Attack();
            }
        }

        void Attack()
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, attackRange))
            {
                EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
                if (enemy != null)
                    enemy.TakeDamage(damage);
                

                Debug.DrawRay(fpsCam.transform.position, fpsCam.transform.forward * attackRange, Color.green, 0.5f);
            }
            
        }
}
