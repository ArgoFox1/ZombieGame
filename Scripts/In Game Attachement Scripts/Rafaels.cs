using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class Rafaels : MonoBehaviour
{
    public List<GameObject> jets = new List<GameObject>();
    public List<GameObject> bombs = new List<GameObject>();
    public List<ParticleSystem> afterBurners = new List<ParticleSystem>();
    public bool canSpawn;
    private AudioSource soundFolder;
    private AudioClip clip;
    [SerializeField] private float speed;
    private void Start()
    {
        canSpawn = false;
        soundFolder = GetComponent<AudioSource>();
        clip = soundFolder.clip;
    }
    private void Update()
    {

        #region SoundSettings
        if(Time.timeScale == 0)
        {
            soundFolder.Pause();
        }
        else
        {
            soundFolder.UnPause();
        }
        #endregion

        #region Flight
        if(canSpawn == true)
        {
            Flight();
        }
        else
        {
            DisableFlight();
        }
        #endregion

    }   
    private void Flight()
    {
        for (int i = 0; i < jets.Count; i++)
        {
            jets[i].SetActive(true);
            afterBurners[i].gameObject.SetActive(true);
            afterBurners[i].Play();
            if(bombs.Count != 0)
            {
                jets[i].gameObject.transform.position += transform.forward * Time.deltaTime * speed;
            }
            else
            {
                jets[i].gameObject.transform.position += transform.forward * Time.deltaTime * speed * 2;
            }        
        }
        Invoke(nameof(SoundMaker), 1);
        StartCoroutine(nameof(CoolDown4Flight));
    }
    private void DisableFlight()
    {
        for (int i = 0; i < jets.Count; i++)
        {
            soundFolder.Stop();
            jets[i].SetActive(false);
            afterBurners[i].gameObject.SetActive(false);
            afterBurners[i].Stop();
        }
    }
    private void SoundMaker()
    {
        if(soundFolder.isPlaying != true)
        {
            soundFolder.PlayOneShot(clip);
        }
    }
    IEnumerator CoolDown4Flight()
    {
        yield return new WaitForSeconds(25);
        DisableFlight();
        canSpawn = false;
    }
}
