using UnityEngine;
using System.Collections;

public class Debris : MonoBehaviour {

    public Sprite[] DebrisTextures;

	void Start ()
    {
        float RandomY = Random.Range(-10, 10);
        float RandomX = Random.Range(5, 20);

        GetComponent<Rigidbody2D>().velocity = new Vector2(RandomX, RandomY);
        GetComponent<SpriteRenderer>().sprite = DebrisTextures[Random.Range(0, DebrisTextures.Length)];
        Destroy(gameObject, 5);
	}
	
}
