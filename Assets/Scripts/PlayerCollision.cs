using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    SkateboardController sc;

    public void Awake()
    {
        sc = GameObject.FindGameObjectWithTag("Player").GetComponent<SkateboardController>();
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (sc == null)
            return;

        if (col.collider.tag == "death")
            sc.Die();
    }
}
