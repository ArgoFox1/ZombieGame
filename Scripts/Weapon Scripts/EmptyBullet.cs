using UnityEngine;
public class EmptyBullet : MonoBehaviour
{
    public AudioSource fallFolder;
    public AudioClip fallClip;    
    private void OnTriggerEnter(Collider other)
    {      
       if (other.gameObject.CompareTag("Floor"))
       {
            fallFolder.Play();
       }             
    }
}
