using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField] GameManager gm;
    //private void Awake()
    //{
        
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            // player collided, load end game
            gm = FindObjectOfType<GameManager>();
            gm.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnEnable()
    {
        gm = FindObjectOfType<GameManager>();

        //Debug.Log("Good morning Vietnam!");
    }
}
