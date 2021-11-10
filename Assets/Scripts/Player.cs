using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Player : MonoBehaviour
{
    // animation kýsmýnda bir problem yaþamýþtým
    // bu problem mixamodan indirdigim animasyon olayýnýn çalýþmamasýydý
    // bunu þu videoyu izlerken düzelttim
    //https://www.youtube.com/watch?v=9H0aJhKSlEQ&ab_channel=syntystudios
    // indirdigim dosyaya týkladým (kendisi import setttings aslinda)
    // Rig bolumune tikladim  
    // Animation Type   : Humanoid yaptim
    // simdi dans vakti 


    public static float _score = 0;

    public static bool _alive = false;

    public static bool onGround;

    private Rigidbody rb;

    [SerializeField]
    private float jumpForce;


    private Animator animator;

    public static Vector3 Charmer;

    private GameObject obstacles;

    private GameObject shock;

    private SkinnedMeshRenderer met;

    private GameObject barrierTall;


    [SerializeField]
    private List<AudioClip> auClip;



    private void Awake()
    {
        onGround = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        shock = GameObject.Find("electricity shock");
        obstacles = GameObject.Find("Obstacles");
        rb = GetComponent<Rigidbody>();
        animator = transform.GetChild(1).GetComponent<Animator>();
        refreshCharmer();
        StartCoroutine(createObstacle());
        met = transform.GetChild(1).GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>();

        barrierTall = GameObject.Find("barrierTall");

        _score = PlayerPrefs.GetFloat("maxScore");
        GameObject.Find("Score").GetComponent<Text>().text = ((int)(_score)).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        refreshCharmer();
        ControlKeyBoard();
      

    }
        


    private IEnumerator beShock()
    {
        float timerShock = 0.02f;
        while (true)
        {

           met.material.color = new Color(1.5f, 1.5f, 2,2) - met.material.color;
            
            yield return new WaitForSeconds(timerShock);
            timerShock = timerShock * 1.01f;

        }
        
    }
    private IEnumerator createObstacle()
    {
        /*
         * normalde  bunu kullaniyordum ama gerek yok cunku bir altta bir sey kullandým hemen ornek olarak buraya býrakýyorum
         * yield return new WaitWhile(()=> !_alive );  _alive false oldugu surece bekliyecek alta geçmeyecek mwah
         * 
         * 
        while(!_alive)
        {
            yield return new WaitForSeconds(1);
            //yield return new WaitForEndOfFrame();
        }
        */

        yield return new WaitWhile(()=> !_alive );       
        


        while (_alive)
        {
            createObstacleIn();
            //obstacle olusturuyo
            //rastgele birini alýyor 
            // uzakligini rastgele ayarlayip
            // instantiate ile olusturuyor 
            // tagina obstacle diyor cunku carpisma icin kullaniliyor bu 

            yield return new WaitForSeconds(Random.Range(2f,3f));
        
        }



        // if not _alive 

        // kaydediyor max score u
        scoreRecord();
        

        yield return new WaitForSeconds(2f);
        // 2 saniye sonra reset atiyor 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        

       


        

    }

    private void createObstacleIn()
    {
        GameObject go = obstacles.transform.GetChild(Random.Range(0, obstacles.transform.childCount)).gameObject;

        Vector3 pos_obstacle = Vector3.Scale(transform.position, new Vector3(1, 0, 1)) + transform.forward * Random.Range(16, 22) * (MoveObstacles.speed / 6);


        GameObject tempGo = Instantiate(go, pos_obstacle, Quaternion.identity, GameObject.Find("ObstaclesMoving").transform);

        tempGo.tag = "obstacle";

    }

    private void scoreRecord()
    {

        
        
        
        if (PlayerPrefs.GetFloat("maxScore") < _score)
        {
            PlayerPrefs.SetFloat("maxScore", _score);
        }

    }


    public void StartButton()
    {
        _alive = true;
        GameObject.Find("Start").GetComponent<CanvasGroup>().alpha = 0;
        GameObject.Find("Start").GetComponent<CanvasGroup>().interactable = false;
        _score = 0;

    }
        
    public void ScoreProcess()
    {

        _score += Time.deltaTime * MoveObstacles.speed;
        GameObject.Find("Score").GetComponent<Text>().text = ((int)_score).ToString();



    }


    private void refreshCharmer()
    {
        Charmer = transform.GetChild(0).transform.position;
    }

    private bool beganTouch
    {
        get
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).phase == TouchPhase.Began;
            }
            else return false;

        }
    }


    
    private void ControlKeyBoard()
    {

        if(_alive)
        {
            ScoreProcess();

            
        if (beganTouch) //Input.GetKeyDown(KeyCode.Space) ||
        {
            if(onGround)
            {
                //jump

                rb.AddForce(Vector3.up * jumpForce);

                jumpAnimate();
            }

        }

        
        }



    }




    private void jumpAnimate()
    {
        //jump icin animate speed = 0.43 yaptým iyi oldu 
        animator.SetTrigger("jump");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground") && _alive)
        {
            
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().Play();
            
            GetComponent<AudioSource>().volume = MoveObstacles.speed;
            onGround = true;
            
        }

        if (collision.gameObject.CompareTag("obstacle"))
        {

            
            
            shock.transform.parent = collision.transform;
            shock.transform.position = collision.GetContact(0).point;
            shock.GetComponent<ParticleSystem>().Play();

            StartCoroutine(beShock());

            _alive = false;

            MoveObstacles.speed = 0;

            animator.SetTrigger("death");
            
            rb.AddForce(transform.forward * -jumpForce);
            //GetComponent<AudioSource>().clip = auClip[1];
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(auClip[1]);


        }




    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
            if (_alive)
            {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(auClip[2]);
                 }
        }

    }
}
