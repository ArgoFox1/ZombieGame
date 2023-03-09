using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    [Header("Player s Properties")]

    #region Keys
    public KeyCode flashKey = KeyCode.F; 
    public KeyCode ak47Key = KeyCode.Alpha1;
    public KeyCode pistolKey = KeyCode.Alpha2;
    public KeyCode m16Key = KeyCode.Alpha3;
    public KeyCode ak74Key = KeyCode.Alpha4;
    public KeyCode m4A4Key = KeyCode.Alpha5;
    public KeyCode berettaKey = KeyCode.Alpha6;
    public KeyCode shoutGunKey = KeyCode.Alpha7;
    #endregion
  
    #region Guns And Player Variables
    public GameObject shoutGun;
    public GameObject berretta;
    public GameObject m4A4;
    public GameObject ak74;
    public GameObject ak47;
    public GameObject Arm;
    public GameObject Arm2;
    public GameObject pistol;
    public Transform ak;
    public GameObject m16;
    private List<GameObject> guns = new List<GameObject>();
    public Transform arm;
    public Transform arm2;
    #endregion

    #region Static Variables
    private string Name;
    private Vignette vignette;
    public PostProcessVolume pVolume;
    [SerializeField] private float bulletSpeed;
    private int count;
    public Image buyMenu;
    public AudioSource playFolder;
    public GameObject cam;
    public GameManager gameManager;
    public float health = 100f;
    #endregion   

    #region Movement Variables
    public float speed;
    public float fallingSpeed;
    public float  timer, currentTimer;
    private bool canPlay;
    private CharacterController cc;
    private Vector3 moveDir;
    #endregion   

    [Header("Gravity Checker")]

    #region Gravity Variables
    private Vector3 velocity;
    private float gravity = -9.83f;
    private bool isGrounded;
    public Transform Checker;
    public LayerMask floorMask;
    #endregion     

    private enum GunKeybinds
    {
        Ak47,
        Pistol,
        M16,
        Arm,
        AK74,
        M4A4,
        ShoutGun,
        Beretta,
        Arm2
    }     
    private GunKeybinds keybinds;
    private void Start()
    {
        Name = SceneManager.GetActiveScene().name;
        if(Name == "Level2")
        {
            count = 3;
        }
        if (Name == "Level1")
        {
            count = 0;
        }
        cc = gameObject.GetComponent<CharacterController>();
        pVolume.profile.TryGetSettings(out vignette);
    }
    private void FixedUpdate()
    {            

        #region Gravity 
        isGrounded = Physics.CheckSphere(Checker.position, 0.3f,floorMask);
        if(isGrounded && velocity.y <= 0f)
        {
            velocity.y = 0f;
        }
        velocity.y += gravity * Time.fixedDeltaTime;
        cc.Move(velocity * Time.fixedDeltaTime * fallingSpeed);      
        #endregion     

    }
    private async void Update()
    {
        Name = SceneManager.GetActiveScene().name;
        health = gameManager.GetHealth(health);

        #region ScreenFx
        if(gameObject.activeInHierarchy == true)
        {
            health = gameManager.GetHealth(health);
            if (health < 50f)
            {
                vignette.color.value = Color.red;
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.2f, Time.time);
            }
            if (health < 25f)
            {
                vignette.color.value = Color.red;
                vignette.intensity.value += Mathf.Lerp(vignette.intensity.value, 0.4f, Time.time);
            }
        }      
        #endregion

        #region Movement
        float ver = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float hor = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        moveDir = ver * transform.forward + hor * transform.right;
        cc.Move(moveDir);
        if (ver != 0 || hor != 0)
            canPlay = true;
        else
            canPlay = false;
        if (canPlay == true)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                playFolder.Play();
                timer += currentTimer;
            }
        }
        else
        {   
            canPlay = false;
            playFolder.Stop();
        }
        #endregion
      
        #region Keybinds       
  
        #region GunsKeybinds     
        guns = new List<GameObject> { ak47, pistol, m16, ak74, m4A4, berretta, shoutGun };      
        switch (keybinds)
        {
            case GunKeybinds.Ak47:
                ActiveGun();
                break;
            case GunKeybinds.Pistol:
                ActiveGun();
                break;
            case GunKeybinds.M16:
                ActiveGun();
                break;
            case GunKeybinds.AK74:
                ActiveGun();
                break;
            case GunKeybinds.M4A4:
                ActiveGun();
                break;
            case GunKeybinds.Beretta:
                ActiveGun();
                break;
            case GunKeybinds.ShoutGun:
                ActiveGun();
                break;
        }   
        if(Name == "Level1")
        {
            if (Input.GetKeyDown(ak47Key))
            {
                await Task.Delay(3000);
                keybinds = GunKeybinds.Ak47;
                count = 0;
            }
            if (Input.GetKeyDown(pistolKey))
            {
                await Task.Delay(3000);
                keybinds = GunKeybinds.Pistol;
                count = 1;
            }
            if (Input.GetKeyDown(m16Key))
            {
                await Task.Delay(3000);
                keybinds = GunKeybinds.M16;
                count = 2;
            }
        }   
        if(Name == "Level2")
        {
            if (Input.GetKeyDown(ak74Key))
            {
                await Task.Delay(3000);
                keybinds = GunKeybinds.AK74;
                count = 3;
            }
            if (Input.GetKeyDown(m4A4Key))
            {
                await Task.Delay(3000);
                keybinds = GunKeybinds.M4A4;
                count = 4;
            }
            if (Input.GetKeyDown(berettaKey))
            {
                await Task.Delay(3000);
                keybinds = GunKeybinds.Beretta;
                count = 5;
            }
            if (Input.GetKeyDown(shoutGunKey))
            {
                await Task.Delay(3000);
                keybinds = GunKeybinds.ShoutGun;
                count = 6;
            }
        }
        #endregion

        #endregion

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DeadZone"))
        {
            gameManager.isDead = true;
        }
    }
    private void ActiveGun()
    {
        Name = SceneManager.GetActiveScene().name;
        List<GameObject> disabledObjects = new List<GameObject>();
        if (this.gameObject.activeInHierarchy == true && Time.timeScale != 0)
        {
            if(Name == "Level1")
            {
                if (keybinds == GunKeybinds.Ak47 || count == 0)
                {
                    guns[count].SetActive(true);
                    Arm.SetActive(true);
                    disabledObjects = new List<GameObject> { Arm2, m16, pistol, ak74, m4A4, berretta, shoutGun };
                    for (int i = 0; i < disabledObjects.Count; i++)
                    {
                        disabledObjects[i].SetActive(false);
                    }
                }
                if (keybinds == GunKeybinds.Pistol || count == 1)
                {
                    guns[count].SetActive(true);
                    Arm2.SetActive(true);
                    disabledObjects = new List<GameObject> { Arm, m16, ak47, ak74, m4A4, berretta, shoutGun };
                    for (int i = 0; i < disabledObjects.Count; i++)
                    {
                        disabledObjects[i].SetActive(false);
                    }
                }
                if (keybinds == GunKeybinds.M16 || count == 2)
                {
                    guns[count].SetActive(true);
                    Arm.SetActive(true);
                    disabledObjects = new List<GameObject> { Arm2, ak47, pistol, ak74, m4A4, berretta, shoutGun };
                    for (int i = 0; i < disabledObjects.Count; i++)
                    {
                        disabledObjects[i].SetActive(false);
                    }
                }
            }     
            if(Name == "Level2")
            {
                if (keybinds == GunKeybinds.AK74 || count == 3)
                {
                    guns[count].SetActive(true);
                    Arm.SetActive(true);
                    disabledObjects = new List<GameObject> { Arm2, ak47, pistol, m4A4, berretta, shoutGun, m16 };
                    for (int i = 0; i < disabledObjects.Count; i++)
                    {
                        disabledObjects[i].SetActive(false);
                    }
                }
                if (keybinds == GunKeybinds.M4A4 || count == 4)
                {
                    guns[count].SetActive(true);
                    Arm.SetActive(true);
                    disabledObjects = new List<GameObject> { Arm2, ak47, pistol, ak74, berretta, shoutGun, m16 };
                    for (int i = 0; i < disabledObjects.Count; i++)
                    {
                        disabledObjects[i].SetActive(false);
                    }
                }
                if (keybinds == GunKeybinds.Beretta || count == 5)
                {
                    guns[count].SetActive(true);
                    Arm2.SetActive(true);
                    disabledObjects = new List<GameObject> { Arm, ak47, pistol, m4A4, shoutGun, ak74, m16 };
                    for (int i = 0; i < disabledObjects.Count; i++)
                    {
                        disabledObjects[i].SetActive(false);
                    }
                }
                if (keybinds == GunKeybinds.ShoutGun || count == 6)
                {
                    guns[count].SetActive(true);
                    Arm.SetActive(true);
                    disabledObjects = new List<GameObject> { Arm2, ak47, pistol, m4A4, berretta, m16, ak74 };
                    for (int i = 0; i < disabledObjects.Count; i++)
                    {
                        disabledObjects[i].SetActive(false);
                    }
                }
            }           
        }
    }    
    public float GetDamage(float damage)
    {
        this.health -= damage;
        return health;
    }  
}
#if UNITY_EDITOR
[CustomEditor(typeof(Player)), InitializeOnLoadAttribute]
public class PlayerED : Editor
{
    Player pl;
    SerializedObject SerFPC;
    private void OnEnable()
    {
        pl = (Player)target;
        SerFPC = new SerializedObject(pl);
    }
    public override void OnInspectorGUI()
    {
        SerFPC.Update();

        EditorGUILayout.Space();
        GUILayout.Label("First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("This is The Property Of The Emirhan Metin", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #region Keys Setup      
        GUILayout.Label("Key Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        pl.ak47Key = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Ak47 Key", "Determines what key is used to handle gun."), pl.ak47Key);
        pl.pistolKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Pistol Key", "Determines what key is used to handle gun."), pl.pistolKey);
        pl.m16Key = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("M16 Key", "Determines what key is used to handle gun."), pl.m16Key);
        pl.ak74Key = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Ak74 Key", "Determines what key is used to handle gun."), pl.ak74Key);
        pl.m4A4Key = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("M4A4 Key", "Determines what key is used to handle gun."), pl.m4A4Key);
        pl.berettaKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Beratta Key", "Determines what key is used to handle gun."), pl.berettaKey);
        pl.shoutGunKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("ShoutGun Key", "Determines what key is used to handle gun."), pl.shoutGunKey);
        EditorGUILayout.Space();
        #endregion

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Static Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        #region Static Setup       
        pl.cam = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Cam", "Cam should be attach here."), pl.cam, typeof(GameObject), true);
        pl.pVolume = (PostProcessVolume)EditorGUILayout.ObjectField(new GUIContent("PostProcessVolume","For Graphics."),pl.pVolume, typeof(PostProcessVolume), true);
        pl.buyMenu = (Image)EditorGUILayout.ObjectField(new GUIContent("Buy Menu","Attach Here"),pl.buyMenu, typeof(Image), true);
        pl.playFolder = (AudioSource)EditorGUILayout.ObjectField(new GUIContent("Play Sound Folder", "Attach Here"), pl.playFolder, typeof(AudioSource), true);
        pl.gameManager = (GameManager)EditorGUILayout.ObjectField(new GUIContent("GameManager", "Attach Here"), pl.gameManager, typeof(GameManager), true);
        pl.health = EditorGUILayout.Slider(new GUIContent("Health", "Yours Total Health"), pl.health, 1, 100);
        #endregion

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Gravity Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        #region Gravity Setup
        pl.Checker = (Transform)EditorGUILayout.ObjectField(new GUIContent("Checker", "Attach Here"), pl.Checker, typeof(Transform), true);
        pl.floorMask = (LayerMask)EditorGUILayout.LayerField(new GUIContent("Floor Mask", "Attach Here"), pl.floorMask);
        #endregion

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        #region MovementSetup
        pl.timer = (float)EditorGUILayout.FloatField(new GUIContent("Timer", "Attach Here"), pl.timer);
        pl.currentTimer = (float)EditorGUILayout.FloatField(new GUIContent("CurrentTimer", "Attach Here"), pl.currentTimer);
        pl.speed = (float)EditorGUILayout.FloatField(new GUIContent("Speed", "Attach Here"), pl.speed);
        pl.fallingSpeed = (float)EditorGUILayout.FloatField(new GUIContent("FallingSpeed", "Attach Here"), pl.fallingSpeed);
        #endregion

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Guns Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 }, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        #region GunsSetup
        pl.ak47 = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Ak47", "Attach Here"), pl.ak47, typeof(GameObject), true);
        pl.ak74 = (GameObject)EditorGUILayout.ObjectField(new GUIContent("AK74", "Attach Here"), pl.ak74, typeof(GameObject), true);
        pl.m4A4 = (GameObject)EditorGUILayout.ObjectField(new GUIContent("M4A4","Attach Here"),pl.m4A4, typeof(GameObject), true);
        pl.berretta = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Beretta","Attach Here"),pl.berretta,typeof(GameObject), true);
        pl.shoutGun = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Shoutgun","Attach Here"),pl.shoutGun,typeof(GameObject), true);
        pl.m16 = (GameObject)EditorGUILayout.ObjectField(new GUIContent("M16","Attach Here"),pl.m16, typeof(GameObject), true); 
        pl.pistol = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Pistol","Attach Here"),pl.pistol, typeof(GameObject), true);
        pl.arm = (Transform)EditorGUILayout.ObjectField(new GUIContent("Arm", "Attach Here"), pl.arm, typeof(Transform), true);
        pl.arm2 = (Transform)EditorGUILayout.ObjectField(new GUIContent("Arm2", "Attach Here"), pl.arm2, typeof(Transform), true);
        pl.Arm = (GameObject)EditorGUILayout.ObjectField(new GUIContent("TransFormArm", "Attach Here"), pl.Arm, typeof(GameObject), true);
        pl.Arm2 = (GameObject)EditorGUILayout.ObjectField(new GUIContent("TransFormArm2", "Attach Here"), pl.Arm2, typeof(GameObject), true);
        pl.ak = (Transform)EditorGUILayout.ObjectField(new GUIContent("Ak47Transform", "Attach Here"), pl.ak, typeof(Transform), true);
        #endregion         
    }
}
#endif
