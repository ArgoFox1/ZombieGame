using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    [Header("In Game UIs")]

    #region UIs Variables
    private bool isPaused;
    public bool isBuying;
    public bool canSpawn;
    public TMPro.TextMeshProUGUI LostTMPCountdown;
    public TMPro.TextMeshProUGUI lostTMP;
    public TMPro.TextMeshProUGUI moneyTMP;
    public TMPro.TextMeshProUGUI timeTMP;
    public TMPro.TextMeshProUGUI healthTMP;
    public Image lostImage;
    public Image buyMenuImage;
    public Image pauseImage;
    public Image timeImage;
    #endregion

    [Header("Guns")]

    #region Gun Variables
    public GameObject pistol;
    public Rafaels rafaels;
    public M16 m16;
    public Ak ak;
    public Beretta beretta;
    public M4A4 m4a4;
    public ShoutGun shoutgun;
    public AK74N ak74;

    #endregion    

    [Header("Properties")]

    #region Static Variables
    private GameObject currentCredit;
    private bool canCredit;
    public int count;
    public RawImage creditImage;
    public AudioSource folder;
    public AudioSource cFolder;
    private AudioClip clip;
    public Light sun;
    public Apache apach;  
    private string sceneName;

    #region GameObjects && Transforms
    public Transform buyZone;
    public Transform player;
    private GameObject currentJet;
    public GameObject carrier;
    public GameObject deadCam;
    private GameObject zombie;
    private GameObject currentZombie;
    private GameObject currentFatZombie;
    private GameObject currentFlamer;
    #endregion

    #region Floats && Integers
    private float distance;
    private int random;
    public float buyTime;
    public bool isDead;
    [SerializeField] private int money = 0;
    [SerializeField] private float gameTime;
    [SerializeField] private float currentTime;
    [SerializeField] private float totalTime;
    private float countDown;
    public float playerHealth;
    private int spawnTime;
    #endregion    

    #endregion   

    [Header("Lists")]

    #region Lists
    public List<GameObject> credits = new List<GameObject>();
    public List<GameObject> biles = new List<GameObject>();
    public List<GameObject> flamers = new List<GameObject>();
    public List<GameObject> turrets = new List<GameObject>();
    public List<GameObject> fatZombies = new List<GameObject>();
    public List<Transform> spawnPossies = new List<Transform>();
    public List<GameObject> zombies = new List<GameObject>();
    public List<GameObject> jets = new List<GameObject>();
    #endregion  

    [Header("Pools")]

    #region Pools
    private List<GameObject> credits2 = new List<GameObject>();
    private List<GameObject> flamers2 = new List<GameObject>();
    private List<GameObject> fatZombies2 = new List<GameObject>();
    private List<GameObject> zombies2 = new List<GameObject>();
    #endregion

    #region Singleton
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if(_instance == null)
                {
                    _instance = new GameObject("Game Manager").AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    #endregion   

    private void Awake()
    { 
        random = Random.Range(10, 40);
        sceneName = SceneManager.GetActiveScene().name;
        gameTime = 300f;
        if (_instance is not null) Destroy(this);      
        spawnTime = 30;
    }
    private void Start()
    {
        creditImage.gameObject.SetActive(false);
        cFolder = creditImage.GetComponent<AudioSource>();
        canCredit = true;
        clip = folder.clip;
        isDead = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;            
        deadCam.gameObject.SetActive(false);    
        countDown = 3f;
        canSpawn = true;
        folder.PlayOneShot(clip);
        InvokeRepeating(nameof(Timer), 0.1f, 1f);     
        if(canSpawn == true)
        {
            InvokeRepeating(nameof(DefaultZombieSpawn), 0.1f, spawnTime);
            InvokeRepeating(nameof(SpawnFatZombie), 0.1f, spawnTime + 30f);
            InvokeRepeating(nameof(SpawnFlamerZombie), 0.1f, spawnTime + 20f);
        }            
    }
    private void Update()
    {
        if (buyTime > 0f)
        {
            canSpawn = false;
        }

        #region SunSettings
        SunSettings();
        #endregion

        #region Kinda Stuff

        #region SoundSettings
        if (Time.timeScale == 0)
        {
            folder.Pause();
            cFolder.Pause();
        }
        else
        {
            folder.UnPause();
            cFolder.UnPause();
        }
        #endregion

        #region Death Of Player
        if (playerHealth <= 0)
        {
            Dead();
        }
        if(isDead == true)
        {
            Dead();
        }
        if (countDown <= 0)
        {
            isPaused = false;
            countDown = 3f;
            if(sceneName == "Level1")
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                SceneManager.LoadScene(2);
            }
        }
        #endregion

        #endregion

        #region UIs Utilitys     

        #region Health
        healthTMP.text = playerHealth.ToString();
        if(playerHealth <= 0)
        {
            player.gameObject.SetActive(false);
        }
        #endregion

        #region Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {           
            isPaused = !isPaused;       
        }
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseImage.gameObject.SetActive(true);
            Time.timeScale = 0f;            
        }
        else
        {            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseImage.gameObject.SetActive(false);
            Time.timeScale = 1f;          
        }
        #endregion

        #region Money
        moneyTMP.text = money.ToString();
        #endregion

        #region BuyMenuOpen      
        distance = Vector3.Distance(player.position, buyZone.position);
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (distance <= 500f && buyTime > 0)
            {
                isBuying = !isBuying;
            }
        }               
        if(buyMenuImage.gameObject.activeInHierarchy == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if(this.money >= 500 && distance <=5f)
                {
                    BuyM16();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if(this.money >=10 && distance <= 5f)
                {
                    BuyPistolAmmo();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if(this.money >= 10 && distance <=5f)
                {
                    BuyM16Ammo();
                }
            }          
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if(this.money >=10 && distance <= 5f)
                {
                    BuyAk47Ammo();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if(this.money >= 80f && distance <= 5f)
                {
                    BuyAirStrike();
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if(this.money >= 30 && distance <= 5f)
                {
                    BuyTurret1();
                }
            }           
        }      
        if (isBuying)
        {
            buyMenuImage.gameObject.SetActive(true);
            pauseImage.gameObject.SetActive(false);                     
        }
        else
        {
            buyMenuImage.gameObject.SetActive(false);                       
        }
        #endregion

        #endregion

    }     
    // What if Player Deads ?
    private void Dead()
    {
        deadCam.gameObject.SetActive(true);
        lostImage.gameObject.SetActive(true);
        countDown -= Time.deltaTime;
        LostTMPCountdown.text = countDown.ToString() + "\nGame Gonna Start After Countdown\n";
    }
    // Sun Rises and Sunsets
    private void SunSettings()
    {
        float x = sun.transform.rotation.x;
        x += Time.time / 5;
        sun.transform.rotation = Quaternion.Euler(x, 0, 0);
    }
    // Spawn Flamethrower Zombie
    private void SpawnFlamerZombie()
    {
        int random = Random.Range(0, flamers.Count);
        for (int i = 0; i < flamers.Count; i++)
        {
            flamers[0].gameObject.SetActive(true);
            currentFlamer = flamers[0].gameObject;
            currentFlamer.transform.position = spawnPossies[random].transform.position;
            flamers2.Add(currentFlamer);
            flamers.RemoveAt(0);
            if(flamers.Count != 0)
            {
                Invoke(nameof(FlamerPool), 180f);
            }
            else
            {
                Invoke(nameof(FlamerPool), 1f);
            }
        }       
    }
    private void FlamerPool()
    {
        flamers2[0].gameObject.SetActive(false);
        flamers.Add(flamers2[0]);
        flamers2.RemoveAt(0);
    }
    // Spawns FatZombie
    private void SpawnFatZombie()
    {
        int random = Random.Range(0, spawnPossies.Count);
        for (int i = 0; i < fatZombies.Count; i++)
        {
            fatZombies[0].gameObject.SetActive(true);
            currentFatZombie = fatZombies[0].gameObject;
            currentFatZombie.transform.position = spawnPossies[random].transform.position;
            fatZombies2.Add(currentFatZombie);
            fatZombies.RemoveAt(0);
            if (fatZombies.Count != 0)
            {
                Invoke(nameof(FatZombiePool), 180f);
            }
            else
            {
                Invoke(nameof(FatZombiePool), 1f);
            }
        }       
    }
    private void FatZombiePool()
    {
        fatZombies2[0].gameObject.SetActive(false);
        fatZombies.Add(fatZombies2[0]);
        fatZombies2.RemoveAt(0);
    }
    // Spawns Default Zombie
    private void DefaultZombieSpawn()
    {      
        int random = Random.Range(0, spawnPossies.Count);
        for (int i = 0; i < zombies.Count; i++)
        {
            zombies[0].gameObject.SetActive(true);
            currentZombie = zombies[0].gameObject;
            currentZombie.transform.position = spawnPossies[random].transform.position;
            zombies2.Add(currentZombie);
            zombies.RemoveAt(0);
            if (zombies.Count != 0)
            {
                Invoke(nameof(ZombiePool), 180f);
            }
            else
            {
                Invoke(nameof(ZombiePool), 1f);
            }
        }       
    }
    private void ZombiePool()
    {
        for (int i = 0; i < zombies2.Count; i++)
        {
            zombies2[i].gameObject.SetActive(false);
            zombies.Add(zombies2[i]);
            zombies2.RemoveAt(i);
        }
    }
    public int IncreaseMoney(int money)
    {       
        this.money += money;
        return this.money;
    }
    public void DecreaseMoney(int value)
    {
        this.money -= value;
    }    
    public  float DecreaseHealth(float damage)
    {
        playerHealth -= damage;
        return playerHealth;
    }
    // Time Settings && Buy Time Settings
    public void Timer()
    {       
        buyTime--;
        currentTime--;
        gameTime--;
        timeTMP.text = gameTime.ToString();
        timeImage.fillAmount = gameTime / totalTime;       
        if(currentTime <= 0f)
        {
            currentTime += 100f;
            buyTime += 30f;
        }
        if(buyTime <= 0f)
        {
            buyTime = 0f;
        }       
        if (gameTime <= 0f)
        {
            folder.Stop();
            if (sceneName == "Level1")
            {
                SceneManager.LoadScene(2);
            }
            if (sceneName == "Level2")
            {
                creditImage.gameObject.SetActive(true);
                SoundMaker();
                if (canCredit == true)
                {
                    EndCredits();
                }
            }
        }       
    }
    private void SoundMaker()
    {
       if(cFolder.isPlaying != true)
       {
           cFolder.PlayOneShot(cFolder.clip);
       }
    }
    private void EndCredits()
    {
        canCredit = false;       
        credits[0].SetActive(true);
        currentCredit = credits[0].gameObject;
        credits2.Add(currentCredit);
        credits.RemoveAt(0);
        Invoke(nameof(CreditPool), 2f);
        StartCoroutine(nameof(CoolDown4Credits));
    }
    private void CreditPool()
    {
        credits2[0].SetActive(false);
        credits.Add(credits[0]);
        credits2.RemoveAt(0);
    }
    IEnumerator CoolDown4Credits()
    {
        yield return new WaitForSeconds(2f);
        canCredit = true;
        yield return new WaitForSeconds(15f);
        SceneManager.LoadScene(0);
    }   
    public void BuyM16()
    {
        if(money >= 500)
        {
            // add guns
        }
    }
    public void GoMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Retry()
    {
        if(sceneName == "Level1")
        {
            SceneManager.LoadScene(1);
        }
        if (sceneName == "Level2")
        {
            SceneManager.LoadScene(2);
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void BuyAk47Ammo()
    {
        if(this.money >= 10)
        {
            ak.GetComponent<Ak>().IncreaseAk47Ammo(30);
            DecreaseMoney(10);
        }       
    }
    public void BuyM16Ammo()
    {
        if(this.money >= 10)
        {
            m16.GetComponent<M16>().IncreaseM16Ammo(20);
            DecreaseMoney(10);
        }      
    }
    public void BuyPistolAmmo()
    {
        if(this.money >= 10)
        {
            pistol.GetComponent<Pistol>().IncreasePistolAmmo(7);
            DecreaseMoney(10);
        }
    }
    public void BuyBerettaAmmo()
    {
        if (this.money >= 10)
        {
            beretta.GetComponent<Beretta>().IncreaseBerettaAmmo(15);
            DecreaseMoney(10);
        }
    }
    public void BuyAk74Ammo()
    {
        if (this.money >= 10)
        {
            ak74.GetComponent<AK74N>().IncreaseAk74Ammo(30);
            DecreaseMoney(10);
        }
    }
    public void BuyM4A4Ammo()
    {
        if (this.money >= 10)
        {
            m4a4.GetComponent<M4A4>().IncreaseM4A4Ammo(30);
            DecreaseMoney(10);
        }
    }
    public void BuyShoutGunAmmo()
    {
        if (this.money >= 10)
        {
            shoutgun.GetComponent<ShoutGun>().IncreaseShoutGunAmmo(8);
            DecreaseMoney(10);
        }
    }
    public void BuyTurret1()
    {
        if(this.money >= 20)
        {
            turrets[0].gameObject.GetComponent<Turret>().CanWork(30f, true);
            turrets[1].gameObject.GetComponent<Turret>().CanWork(30f, true);
            DecreaseMoney(20);
        }
    }   
    public void BuyAirStrike()
    {
        if(this.money >= 80)
        {
            DecreaseMoney(80);
            rafaels.GetComponent<Rafaels>().canSpawn = true;
        }
    }    
    public void BuyApache()
    {
        if(this.money >= 80)
        {
            apach.gameObject.SetActive(true);
            apach.GetComponent<Apache>().CanWork(150, true);
            DecreaseMoney(80);
        }
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Resume()
    {
        Time.timeScale = 1;
    }
    public float GetHealth(float health)
    {
        health = this.playerHealth;
        return this.playerHealth;
    }
    public float GetBuyTime(float time)
    {
        time = this.buyTime;
        return this.buyTime;
    }  
}
