using System.Collections.Generic;
using UnityEngine;
public class Chopper : MonoBehaviour
{
    #region Lists
    public List<GameObject> choppers = new List<GameObject>();
    public List<GameObject> spawnPossies = new List<GameObject>();
    #endregion

    #region Statics
    private AudioClip clip;
    private AudioSource folder;
    public bool canChopperSpawn;
    private int random;
    private GameObject carrier;
    #endregion

    private void Start()
    {
        folder = GetComponent<AudioSource>();
        clip = folder.clip;
        random = Random.Range(60, 75);
        carrier = GameObject.Find("Carrier");
        InvokeRepeating(nameof(RandomBool), 0, random);
    }
    private void Update()
    {

        #region Sounds
        if (Time.timeScale == 0)
        {
            folder.Pause();
        }
        else
        {
            folder.UnPause();
        }
        #endregion

        #region Flight
        if (canChopperSpawn == true)
        {
            if (Time.timeScale != 0)
            {
                SpawnRandomChoppers();
            }
        }
        else
        {
            DisableChoppersFlight();
        }
        #endregion       

    }
    private void RandomBool()
    {
        canChopperSpawn = Random.value > 0.95f;
        if (canChopperSpawn == true)
        {
            folder.PlayOneShot(clip);
        }
        else
        {
            folder.Stop();
        }
    }
    private void SpawnRandomChoppers()
    {
        for (int i = 0; i < choppers.Count; i++)
        {
            choppers[i].SetActive(true);
            choppers[i].transform.position += transform.right * Time.deltaTime * 100;
            choppers[i].transform.rotation = Quaternion.Euler(30, 90, 0);
            float distance = Vector3.Distance(choppers[i].transform.position, carrier.transform.position);
        }     
    }
    private void DisableChoppersFlight()
    {
        for (int i = 0; i < choppers.Count; i++)
        {
            choppers[i].SetActive(false);
            choppers[i].transform.position = spawnPossies[i].transform.position;
            choppers[i].transform.rotation = spawnPossies[i].transform.rotation;
            folder.Stop();
        }      
    }  
}