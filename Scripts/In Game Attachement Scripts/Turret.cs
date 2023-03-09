using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class Turret : MonoBehaviour
{
    [Header("Turret Properties")]

    #region Turret Variables
    public Transform turretMuzzle;
    public Light spotLight;
    public Transform turretBody;
    public Transform bulletSpawner;
    public bool canShoot;
    public bool canWork;
    public bool isShooting;
    private AudioSource soundFolder;
    public AudioClip shootSound;
    public float workTime;
    public ParticleSystem bloodFX;
    public ParticleSystem minigunFX;
    public ParticleSystem smokeFX;

    #endregion

    #region Enemy Detect Variables
    public LayerMask mask;
    public List<GameObject> enemies = new List<GameObject>();
    #endregion
   
    private void Start()
    {
        soundFolder = GetComponent<AudioSource>();
        spotLight.gameObject.SetActive(false);
        workTime = 0f;
        canWork = false;
        minigunFX.gameObject.SetActive(false);
        minigunFX.Stop();
        canShoot = true;      
        InvokeRepeating(nameof(DecreaseTime), 0.1f, 1f);       
    }
    private void Update()
    {     
        if (isShooting == false)
        {
            minigunFX.gameObject.SetActive(false);
            minigunFX.Stop();
            soundFolder.Stop();
            smokeFX.Play();        
            spotLight.gameObject.SetActive(false);
            Patrol();
            if(workTime <= 0)
            {
                turretBody.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }                   
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
        {
            foreach (var enem in enemies) 
            {
                turretBody.LookAt(Vector3.Lerp(turretBody.position, enem.transform.position, 5f * Time.deltaTime));
                RaycastHit hit;
                if (Physics.Raycast(bulletSpawner.position, bulletSpawner.TransformDirection(Vector3.forward), out hit, 30f, mask))
                {
                    if (hit.collider.gameObject.activeInHierarchy == true)
                    {
                        if (canShoot == true && workTime >0)
                        {                          
                            float z = turretMuzzle.localRotation.z;
                            z += Time.time * 100f;
                            turretMuzzle.localRotation = Quaternion.Euler(0, 0, z);
                            ParticleSystem newBlood = Instantiate(bloodFX, hit.point, Quaternion.identity);
                            newBlood.gameObject.SetActive(true);
                            isShooting = true;
                            canShoot = false;
                            if(hit.collider.gameObject.GetComponentInParent<Enemy>() != null)
                            {
                                hit.collider.gameObject.GetComponentInParent<Enemy>().GetDamage(20);
                            }
                            if(hit.collider.gameObject.GetComponentInParent<Enemy>() != null)
                            {
                                hit.collider.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(20);
                            }
                            if(hit.collider.gameObject.GetComponentInParent<Flamer>() != null)
                            {
                                hit.collider.gameObject.GetComponentInParent<Flamer>().GetDamage(20);
                            }
                            soundFolder.Play();
                            minigunFX.gameObject.SetActive(true);
                            minigunFX.Play();
                            spotLight.gameObject.SetActive(true);
                            StartCoroutine(nameof(CoolDown));                         
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
        {
            enemies.Add(other.gameObject);
        }
    }
    public void Patrol()
    {
        float y = turretBody.rotation.y;
        y = Mathf.Sin(Time.time) * 90f;
        Quaternion turretRot = Quaternion.Euler(turretBody.rotation.x, y, turretBody.rotation.z);
        turretBody.rotation = Quaternion.Lerp(turretBody.rotation, turretRot, Time.deltaTime * 1f);
    }
    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(0.3f);
        canShoot = true;       
        isShooting = false;
    }
    public void CanWork(float value, bool value2)
    {
        workTime += value;
        canWork = value2;
    }
    public void DecreaseTime()
    {
        workTime--;
        if(workTime <= 0)
        {
            workTime = 0;   
            canWork = false;            
        }
    }
}
