using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
public class FatZmbScr : MonoBehaviour
{
    public SphereCollider spCollider;
    public ParticleSystem bileFX;
    public ParticleSystem bombedFX;
    private NavMeshAgent navAgent;
    public GameManager gameManager;
    private Animation ani;
    public GameObject player;
    public Transform fLaser;
    private bool canBile;
    private bool isDead;
    private float randomPosZ;
    [Header("Fat Zombie Properties")]
    [SerializeField] private float speed;
    [SerializeField] private float health = 100f;
    [SerializeField] private float pushSpeed;
    private AudioSource soundFolder;
    public AudioClip hitSound;
    public AudioClip fatZombieSound;
    public List<GameObject> impactParts = new List<GameObject>();
    private void Start()
    {
        spCollider.enabled = false;       
        ani = GetComponent<Animation>();
        navAgent = GetComponent<NavMeshAgent>();
        soundFolder = GetComponent<AudioSource>();
        canBile = true;
        navAgent.SetDestination(player.transform.position);
        health = 200;
        if(Time.time != 0)
        {
            soundFolder.PlayOneShot(fatZombieSound);
        }
    }
    private async void Update()
    {

        #region Sound Settings
        if (Time.timeScale == 0)
        {
            soundFolder.Stop();
        }
        if (Time.time != 0 && soundFolder.isPlaying != true)
        {
            soundFolder.Play();
        }
        #endregion

        #region Movement       
        gameObject.GetComponent<NavMeshAgent>().destination = player.transform.position;
        ani.PlayQueued("fatWalk");
        #endregion

        #region Dead
        if (health <= 0f)
        {
            spCollider.enabled = true;
            ParticleSystem newBombFX = Instantiate(bombedFX, gameObject.transform.position, gameObject.transform.rotation);
            newBombFX.gameObject.SetActive(true);
            newBombFX.Play();
            bombedFX.gameObject.SetActive(false);
            bombedFX.Stop();
            StartCoroutine(nameof(CoolDown4HealingAndMoney));
            await Task.Delay(100);
            gameObject.SetActive(false);
        }
        #endregion

        #region Bile
        if (gameObject.activeInHierarchy == true)
        {
            Bile();
        }
        #endregion

    }
    private void Bile()
    {
        float bileDistance = Vector3.Distance(gameObject.transform.position, player.transform.position);
        if (bileDistance < 5f && canBile == true)
        {
            canBile = false;
            GameObject newBile = Instantiate(GameManager.instance.biles[0].gameObject, fLaser.position, fLaser.rotation);
            newBile.gameObject.transform.localScale = new Vector3(bileDistance, bileDistance, bileDistance);
            newBile.gameObject.transform.parent = fLaser.transform;
            newBile.gameObject.SetActive(true);
            bileFX.Play();
            gameManager.GetComponent<GameManager>().DecreaseHealth(15f);
            StartCoroutine(nameof(CoolDown4Bile));
        }
    }
    IEnumerator CoolDown4Bile()
    {
        yield return new WaitForSeconds(6f);
        canBile = true;
    }
    IEnumerator CoolDown4HealingAndMoney()
    {
        yield return new WaitForSeconds(0.1f);
        this.health = 200f;
        gameManager.GetComponent<GameManager>().IncreaseMoney(20);
    }
    public float GetDamage(float damage)
    {
        this.health -= damage;
        return health;
    }
    public float GetHealth(float value)
    {
        value = this.health;
        return this.health;
    }
    private void OnTriggerEnter(Collider other)
    {
        //  zombies get damage by explosion
        if (other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
        {            
            if (other.gameObject.activeInHierarchy == true)
            {
                float distance = Vector3.Distance(gameObject.transform.position, other.gameObject.transform.position);
                if (distance <= 6f)
                {
                    impactParts.Add(other.gameObject);
                    foreach (GameObject zmb in impactParts)
                    {
                        if (zmb.GetComponentInParent<FatZmbScr>() != null && health <= 0f)
                        {
                            zmb.GetComponentInParent<FatZmbScr>().GetDamage(200f);
                        }
                        if (zmb.GetComponentInParent<Flamer>() != null && health <= 0f)
                        {
                            zmb.GetComponentInParent<Flamer>().GetDamage(150f);
                        }
                        if (zmb.GetComponentInParent<Enemy>() != null && health <= 0f)
                        {
                            zmb.GetComponentInParent<Enemy>().GetDamage(100f);
                        }
                    }
                }
            }
        }
        // player get damage by explosion 
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.activeInHierarchy == true)
            {
                float distance = Vector3.Distance(gameObject.transform.position, other.gameObject.transform.position);
                if (distance <= 6f)
                {
                    impactParts.Add(other.gameObject);
                    foreach (GameObject zmb in impactParts)
                    {
                        other.gameObject.GetComponent<Player>().GetDamage(20f);
                    }
                }
            }
        }
        // throws player
        if (other.gameObject.CompareTag("Player"))
        {         
            gameManager.GetComponent<GameManager>().DecreaseHealth(10f);
            ani.Play("FatAttack");
            other.gameObject.GetComponent<CharacterController>().Move(transform.forward * pushSpeed);
            other.gameObject.GetComponent<CharacterController>().Move(transform.up * pushSpeed);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // zombie removes from list of impacts
        if(other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
        {
            if (other.gameObject.activeInHierarchy == true)
            {
                float distance = Vector3.Distance(gameObject.transform.position, other.gameObject.transform.position);
                if (distance >= 6f)
                {
                    impactParts.Remove(other.gameObject);
                }
            }
        }
        // player removes from list of impacts
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.activeInHierarchy == true)
            {
                float distance = Vector3.Distance(gameObject.transform.position, other.gameObject.transform.position);
                if (distance >= 6f)
                {
                    impactParts.Remove(other.gameObject);
                }
            }
        }
    }
}  
