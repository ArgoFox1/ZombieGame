using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Navals : MonoBehaviour
{
    #region Lists
    public List<ParticleSystem> smallShotFXS = new List<ParticleSystem>();
    public List<GameObject> smBulletSpawns = new List<GameObject>();
    public List<GameObject> smTFXS = new List<GameObject>();
    public List<GameObject> tFXS = new List<GameObject>();
    public List<ParticleSystem> shotFXS = new List<ParticleSystem>();
    public List<GameObject> bulletSpawns = new List<GameObject>();
    #endregion   

    #region Static Variables
    private bool canSmallFX;
    private float time;
    private GameObject currentTrail;
    private Animation anim;
    private float random;
    private AudioSource folder;
    private AudioClip clip;
    private bool canActive;
    private bool canFX;
    #endregion

    private void Start()
    {
        folder = GetComponent<AudioSource>();
        clip = GetComponent<AudioSource>().clip;
        anim = GetComponent<Animation>();
        random = Random.Range(50, 60);
        InvokeRepeating(nameof(RandomBool), 0, random);
        canActive = false;
        canFX = true;
        canSmallFX = true;
        random = Random.Range(50, 80);
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

        #region GameSettings
        if (canFX == true && canActive == true)
        {
            ShotFXSpawner();
            TrailFXSpawner();
        }
        if (canSmallFX == true && canActive == true)
        {
            SmallShotFXSpawner();
            SmallTrailFXSpawner();
        }
        #endregion

    }
    private void RandomBool()
    {
        canActive = Random.value > 0.95f;     
    }
    private void SmallTrailFXSpawner()
    {
        for (int i = 0; i < smTFXS.Count; i++)
        {
            smTFXS[i].transform.rotation = smBulletSpawns[i].transform.rotation;
            smTFXS[i].transform.position = smBulletSpawns[i].transform.position;
            smTFXS[i].gameObject.SetActive(true);
            smTFXS[i].GetComponent<Rigidbody>().velocity += smTFXS[i].transform.forward * 100000f * Time.deltaTime;
        }
    }
    private void SmallShotFXSpawner()
    {
        canSmallFX = false;
        for (int i = 0; i < smallShotFXS.Count; i++)
        {
            smallShotFXS[i].gameObject.transform.position = smBulletSpawns[i].transform.position;
            smallShotFXS[i].gameObject.transform.rotation = smBulletSpawns[i].transform.rotation;
            smallShotFXS[i].gameObject.SetActive(true);
            smallShotFXS[i].Play();
            folder.Play();
        }
        StartCoroutine(nameof(CoolDown4SmallFX));
    }
    private void TrailFXSpawner()
    {
        for (int i = 0; i < tFXS.Count; i++)
        {
            tFXS[i].transform.rotation = bulletSpawns[i].transform.rotation;
            tFXS[i].transform.position = bulletSpawns[i].transform.position;
            tFXS[i].gameObject.SetActive(true);
            tFXS[i].GetComponent<Rigidbody>().velocity += tFXS[i].transform.forward * 100000f * Time.deltaTime;
        }
    }  
    private void ShotFXSpawner()
    {
        canFX = false;
        for (int i = 0; i < shotFXS.Count; i++)
        {
            shotFXS[i].gameObject.transform.position = bulletSpawns[i].transform.position;
            shotFXS[i].gameObject.transform.rotation = bulletSpawns[i].transform.rotation;
            shotFXS[i].gameObject.SetActive(true);
            shotFXS[i].Play();
            folder.Play();
        }
        StartCoroutine(nameof(CoolDown4FX));
    }   
    IEnumerator CoolDown4FX()
    {
        yield return new WaitForSeconds(2f);
        canFX = true;
    }
    IEnumerator CoolDown4SmallFX()
    {
        yield return new WaitForSeconds(0.5f);
        canSmallFX = true;
    }
}
