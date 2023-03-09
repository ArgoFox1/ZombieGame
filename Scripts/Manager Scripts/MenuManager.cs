using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    public GameObject videoPlayer;
    public RawImage image;
    private void Start()
    {      
        image.gameObject.SetActive(false);
        videoPlayer.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            image.gameObject.SetActive(false);
            videoPlayer.SetActive(false);         
        }
    }
    public void Play()
    {
        StartCoroutine(nameof(Wait));
        SceneManager.LoadScene(1);
        Destroy(this);
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
    }
    public void Trailer()
    {
        image.gameObject.SetActive(true);
        videoPlayer.SetActive(true);    
    }
    public void Support()
    {
        Application.OpenURL("https://www.youtube.com/channel/UCGPS3t2FfqaljxiaNs9YaSA");
    }
   
}
