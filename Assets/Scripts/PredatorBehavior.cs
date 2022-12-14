using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorBehavior : MonoBehaviour
{
    private float speed = 5f;

    private Rigidbody2D rb;

    private Vector2 velocity;

    [SerializeField] private GameObject player;

    private bool squirrelFound = false;

    [SerializeField] private float snakescaling = 1.2f;

    [SerializeField] private float snakepositioning = 0.3f;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int minIndex = 0;
        float mindist = 10000;
        for (int i = 0; i < players.Length; i++)
        {
            if (Mathf.Abs(this.transform.position.x - players[i].transform.position.x) < mindist)
            {
                mindist = Mathf.Abs(this.transform.position.x - players[i].transform.position.x);
                minIndex = i;
            }
        }
        player = players[minIndex];
        switch (this.tag)
        {
            case "Default":
                velocity = new Vector2(speed, 0);
                rb.velocity = velocity;
                break;
            case "Snake":
                velocity = Vector2.zero;
                break;
            case "Hawk":
                velocity = Vector2.zero;
                break;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            int minIndex = 0;
            float mindist = 10000;
            for (int i = 0; i < players.Length; i++)
            {
                if (Mathf.Abs(this.transform.position.x - players[i].transform.position.x) < mindist)
                {
                    mindist = Mathf.Abs(this.transform.position.x - players[i].transform.position.x);
                    minIndex = i;
                }
            }
            player = players[minIndex];
        }
        checkForSquirrel();
        rb.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            velocity.x *= -1;
            if (velocity.x < 0)
            {
                anim.SetBool("FacingRight", false);
            } else
            {
                anim.SetBool("FacingRight", true);
            }
            rb.velocity = velocity;
        }
    }

    private void checkForSquirrel()
    {
        if (transform.position.y >= player.transform.position.y - 1 && !squirrelFound)
        {
            squirrelFound = true;
            Debug.Log("Detected squirrel");
            
            switch (this.tag)
            {
                case "Hawk":
                    velocity += new Vector2(speed, -speed);
                    this.GetComponent<AudioSource>().Play();
                    anim.SetBool("Fly", true);
                    anim.SetBool("FacingRight", true);
                    break;
                case "Snake":
                    rb.mass *= 5;
                    rb.drag *= 5;
                    //transform.localScale = new Vector3(transform.localScale.x * 5, transform.localScale.y, transform.localScale.z);
                    StartCoroutine(ScaleSnake());
                    //transform.position = new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z);
                    break;
            }

        }
    }

    IEnumerator ScaleSnake()
    {
        for (int i = 0; i < 10; i++)
        {
            transform.localScale = new Vector3(transform.localScale.x * snakescaling, transform.localScale.y, transform.localScale.z);
            transform.position = new Vector3(transform.position.x + snakepositioning, transform.position.y, transform.position.z);
            yield return new WaitForSeconds(0.1f);
        }

    }
}
