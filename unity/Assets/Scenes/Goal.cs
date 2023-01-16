using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool IsPlayer1Goal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if(IsPlayer1Goal) 
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().Player2Scored();
            }
            else 
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().Player1Scored();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
