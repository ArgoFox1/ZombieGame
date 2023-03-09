using UnityEngine;

public class FPS : MonoBehaviour
{
    [Header("FPS Properties")]
    [SerializeField] private float sensivity;
    private float rotX;
    public GameObject player;  
    private void FixedUpdate()
    {

        #region FPS
        Look();
        #endregion

    }
    public void Look()
    {
        float x = Input.GetAxisRaw("Mouse X") * sensivity * Time.fixedDeltaTime;
        float y = Input.GetAxisRaw("Mouse Y") * sensivity * Time.fixedDeltaTime;
        float z = gameObject.transform.localRotation.z;
        z += Mathf.Sin(Time.time * 3f) * 0.5f;
        rotX -= y * sensivity;
        rotX = Mathf.Clamp(rotX, -70, 70);
        gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, Quaternion.Euler(rotX, 0, 0), Time.fixedDeltaTime * 100f);
        player.transform.Rotate(Vector3.up * x * sensivity);
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, Quaternion.Euler(rotX, 0, z), Time.fixedDeltaTime * 100);
        }
    }
}
