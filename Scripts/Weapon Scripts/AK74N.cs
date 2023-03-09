using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AK74N : MonoBehaviour 
{
    [Header("Ak74 Properties")]

    #region Enemy Variables
    public LayerMask zombieMask;
    private float enemyHealth, enemyHealth2;
    public AudioClip boomSound;
    #endregion

    #region FX Variables
    private ParticleSystem currentBlood;
    private GameObject currentTrail;
    public LayerMask floorMask;
    private GameObject currentholeFX;
    private bool canFX;
    #endregion

    #region Reload Variables
    public GameObject mag;
    private bool canReload;
    public AudioClip reloadSound;
    private GameObject currentMag;
    #endregion

    #region Fire Variables
    public AudioClip shootClip;
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
    public Transform bulletSpawnPos;
    public Transform emptyBulletSpawnPos;
    #endregion

    #region Recoil Variables
    private float recTimer;
    private Vector3 startPos;
    private Vector3 startArmPos;
    public Vector3 recoilAmount;
    [SerializeField] private float recoilSpeed;
    #endregion

    #region ChargingHandle Variables
    [SerializeField] private float chSpeed;
    public Transform chargingHandle;
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
    public List<GameObject> ak74Capsules = new List<GameObject>();
    public List<GameObject> mags = new List<GameObject>();
    public List<GameObject> holeFxs = new List<GameObject>();
    #endregion

    #region Pools
    [Header("Pools")]
    private List<ParticleSystem> bloodFXS2 = new List<ParticleSystem>();
    private List<GameObject> tFXS2= new List<GameObject>();
    private List<GameObject> ak74Capsules2 = new List<GameObject>();
    private List<GameObject> mags2 = new List<GameObject>();
    private List<GameObject> holeFxs2 = new List<GameObject>();
    #endregion

    private void Awake()
    {
        startArmPos = arm.localPosition;
        startPos = gameObject.transform.localPosition;
    }
    private void Start()
    {
        InvokeRepeating(nameof(LocateMagPos), 0, 3f);
        muzzleFlash.gameObject.SetActive(false);
        muzzleFlash.Pause();
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
        if (currentMagAmmo == 0 && Input.GetKeyDown(shootBtn) && Time.timeScale != 0)
        {
            folder.PlayOneShot(emptyGunSound);
        }
        if(Time.timeScale == 0)
        {
            folder.Pause();
        }

        #region Shoot
        if (gameObject.activeInHierarchy == true && gm.buyMenuImage.gameObject.activeInHierarchy != true && Time.timeScale != 0)
        {
            if (Input.GetKeyDown(shootBtn) || Input.GetKey(shootBtn) && canFire == true && canCapsule == true)
            {
                if (Input.GetKey(scopeBtn) != true)
                {
                    if (currentMagAmmo != 0 && canReload == true && canFX == true)
                    {
                        BloodFXSpawner();
                        ChargingHandleFX();
                        GiveDamage();
                        Recoil();
                        Fire();
                        SpawnCapsule();
                        SpawnBulletHole();
                        TrailFXSpawner();
                    }
                    else
                    {
                        StopChargingHandleFX();
                        NonRecoil();
                    }
                }
            }
            else
            {
                StopChargingHandleFX();
                NonRecoil();
            }
        }
        #endregion       

        #region Reload
        if (Input.GetKeyDown(reloadKey) && canReload == true && gameObject.activeInHierarchy == true)
        {
            if (gm.buyMenuImage.gameObject.activeInHierarchy != true && Time.timeScale != 0)
            {
                ReloadBullet();
            }
        }
        #endregion

    }
    // FXS

    #region FXS
    private void SpawnBulletHole()
    {
        RaycastHit holeHit;
        if (Physics.Raycast(bulletSpawnPos.position, bulletSpawnPos.transform.forward, out holeHit, Mathf.Infinity, floorMask))
        {
            HoleFXSpawner();
            currentholeFX.transform.position = holeHit.point;
        }
    }
    private void HoleFXSpawner()
    {
        holeFxs[0].gameObject.SetActive(true);
        currentholeFX = holeFxs[0].gameObject;
        holeFxs2.Add(currentholeFX);
        holeFxs.RemoveAt(0);
        Invoke(nameof(HoleFXPool), 0.3f);
    }
    private void HoleFXPool()
    {
        holeFxs2[0].gameObject.SetActive(false);
        holeFxs.Add(holeFxs2[0].gameObject);
        holeFxs2.RemoveAt(0);
    }
    private void LocateTrailPos()
    {
        for (int i = 0; i < tFXS.Count; i++)
        {
            if (tFXS[i].gameObject.activeInHierarchy != true)
            {
                tFXS[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                tFXS[i].gameObject.transform.position = bulletSpawnPos.transform.position;
            }
        }
    }
    private void TrailFXSpawner()
    {
        tFXS[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
        tFXS[0].transform.position = bulletSpawnPos.transform.position;
        tFXS[0].gameObject.SetActive(true);
        tFXS[0].GetComponent<Rigidbody>().velocity += bulletSpawnPos.transform.forward * 10000f * Time.time;
        currentTrail = tFXS[0].gameObject;
        tFXS2.Add(currentTrail);
        tFXS.RemoveAt(0);
        Invoke(nameof(TrailPool), 0.5f);
    }
    private void TrailPool()
    {
        tFXS2[0].gameObject.transform.localPosition = Vector3.zero;
        tFXS2[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
        tFXS2[0].gameObject.SetActive(false);
        tFXS.Add(tFXS2[0].gameObject);
        tFXS2.RemoveAt(0);
    }
    private void BloodFXSpawner()
    {
        RaycastHit bHit;
        if (Physics.Raycast(bulletSpawnPos.position, bulletSpawnPos.transform.forward, out bHit, Mathf.Infinity, zombieMask))
        {
            canFX = false;
            BloodFXS();
            currentBlood.gameObject.transform.position = bHit.point;
            currentBlood.gameObject.SetActive(true);
            currentBlood.Play();
            StartCoroutine(nameof(CoolDown4Fire));
        }
    }
    private void BloodFXS()
    {
        currentBlood = bloodFXS[0];
        bloodFXS2.Add(currentBlood);
        bloodFXS.RemoveAt(0);
        Invoke(nameof(BloodPool), 0.5f);
    }
    private void BloodPool()
    {
        bloodFXS2[0].gameObject.SetActive(false);
        bloodFXS2[0].Stop();
        bloodFXS.Add(bloodFXS2[0]);
        bloodFXS2.RemoveAt(0);
    }
    #endregion

    // Basic CoolDowns

    #region CoolDowns
    IEnumerator CoolDown4Capsule()
    {
        yield return new WaitForSeconds(0.1f);
        canCapsule = true;
    }
    IEnumerator CoolDown4Fire()
    {
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.gameObject.SetActive(false);
        muzzleFlash.Pause();
        canFire = true;
        canFX = true;
    }
    IEnumerator CoolDown4Reload()
    {
        yield return new WaitForSeconds(3f);
        canReload = true;
        anim.Stop();
        magAmmo = 30;
    }
    #endregion

    // Damage Dealing
    private void GiveDamage()
    {
        RaycastHit zHit;
        if (gameObject.activeInHierarchy == true)
        {
            if (Physics.Raycast(bulletSpawnPos.position, bulletSpawnPos.transform.forward, out zHit, Mathf.Infinity, zombieMask))
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
    private void ChargingHandleFX()
    {
        float z = chargingHandle.localPosition.z;
        z = Mathf.Lerp(z, -0.04f, Time.deltaTime * chSpeed);
        Vector3 akCharging = new Vector3(chargingHandle.localPosition.x, chargingHandle.localPosition.y, z);
        chargingHandle.localPosition = Vector3.Lerp(chargingHandle.localPosition, akCharging, Time.deltaTime * chSpeed);
    }
    private void StopChargingHandleFX()
    {
        float z = chargingHandle.localPosition.z;
        z = Mathf.Lerp(z, 0.1907f, Time.deltaTime * chSpeed / 4);
        Vector3 akCharging = new Vector3(chargingHandle.localPosition.x, chargingHandle.localPosition.y, z);
        chargingHandle.localPosition = Vector3.Lerp(chargingHandle.localPosition, akCharging, Time.deltaTime * chSpeed);
    }    
    // Spawns Capsule
    private void SpawnCapsule()
    {
        canCapsule = false;
        ak74Capsules[0].gameObject.SetActive(true);
        currentCapsule = ak74Capsules[0].gameObject;
        ak74Capsules2.Add(currentCapsule);
        currentCapsule.gameObject.GetComponent<Rigidbody>().velocity = transform.right * 10f;
        currentCapsule.transform.position = emptyBulletSpawnPos.position;
        ak74Capsules.RemoveAt(0);
        Invoke(nameof(CapsulePool), 1f);
        StartCoroutine(nameof(CoolDown4Capsule));
    }  
    private void CapsulePool()
    {
        ak74Capsules2[0].gameObject.SetActive(false);
        ak74Capsules.Add(ak74Capsules2[0]);
        ak74Capsules2.RemoveAt(0);
    }   
    // Fire System And FX
    private void Fire()
    {
        currentMagAmmo--;
        folder.PlayOneShot(shootClip);
        muzzleFlash.gameObject.SetActive(true);
        muzzleFlash.Play();
        canFire = false;
        canFX = false;
        StartCoroutine(nameof(CoolDown4Fire));
    }        
    // Reload System And Mag Change
    private void LocateMagPos()
    {
        for (int i = 0; i < mags.Count; i++)
        {
            mags[i].transform.localPosition = mag.transform.localPosition;
            if (mags[i].gameObject.activeInHierarchy != true && canReload == false)
            {
                mags[i].gameObject.SetActive(true);
                mags[i].gameObject.transform.localPosition = mag.transform.localPosition;
                mags[i].transform.localRotation = Quaternion.identity;
            }
        }
    }
    private void ReloadBullet()
    {
        if (canReload == true && currentMagAmmo != 30  && totalAmmo != 0 && magAmmo - currentMagAmmo <= totalAmmo) 
        {
            canReload = false;
            magAmmo -= currentMagAmmo;
            totalAmmo -= magAmmo;
            currentMagAmmo += magAmmo;
            ChangeMag();
        }
        if (magAmmo - currentMagAmmo >= totalAmmo && canReload == true && totalAmmo != 0 && currentMagAmmo != 30)
        {
            currentMagAmmo += totalAmmo;
            totalAmmo = 0;
            ChangeMag();
        }
    }
    private void ChangeMag()
    {
        canReload = false;
        mags[0].gameObject.SetActive(true);
        currentMag = mags[0].gameObject;
        if (currentMag.gameObject.GetComponent<Rigidbody>() == null)
        {
            currentMag.AddComponent<Rigidbody>();
        }
        currentMag.transform.parent = null;
        mags2.Add(currentMag);
        mags.RemoveAt(0);
        folder.PlayOneShot(reloadSound);
        anim.Play("74Reload");
        Invoke(nameof(MagPool), 10f);
        StartCoroutine(nameof(CoolDown4Reload));
    }
    private void MagPool()
    {
        Destroy(mags2[0].GetComponent<Rigidbody>());
        mags2[0].transform.localPosition = mag.transform.localPosition;
        mags2[0].gameObject.transform.parent = gameObject.transform;
        mags2[0].gameObject.SetActive(false);
        mags.Add(mags2[0].gameObject);
        mags2.RemoveAt(0);
    }   
    public void IncreaseAk74Ammo(int value)
    {
        totalAmmo += value;
    }  
}