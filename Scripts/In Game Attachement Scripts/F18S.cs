using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class F18S : MonoBehaviour
{
    #region Lists
    public List<GameObject> spawnPossies = new List<GameObject>();
    public List<GameObject> f18s = new List<GameObject>();
    #endregion

    #region Statics
    private string sceneName;
    private AudioClip clip;
    private AudioSource folder;
    public bool canF18Spawn, canPlay;
    private int random;
    private GameObject carrier;
    #endregion

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        clip = GetComponent<AudioSource>().clip;
        folder = GetComponent<AudioSource>();
        random = Random.Range(80, 90);
        carrier = GameObject.Find("Carrier");
        InvokeRepeating(nameof(RandomBool), 0, random);
        canPlay = false;
        canF18Spawn = false;
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
        if (canF18Spawn == true)
        {
            if (Time.timeScale != 0)
            {
                SpawnRandomF18();
            }
        }
        else
        {
            DisableJetFlight();
        }
        #endregion

    }
    private void RandomBool()
    {
        canF18Spawn = Random.value > 0.95f;
        canPlay = canF18Spawn;
        if (canPlay == true)
        {
            if (Time.timeScale != 0)
            {
                folder.PlayOneShot(clip);
            }
        }
    }
    private void SpawnRandomF18()
    {
        for (int i = 0; i < f18s.Count; i++)
        {
            f18s[i].SetActive(true);
            f18s[i].transform.position += transform.forward * Time.deltaTime * 500;
            f18s[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            float distance = Vector3.Distance(f18s[i].transform.position, carrier.transform.position);
            if (distance <= 2000f && sceneName != "Level1")
            {
                f18s[i].transform.rotation = Quaternion.Euler(0, 0, f18s[i].transform.rotation.z + Time.time * 100f);
            }
        }
    }
    private void DisableJetFlight()
    {
        for (int i = 0; i < f18s.Count; i++)
        {
            f18s[i].SetActive(false);
            f18s[i].transform.position = spawnPossies[i].transform.position;
            f18s[i].transform.rotation = spawnPossies[i].transform.rotation;
        }
    }
}
