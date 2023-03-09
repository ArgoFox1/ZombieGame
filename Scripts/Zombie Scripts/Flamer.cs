using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
public class Flamer : MonoBehaviour
{
    public GameObject player;
    public SphereCollider spCollider;
    private Animator animator;
    public ParticleSystem fireFX, bombFX;
    private NavMeshAgent navAgent;
    public GameManager gameManager;
    public Transform fireSpawnPos;
    private bool canFire;
    private bool canBlewUp;
    [Header("PropertiesOfZombie")]
    [SerializeField] private float pushSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float health = 100f;
    private AudioSource hitFolder;
    public AudioClip hitSound;
    public AudioClip fireSound;
    public AudioClip zombieAmbienceSound;
    public AudioClip bombSound;
    public List<GameObject> impactParts = new List<GameObject>();
    private void Start()
    {
        if(impactParts.Count > 0)
        {
            for (int i = 0; i < impactParts.Count; i++)
            {
                impactParts.RemoveAt(i);
            }
        }
        canBlewUp = false;
        spCollider.enabled = false;
        hitFolder = GetComponent<AudioSource>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        fireFX.gameObject.SetActive(false);
        fireFX.Stop();
        canFire = true;
        if (gameObject.activeInHierarchy == true && Time.timeScale != 0)
        {
            hitFolder.PlayOneShot(zombieAmbienceSound);
        }
        health = 200;
    }
    private async void Update()
    {
        #region Sound Settings
        if (Time.timeScale == 0)
        {
            hitFolder.Pause();
        }
        if (hitFolder.isPlaying == false && Time.timeScale != 0)
        {
            hitFolder.UnPause();
        }
        #endregion

        #region Movement        
        gameObject.GetComponent<NavMeshAgent>().destination = player.transform.position;      
        animator.SetBool("walk", true);
        #endregion            
      
        #region Dead
        if (health <= 0f)
        {
            spCollider.enabled = true;
            hitFolder.PlayOneShot(bombSound);
            ParticleSystem newFX = Instantiate(bombFX, gameObject.transform.position, Quaternion.identity);
            newFX.gameObject.SetActive(true);
            newFX.Play();
            StartCoroutine(nameof(CoolDown4Healing));
            await Task.Delay(100);
            gameObject.SetActive(false);
        }
        #endregion

        #region ThrowFire
        if (gameObject.activeInHierarchy == true)
        {
            float fireDistance = Vector3.Distance(gameObject.transform.position, player.transform.position);
            if (fireDistance < 7f && canFire == true && health != 0)
            {
                canFire = false;
                fireFX.gameObject.transform.localScale = new Vector3(fireDistance, fireDistance, 1);
                fireFX.gameObject.SetActive(true);
                fireFX.Play();
                hitFolder.PlayOneShot(fireSound);
                gameManager.GetComponent<GameManager>().DecreaseHealth(20f);
                StartCoroutine(nameof(CoolDown4Fire));
            }
        }
        #endregion      

        if (health <= 100f)
        {
            animator.SetBool("walk", true);
        }      
    }
    IEnumerator CoolDown4Healing()
    {
        yield return new WaitForSeconds(0.1f);
        this.health = 200f;
        gameManager.GetComponent<GameManager>().IncreaseMoney(20);
    }
    IEnumerator CoolDown4Fire()
    {
        yield return new WaitForSeconds(3f);
        canFire = true;
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
        // zombie gets damage by explosion
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
                        if (zmb.GetComponentInParent<Enemy>() != null && health <= 0f)
                        {
                            zmb.GetComponentInParent<Enemy>().GetDamage(100f);
                        }
                        if(zmb.GetComponentInParent<Flamer>() != null && health <= 0f)
                        {
                            zmb.GetComponentInParent<Flamer>().GetDamage(150f);
                        }
                        if(zmb.GetComponentInParent<FatZmbScr>() != null && health <= 0f)
                        {
                            zmb.GetComponentInParent<FatZmbScr>().GetDamage(200f);
                        }
                    }
                }
            }
        }
        if (other.gameObject.CompareTag("Player") && health <=0)
        {
            gameManager.GetComponent<GameManager>().DecreaseHealth(10f);         
            other.gameObject.GetComponent<CharacterController>().Move(transform.forward * pushSpeed / 2);
            other.gameObject.GetComponent<CharacterController>().Move(transform.up * pushSpeed / 2);
        }
        // player gets damage by explosion
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.activeInHierarchy == true)
            {
                float distance = Vector3.Distance(gameObject.transform.position, other.gameObject.transform.position);
                if (distance <= 6f)
                {
                    impactParts.Add(other.gameObject);
                    foreach (GameObject ply in impactParts)
                    {
                        if(ply.gameObject.GetComponent<Player>() != null)
                        {
                            ply.GetComponent<Player>().GetDamage(20f);
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // zombie removes from list of impacts
        if (other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
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
