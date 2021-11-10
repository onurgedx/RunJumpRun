using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacles : MonoBehaviour
{

    public static float speed;

    private void Awake()
    {
        speed = 6;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnvironmentProcess());
        
        
    }



    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        // GetComponent<Rigidbody>().velocity = transform.forward * speed;
        //Debug.Log(transform.childCount);

        if(transform.childCount>6)
        {
            //Debug.Log(Camera.main.WorldToViewportPoint(transform.TransformPoint(transform.GetChild(2).position)));
            if (Camera.main.WorldToViewportPoint(transform.TransformPoint(transform.GetChild(2).position)).x < -1)
            {
                Destroy(transform.GetChild(2).gameObject);

                speed = speed*(1.1f);
                Debug.Log("speed :" +speed);

            }
            
            

        }

        



    }

    private IEnumerator EnvironmentProcess()
    {
        while(true)
        { 
            foreach( GameObject enviro in    GameObject.FindGameObjectsWithTag("environment"))
            {
                    Vector3 posEnv = Camera.main.WorldToViewportPoint(enviro.transform.GetChild(0).position);
                //Debug.Log(posEnv);
                if(posEnv.x<0)
                    {
                        enviro.transform.position = GameObject.Find("Referans").transform.position;
                    }
             

            }

            
            
                

        yield return new WaitForSeconds(0.1f);
        }
    }



}
