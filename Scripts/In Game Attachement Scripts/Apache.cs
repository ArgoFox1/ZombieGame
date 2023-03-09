using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Apache : MonoBehaviour
{
    #region Lists
    public List<Transform> enemies = new List<Transform>();
    public List<ParticleSystem> rocketFXS = new List<ParticleSystem>();
    public List<GameObject> bullets = new List<GameObject>();
    public List<GameObject> rockets = new List<GameObject>();
    public List<ParticleSystem> holeFXS = new List<ParticleSystem>();
    #endregion

    #region Pools
    private List<GameObject> bullets2 = new List<GameObject>();
    private List<ParticleSystem> holeFXS2 = new List<ParticleSystem>();
    #endregion

    #region Static Variables
    private AudioSource folder;
    public AudioClip boomSound;
    public AudioClip rocketClip;
    public AudioClip shootClip;

    private Transform closestEnemy = null;
    public Transform spawnPos;

    private float enemyHealth, enemyHealth2;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float workTime;

    private ParticleSystem currentHoleFX;

    public LayerMask zombieMask;

    public bool canActive;
    public bool canFire;
    public bool canRocket;

    private GameObject carrier;
    public GameObject turretBarrel;
    public GameObject turret;
    private GameObject currentBullet;
    public GameObject bulletSpawnPos;
    #endregion

    private void Start()
    {
        folder = GetComponent<AudioSource>();
        InvokeRepeating(nameof(SoundSettings), 0, 1);
        InvokeRepeating(nameof(DecreaseTime), 0.1f, 1);
        folder.Play();
        carrier = GameObject.Find("Carrier");
        folder.volume = 0.1f;
        canFire = true;
        canRocket = true;
        canActive = true;
    }
    private void Update()
    {

        #region SoundSettings    
        if (Time.timeScale == 0)
        {
            folder.Pause();
        }
        else
        {
            folder.UnPause();
        }
        #endregion

        #region Apache
        if(canActive == true)
        {
            gameObject.SetActive(true);
            float distance = Vector3.Distance(gameObject.transform.position, carrier.transform.position);
            if(distance <= 500 && workTime >= 30)
            {
                gameObject.transform.position = gameObject.transform.position;
            }
            else
            {
                gameObject.transform.position += transform.forward * Time.deltaTime * speed;
            }
        }
        else
        {
            Disable();
        }
        #endregion

    }
    private void OnTriggerStay(Collider other)
    {        
        if (other.gameObject.CompareTag("Head") || other.gameObject.CompareTag("Body"))
        {
            if(other.gameObject.activeInHierarchy == true)
            {
                FindClosestEnemy();
                RaycastHit cHit;
                Vector3 tPos = closestEnemy.transform.position - turret.transform.position;
                turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, Quaternion.LookRotation(tPos), Time.deltaTime * rotateSpeed);
                turretBarrel.transform.localRotation = Quaternion.Lerp(turretBarrel.transform.localRotation, Quaternion.Euler(0, 0, turretBarrel.transform.localRotation.z + Time.time * 100f), Time.deltaTime * speed * 4);
                if (Physics.Raycast(bulletSpawnPos.transform.position, bulletSpawnPos.transform.forward, out cHit, Mathf.Infinity, zombieMask))
                {
                    if (canFire == true)
                    {
                        Fire();
                        BulletHoleFX(cHit.point);
                        GiveDamage();
                    }
                    if (enemies.Count > 1 && rockets.Count != 0 && workTime >= 60)
                    {
                        Vector3 gPos = closestEnemy.transform.position - gameObject.transform.position;
                        gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(gPos), Time.deltaTime * rotateSpeed);
                        if (gameObject.transform.rotation == Quaternion.LookRotation(gPos))
                        {
                            if (canRocket == true)
                            {
                                Rocket();
                            }
                        }                       
                    }
                    else
                    {
                        gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * 4f);
                    }
                }
            }          
        }
        else
        {
            enemies.Remove(other.gameObject.transform);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Head") || other.gameObject.CompareTag("Body"))
        {
            enemies.Add(other.gameObject.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Head") || other.gameObject.CompareTag("Body"))
        {
            enemies.Remove(other.gameObject.transform);
        }
    }
    private void DecreaseTime()
    {
        workTime--;
        if (workTime <= 0)
        {
            workTime = 0;
            canActive = false;
        }
    }
    private void Disable()
    {
        gameObject.SetActive(false);
        gameObject.transform.position = spawnPos.position;
        gameObject.transform.rotation = spawnPos.rotation;
        enemies.Clear();
    }
    public void CanWork(float valueF, bool valueB)
    {
        workTime += valueF;
        canActive = valueB;
    }
    private void GiveDamage()
    {
        RaycastHit zHit;
        if (gameObject.activeInHierarchy == true)
        {
            if (Physics.Raycast(bulletSpawnPos.transform.position, bulletSpawnPos.transform.forward, out zHit, Mathf.Infinity, zombieMask))
            {
                // Default Zombie Gets Damage
                if (zHit.collider.gameObject.GetComponentInParent<Enemy>() != null)
                {
                    if (zHit.collider.gameObject.CompareTag("Body"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(5);
                    }
                    if (zHit.collider.gameObject.CompareTag("Head"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(10);
                        zHit.collider.gameObject.GetComponentInParent<Animation>().Play("HeadShot");
                    }
                    if (zHit.collider.gameObject.CompareTag("Legs"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(1);
                    }
                    if (zHit.collider.gameObject.CompareTag("Arms"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(1);
                    }
                }
                // Boomer Gets Damage
                if (zHit.collider.gameObject.GetComponentInParent<FatZmbScr>() != null)
                {
                    if (zHit.collider.gameObject.CompareTag("Body"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(5);
                        enemyHealth = zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetHealth(enemyHealth);
                        if (enemyHealth <= 0)
                        {
                            folder.PlayOneShot(boomSound);
                        }
                    }
                    if (zHit.collider.gameObject.CompareTag("Head"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(10);
                        enemyHealth = zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetHealth(enemyHealth);
                        if (enemyHealth <= 0)
                        {
                            folder.PlayOneShot(boomSound);
                        }
                    }
                    if (zHit.collider.gameObject.CompareTag("Legs"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(1);
                        enemyHealth = zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetHealth(enemyHealth);
                        if (enemyHealth <= 0)
                        {
                            folder.PlayOneShot(boomSound);
                        }
                    }
                    if (zHit.collider.gameObject.CompareTag("Arms"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(1);
                        enemyHealth = zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetHealth(enemyHealth);
                        if (enemyHealth <= 0)
                        {
                            folder.PlayOneShot(boomSound);
                        }
                    }
                }
                // Flamer Zombie Gets Damage
                if (zHit.collider.gameObject.GetComponentInParent<Flamer>() != null)
                {
                    if (zHit.collider.gameObject.CompareTag("Body"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(5);
                        enemyHealth2 = zHit.collider.gameObject.GetComponentInParent<Flamer>().GetHealth(enemyHealth2);
                        if (enemyHealth2 <= 0)
                        {
                            folder.PlayOneShot(boomSound);
                        }
                    }
                    if (zHit.collider.gameObject.CompareTag("Head"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(10);
                        enemyHealth2 = zHit.collider.gameObject.GetComponentInParent<Flamer>().GetHealth(enemyHealth2);
                        if (enemyHealth2 <= 0)
                        {
                            folder.PlayOneShot(boomSound);
                        }
                    }
                    if (zHit.collider.gameObject.CompareTag("Legs"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(1);
                        enemyHealth2 = zHit.collider.gameObject.GetComponentInParent<Flamer>().GetHealth(enemyHealth2);
                        if (enemyHealth2 <= 0)
                        {
                            folder.PlayOneShot(boomSound);
                        }
                    }
                    if (zHit.collider.gameObject.CompareTag("Arms"))
                    {
                        zHit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(1);
                        enemyHealth2 = zHit.collider.gameObject.GetComponentInParent<Flamer>().GetHealth(enemyHealth2);
                        if (enemyHealth2 <= 0)
                        {
                            folder.PlayOneShot(boomSound);
                        }
                    }
                }
            }
        }
    }
    private Transform FindClosestEnemy()
    {
        float minDist = Mathf.Infinity;
        foreach (Transform e in enemies)
        {
            if(e.gameObject.activeInHierarchy == true)
            {
                float dist = Vector3.Distance(e.transform.position, gameObject.transform.position);
                if (dist < minDist)
                {
                    closestEnemy = e;
                    minDist = dist;
                }
            }          
        }
        return closestEnemy;
    }
    private void SoundSettings()
    {
        folder.volume += Time.deltaTime;
    }
    private void Fire()
    {
        folder.PlayOneShot(shootClip);
        canFire = false;
        bullets[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
        bullets[0].transform.position = bulletSpawnPos.transform.position;
        bullets[0].transform.rotation = bulletSpawnPos.transform.rotation;
        bullets[0].SetActive(true);
        currentBullet = bullets[0].gameObject;
        currentBullet.GetComponent<Rigidbody>().velocity += bulletSpawnPos.transform.forward * Time.deltaTime * 100000f;
        bullets2.Add(currentBullet);
        bullets.RemoveAt(0);
        Invoke(nameof(AmmoPool), 1f);
        StartCoroutine(nameof(CoolDown4Fire));
    }
    private void AmmoPool()
    {
        bullets2[0].SetActive(false);
        bullets.Add(bullets2[0].gameObject);
        bullets2.RemoveAt(0);
    }
    IEnumerator CoolDown4Fire()
    {
        folder.Pause();
        yield return new WaitForSeconds(0.1f);
        canFire = true;
    }
    private void BulletHoleFX(Vector3 impactPos)
    {
        Vector3 fxPos = closestEnemy.transform.position - turret.transform.position;
        if(turret.transform.rotation == Quaternion.LookRotation(fxPos))
        {
            holeFXS[0].Play();
            holeFXS[0].gameObject.SetActive(true);
            currentHoleFX = holeFXS[0];
            currentHoleFX.gameObject.transform.position = impactPos;
            holeFXS2.Add(currentHoleFX);
            holeFXS.RemoveAt(0);
            Invoke(nameof(BulletHoleFXPool), 1);
        }       
    }
    private void BulletHoleFXPool()
    {
        holeFXS2[0].Stop();
        holeFXS2[0].gameObject.SetActive(false);
        holeFXS.Add(holeFXS2[0]);
        holeFXS2.RemoveAt(0);
    }
    private void Rocket()
    {
        for (int i = 0; i < 2; i++)
        {
            canRocket = false;
            folder.PlayOneShot(rocketClip);
            rocketFXS[0].gameObject.SetActive(true);
            rocketFXS[0].Play();
            rockets[0].transform.parent = null;
            rockets[0].GetComponent<Rigidbody>().velocity += rockets[0].transform.forward * Time.deltaTime * 10000f;
            rocketFXS.RemoveAt(0);
            rockets.RemoveAt(0);
            StartCoroutine(nameof(CoolDown4Rocket));
        }     
    }
    IEnumerator CoolDown4Rocket()
    {
        yield return new WaitForSeconds(4f);
        canRocket = true;
    }
}
