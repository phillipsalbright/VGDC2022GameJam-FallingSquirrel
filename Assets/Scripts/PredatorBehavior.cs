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
    private GameObject hitbox;
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
                hitbox = this.GetComponentInChildren<BoxCollider2D>().gameObject;
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
        switch (this.tag)
        {
            case "Hawk":
                if (transform.position.y >= player.transform.position.y - 1 && !squirrelFound)
                {
                    squirrelFound = true;
                    Debug.Log("Detected squirrel");
                    velocity += new Vector2(speed, -speed);
                    this.GetComponent<AudioSource>().Play();
                    anim.SetBool("Fly", true);
                    anim.SetBool("FacingRight", true);
                    break;

                }
                break;
            case "Snake":
                if (transform.position.y >= player.transform.position.y - 3.5f && !squirrelFound)
                {
                    squirrelFound = true;
                    Debug.Log("Detected squirrel");
                    rb.mass *= 5;
                    rb.drag *= 5;
                    //transform.localScale = new Vector3(transform.localScale.x * 5, transform.localScale.y, transform.localScale.z);
                    StartCoroutine(ScaleSnake());
                    this.GetComponent<Animator>().Play("SnakeAttack");
                    //transform.position = new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z);

                }
                break;
        }
    }

    IEnumerator ScaleSnake()
    {
        this.GetComponent<AudioSource>().Play();
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Frogs");
            hitbox.transform.localScale = new Vector3(hitbox.transform.localScale.x * snakescaling, hitbox.transform.localScale.y, hitbox.transform.localScale.z);
            hitbox.transform.position = new Vector3(hitbox.transform.position.x + snakepositioning, hitbox.transform.position.y, hitbox.transform.position.z);
            yield return new WaitForSeconds(0.06f);
        }
        yield return new WaitForSeconds(.12f);
        this.GetComponent<Animator>().Play("SnakeIdle");
        hitbox.transform.localScale = new Vector3(hitbox.transform.localScale.x / (Mathf.Pow(snakescaling, 10)), hitbox.transform.localScale.y, hitbox.transform.localScale.z);
        hitbox.transform.position = new Vector3(hitbox.transform.position.x - snakepositioning * 10, hitbox.transform.position.y, hitbox.transform.position.z);
        yield return new WaitForSeconds(2.3f);
        this.GetComponent<AudioSource>().volume = 0;
        squirrelFound = false;

    }
}
