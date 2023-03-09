using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Bomb : MonoBehaviour
{
    public float random;
    private string sceneName;
    private List<GameObject> disabledRockets = new List<GameObject>();
    public LayerMask enemyMask;
    public ParticleSystem impactFX;
    private AudioSource folder;
    private AudioClip impactClip;
    public Transform checker;
    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        impactFX.gameObject.SetActive(false);
        impactFX.Stop();
        folder = GetComponent<AudioSource>();
        impactClip = folder.clip;
    }
    private void Update()
    {
        if(sceneName == "Level1")
        {
            JetBomb();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Head") || other.gameObject.CompareTag("Body"))
        {
            if (Time.timeScale != 0)
            {
                FXMaker(gameObject.transform.position);
                GiveDamage(other.gameObject);
                Destroy(gameObject.GetComponent<Rigidbody>());
            }
        }     
        if(other.gameObject.CompareTag("Floor"))
        {
            if (Time.timeScale != 0)
            {
                FXMaker(gameObject.transform.position);
                Destroy(gameObject.GetComponent<Rigidbody>());
            }
        }
    }   
    private void JetBomb()
    {
        RaycastHit hit;
        if (Physics.Raycast(checker.position, checker.transform.forward,out hit, 900f, enemyMask))
        {
            if(hit.collider.gameObject.tag != "DeadZone")
            {
                Debug.DrawRay(checker.position, checker.transform.forward * hit.distance, Color.red);
                gameObject.transform.parent = null;
                gameObject.transform.position += new Vector3(0,-1,1)  *  Time.deltaTime * 50;
                if (this.gameObject.GetComponent<Rigidbody>() == null)
                {
                    this.gameObject.AddComponent<Rigidbody>();
                }
            }          
        }
    }
    private void GiveDamage(GameObject g)
    {
        if (g.gameObject.GetComponentInParent<Enemy>() != null)
        {
            g.gameObject.GetComponentInParent<Enemy>().GetDamage(20);
        }
        if (g.gameObject.GetComponentInParent<Enemy>() != null)
        {
            g.gameObject.GetComponentInParent<FatZmbScr>().GetDamage(20);
        }
        if (g.gameObject.GetComponentInParent<Flamer>() != null)
        {
            g.gameObject.GetComponentInParent<Flamer>().GetDamage(20);
        }
    }
    private async void FXMaker(Vector3 pos)
    {
        folder.PlayOneShot(impactClip);
        impactFX.gameObject.transform.parent = null;
        impactFX.gameObject.transform.position = pos;
        impactFX.gameObject.SetActive(true);
        impactFX.Play();
        disabledRockets.Add(this.gameObject);
        Destroy(this.gameObject.GetComponent<MeshRenderer>());
        await Task.Delay(3000);
        this.gameObject.SetActive(false);
    }
}
