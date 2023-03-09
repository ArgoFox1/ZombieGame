using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShoutGun : MonoBehaviour
{
    #region Enemy Variables
    public LayerMask zombieMask;
    private float enemyHealth, enemyHealth2;
    public AudioClip boomSound;
    #endregion

    #region FX Variables
    private ParticleSystem currentBlood;
    [SerializeField] private float range;
    private GameObject currentHoleFX;
    private GameObject currentTrail;
    public LayerMask floorMask;
    private bool canFX;
    #endregion

    #region Reload Variables
    private bool canReload;
    public AudioClip reloadSound;
    #endregion

    #region Fire Variables
    public AudioClip shootClip;
    public AudioClip reloadClip;
    public int currentMagAmmo;
    public int magAmmo;
    public int totalAmmo;
    private bool canFire;
    #endregion

    #region Capsule Variables
    private bool canCapsule;
    private GameObject currentCapsule;
    #endregion

    #region Static Variables
    public Animation anim;
    public ParticleSystem muzzleFlash;
    public AudioClip emptyGunSound;
    private bool canBuy;
    public GameManager gm;
    public Transform arm;
    private AudioSource folder;
    public Transform emptyBulletSpawnPos;
    #endregion

    #region Recoil Variables
    private float recTimer;
    private Vector3 startPos;
    private Vector3 startArmPos;
    public Vector3 recoilAmount;
    [SerializeField] private float recoilSpeed;
    #endregion

    #region Keys
    [Header("Keys")]
    public KeyCode shootBtn = KeyCode.Mouse0;
    public KeyCode scopeBtn = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;
    #endregion

    #region Lists
    [Header("Lists")]
    public List<ParticleSystem> bloodFXS = new List<ParticleSystem>();
    public List<GameObject> tFXS = new List<GameObject>();
    public List<Transform> bulletSpawnPoss = new List<Transform>();
    public List<GameObject> sCapsules = new List<GameObject>();
    public List<GameObject> holeFxs = new List<GameObject>();
    #endregion

    #region Pools
    [Header("Pools")]
    private List<ParticleSystem> bloodFXS2 = new List<ParticleSystem>();
    private List<GameObject> tFXS2 = new List<GameObject>();
    private List<GameObject> sCapsules2 = new List<GameObject>();
    private List<GameObject> holeFxs2 = new List<GameObject>();
    #endregion

    private void Awake()
    {
        startArmPos = arm.localPosition;
        startPos = gameObject.transform.localPosition;
    }
    private void Start()
    {
        muzzleFlash.gameObject.SetActive(false);
        muzzleFlash.Stop();
        folder = GetComponent<AudioSource>();
        canFX = true;
        canFire = true;
        canCapsule = true;
        canReload = true;
    }
    private void Update()
    {
        LocateTrailPos();      
        if (gm.buyMenuImage.gameObject.activeInHierarchy == true)
        {
            canBuy = true;
        }
        if (magAmmo == 0 && Input.GetKeyDown(shootBtn) && Time.timeScale != 0)
        {
            folder.PlayOneShot(emptyGunSound);
        }
        if (Time.timeScale == 0)
        {
            folder.Pause();
        }
        #region Shoot
        if (gameObject.activeInHierarchy == true && gm.buyMenuImage.gameObject.activeInHierarchy != true && Time.timeScale != 0 && canBuy != true)
        {
            if (Input.GetKeyDown(shootBtn) && canFire == true && canCapsule == true)
            {
                if (Input.GetKey(scopeBtn) != true)
                {
                    if (magAmmo != 0 && canReload == true && canFX == true)
                    {
                        GiveDamage();
                        Recoil();
                        Fire();
                        SpawnCapsule();
                        TrailFXSpawner();
                        SpawnBulletHole();
                        BloodFXSpawner();
                    }
                    else
                    {
                        NonRecoil();
                    }
                }
            }
            else
            {
                NonRecoil();
            }
        }
        #endregion

        #region Reload
        if (Input.GetKeyDown(reloadKey) && canReload == true && gameObject.activeInHierarchy == true)
        {
            if (gm.buyMenuImage.gameObject.activeInHierarchy != true && Time.timeScale != 0)
            {
                Reload();
            }
        }
        #endregion        

    }
    // FXS
    
    #region FXS
    private void BloodFXSpawner()
    {
        for (int i = 0; i < bulletSpawnPoss.Count; i++)
        {
            RaycastHit bHit;
            if (Physics.Raycast(bulletSpawnPoss[i].position, bulletSpawnPoss[i].transform.forward, out bHit, Mathf.Infinity, zombieMask))
            {
                BloodFXS();
                float x = bHit.point.x;
                float y = bHit.point.y;
                x += Mathf.Cos(Time.time * i * 45) * range;
                y += Mathf.Sin(Time.time * i * 45) * range;
                Vector3 pos = new Vector3(x, y, bHit.point.z);
                currentBlood.transform.position = pos;
            }
        }
    }
    private void BloodFXS()
    {
        bloodFXS[0].gameObject.SetActive(true);
        currentBlood = bloodFXS[0];
        bloodFXS2.Add(currentBlood);
        bloodFXS.RemoveAt(0);
        Invoke(nameof(BloodPool), 0.3f);
    }
    private void BloodPool()
    {
        bloodFXS2[0].gameObject.SetActive(false);
        bloodFXS.Add(bloodFXS2[0]);
        bloodFXS2.RemoveAt(0);
    }
    private void LocateTrailPos()
    {
        for (int i = 0; i < tFXS.Count; i++)
        {
            if (tFXS[i].gameObject.activeInHierarchy != true)
            {
                tFXS[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                tFXS[i].gameObject.transform.position = bulletSpawnPoss[i].transform.position;
            }
        }
    }
    private void TrailFXSpawner()
    {
        for (int i = 0; i < tFXS.Count; i++)
        {
            tFXS[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            tFXS[i].transform.position = bulletSpawnPoss[i].transform.position;
            tFXS[i].gameObject.SetActive(true);
            tFXS[i].GetComponent<Rigidbody>().velocity += bulletSpawnPoss[i].transform.forward * 1000f * Time.time;
            currentTrail = tFXS[i].gameObject;
            tFXS2.Add(currentTrail);
            tFXS.RemoveAt(i);
            Invoke(nameof(TrailPool), 1f);
        }
    }
    private void TrailPool()
    {
        tFXS2[0].gameObject.transform.localPosition = Vector3.zero;
        tFXS2[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
        tFXS2[0].gameObject.SetActive(false);
        tFXS.Add(tFXS2[0].gameObject);
        tFXS2.RemoveAt(0);
    }
    private void SpawnBulletHole()
    {
        for (int i = 0; i < bulletSpawnPoss.Count; i++)
        {
            RaycastHit holeHit;
            if (Physics.Raycast(bulletSpawnPoss[i].position, bulletSpawnPoss[i].transform.forward, out holeHit, Mathf.Infinity, floorMask))
            {
                HoleFXSpawner();
                float x = holeHit.point.x;
                float y = holeHit.point.y;
                x += Mathf.Cos(Time.time * i * 45) * range;
                y += Mathf.Sin(Time.time * i * 45) * range;
                Vector3 pos = new Vector3(x, y, holeHit.point.z);
                currentHoleFX.transform.position = pos;
            }
        }
    }
    private void HoleFXSpawner()
    {
        holeFxs[0].gameObject.SetActive(true);
        currentHoleFX = holeFxs[0].gameObject;
        holeFxs2.Add(currentHoleFX);
        holeFxs.RemoveAt(0);
        Invoke(nameof(HoleFXPool), 0.3f);
    }
    private void HoleFXPool()
    {
        holeFxs2[0].gameObject.SetActive(false);
        holeFxs.Add(holeFxs2[0].gameObject);
        holeFxs2.RemoveAt(0);
    }
    #endregion

    // Basic Cooldowns

    #region CoolDowns
    IEnumerator CoolDown4Capsule()
    {
        yield return new WaitForSeconds(1.5f);
        canCapsule = true;
    }
    IEnumerator CoolDown4Reload()
    {
        yield return new WaitForSeconds(4f);
        anim.Stop();
        canReload = true;
        currentMagAmmo = 8;
    }
    IEnumerator CoolDown4Fire()
    {
        yield return new WaitForSeconds(1.5f);
        canFire = true;
        canFX = true;
    }
    IEnumerator CoolDown4Muzzle()
    {
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.gameObject.SetActive(false);
        muzzleFlash.Stop();
    }
    #endregion

    private void GiveDamage()
    {
        if (gameObject.activeInHierarchy == true)
        {
            for (int i = 0; i < bulletSpawnPoss.Count; i++)
            {
                RaycastHit zHit;
                if (Physics.Raycast(bulletSpawnPoss[i].position, bulletSpawnPoss[i].transform.forward, out zHit, Mathf.Infinity, zombieMask))
                {
                    // Default Zombie Gets Damage
                    if (zHit.collider.gameObject.GetComponentInParent<Enemy>() != null)
                    {
                        if (zHit.collider.gameObject.CompareTag("Body"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(15);
                        }
                        if (zHit.collider.gameObject.CompareTag("Head"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(30);
                            zHit.collider.gameObject.GetComponentInParent<Animation>().Play("HeadShot");
                        }
                        if (zHit.collider.gameObject.CompareTag("Legs"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(10);
                        }
                        if (zHit.collider.gameObject.CompareTag("Arms"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(10);
                        }
                    }
                    // Boomer Gets Damage
                    if (zHit.collider.gameObject.GetComponentInParent<FatZmbScr>() != null)
                    {
                        if (zHit.collider.gameObject.CompareTag("Body"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(15);
                            enemyHealth = zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetHealth(enemyHealth);
                            if (enemyHealth <= 0)
                            {
                                folder.PlayOneShot(boomSound);
                            }
                        }
                        if (zHit.collider.gameObject.CompareTag("Head"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(30);
                            enemyHealth = zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetHealth(enemyHealth);
                            if (enemyHealth <= 0)
                            {
                                folder.PlayOneShot(boomSound);
                            }
                        }
                        if (zHit.collider.gameObject.CompareTag("Legs"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(10);
                            enemyHealth = zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetHealth(enemyHealth);
                            if (enemyHealth <= 0)
                            {
                                folder.PlayOneShot(boomSound);
                            }
                        }
                        if (zHit.collider.gameObject.CompareTag("Arms"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(10);
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
                            zHit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(15);
                            enemyHealth2 = zHit.collider.gameObject.GetComponentInParent<Flamer>().GetHealth(enemyHealth2);
                            if (enemyHealth2 <= 0)
                            {
                                folder.PlayOneShot(boomSound);
                            }
                        }
                        if (zHit.collider.gameObject.CompareTag("Head"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(30);
                            enemyHealth2 = zHit.collider.gameObject.GetComponentInParent<Flamer>().GetHealth(enemyHealth2);
                            if (enemyHealth2 <= 0)
                            {
                                folder.PlayOneShot(boomSound);
                            }
                        }
                        if (zHit.collider.gameObject.CompareTag("Legs"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(10);
                            enemyHealth2 = zHit.collider.gameObject.GetComponentInParent<Flamer>().GetHealth(enemyHealth2);
                            if (enemyHealth2 <= 0)
                            {
                                folder.PlayOneShot(boomSound);
                            }
                        }
                        if (zHit.collider.gameObject.CompareTag("Arms"))
                        {
                            zHit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(10);
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
    }
    // Recoil  
    private void Recoil()
    {
        recTimer += Time.deltaTime;
        arm.localPosition = new Vector3(startArmPos.x + Mathf.Sin(recTimer) * recoilAmount.x, startArmPos.y + Mathf.Sin(recTimer) * recoilAmount.y, startArmPos.z + Mathf.Sin(recTimer) * recoilAmount.z);
        gameObject.transform.localPosition = new Vector3(startPos.x + Mathf.Sin(recTimer) * recoilAmount.x, startPos.y + Mathf.Sin(recTimer) * recoilAmount.y, startPos.z + Mathf.Sin(recTimer) * recoilAmount.z);
    }
    // Stops Recoil
    private void NonRecoil()
    {
        recTimer = 0;
        gameObject.transform.localPosition = new Vector3(Mathf.Lerp(gameObject.transform.localPosition.x, startPos.x, Time.deltaTime * recoilSpeed), Mathf.Lerp(gameObject.transform.localPosition.y, startPos.y, Time.deltaTime * recoilSpeed), Mathf.Lerp(gameObject.transform.localPosition.z, startPos.z, Time.deltaTime * recoilSpeed));
        arm.localPosition = new Vector3(Mathf.Lerp(arm.localPosition.x, startArmPos.x, Time.deltaTime * recoilSpeed), Mathf.Lerp(arm.localPosition.y, startArmPos.y, Time.deltaTime * recoilSpeed), Mathf.Lerp(arm.localPosition.z, startArmPos.z, Time.deltaTime * recoilSpeed));
    }
    // Spawns Capsule
    private void SpawnCapsule()
    {
        canCapsule = false;
        sCapsules[0].gameObject.SetActive(true);
        currentCapsule = sCapsules[0].gameObject;
        sCapsules2.Add(currentCapsule);
        currentCapsule.gameObject.GetComponent<Rigidbody>().velocity = transform.right * 5f;
        currentCapsule.transform.position = emptyBulletSpawnPos.position;
        sCapsules.RemoveAt(0);
        Invoke(nameof(CapsulePool), 1.5f);
        StartCoroutine(nameof(CoolDown4Capsule));
    }
    private void CapsulePool()
    {
        sCapsules2[0].gameObject.SetActive(false);
        sCapsules.Add(sCapsules2[0]);
        sCapsules2.RemoveAt(0);
    }  
    // Fire System And FX
    private void Fire()
    {
        magAmmo--;
        folder.PlayOneShot(shootClip);
        muzzleFlash.gameObject.SetActive(true);
        muzzleFlash.Play();
        canFire = false;
        canFX = false;
        StartCoroutine(nameof(CoolDown4Fire));
        StartCoroutine(nameof(CoolDown4Muzzle));
    }   
    // Reload System And Mag Change
    private void Reload()
    {
        if (totalAmmo != 0 && canReload == true && magAmmo != 8)
        {
            canReload = false;
            currentMagAmmo -= magAmmo;
            totalAmmo -= currentMagAmmo;
            magAmmo += currentMagAmmo;
            folder.PlayOneShot(reloadSound);
            anim.Play("ShoutGunReload");
            StartCoroutine(nameof(CoolDown4Reload));
        }
    }      
    public void IncreaseShoutGunAmmo(int value)
    {
        totalAmmo += value;
    }
}
