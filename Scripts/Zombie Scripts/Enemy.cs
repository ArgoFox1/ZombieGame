using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    private NavMeshAgent navAgent;
    public GameManager gameManager;
    private Animation ani;
    public GameObject player;       
    private bool canDamage;
    [Header("PropertiesOfZombie")]
    [SerializeField] private float speed;
    [SerializeField] private float health = 100f;
    private AudioSource hitFolder;
    public AudioClip hitSound;
    public AudioClip zombieAmbienceSound;
    private void Start()
    {
        canDamage = true;
        navAgent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animation>();
        hitFolder = GetComponent<AudioSource>();
        if(gameObject.activeInHierarchy == true && Time.timeScale != 0)
        {
            hitFolder.PlayOneShot(zombieAmbienceSound);
        }
        navAgent.SetDestination(player.transform.position);
        health = 100f;
    }
    private void Update()
    {
        #region Sound Settings
        if (hitFolder.isPlaying == false && Time.timeScale !=0)
        {
            hitFolder.PlayOneShot(zombieAmbienceSound);
        }
        if(hitFolder.isPlaying == true && Time.timeScale == 0)
        {
            hitFolder.Stop();
        }
        #endregion

        #region Movement       
        gameObject.GetComponent<NavMeshAgent>().destination = player.transform.position;        
        ani.PlayQueued("Right");
        #endregion      

        #region Dead
        if(health <= 0f)
        {
            gameObject.SetActive(false);
            gameManager.GetComponent<GameManager>().IncreaseMoney(10);
            health = 100f;          
        }
        #endregion      
    }
    public float GetDamage(float damage)
    {
        this.health -= damage;
        return health;
    }
    public float GetHealth()
    {
        return this.health;
    }
    IEnumerator CoolDown4Damage()
    {
        yield return new WaitForSeconds(1f);
        canDamage = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(canDamage == true)
            {
                canDamage = false;
                ani.Play("Attack");
                gameManager.GetComponent<GameManager>().DecreaseHealth(5f);
                StartCoroutine(nameof(CoolDown4Damage));
            }
        }
    }
}
