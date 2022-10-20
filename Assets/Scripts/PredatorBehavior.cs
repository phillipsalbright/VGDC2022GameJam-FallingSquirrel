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

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
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
        checkForSquirrel();
        rb.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            velocity.x *= -1;
            rb.velocity = velocity;
        }
    }

    private void checkForSquirrel()
    {
        if (transform.position.y >= player.transform.position.y)
        {
            squirrelFound = true;
            Debug.Log("Detected squirrel");
            switch (this.tag)
            {
                case "Hawk":
                    velocity += new Vector2(speed, -speed);
                    break;
                case "Snake":
                    velocity += new Vector2(speed, 0);
                    break;
            }

        }
    }
}
