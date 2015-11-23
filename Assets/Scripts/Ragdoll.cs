using UnityEngine;
using System.Collections;

public class Ragdoll : MonoBehaviour {

    public Rigidbody2D[] force_applier;
    public Vector2 forceToApply;
    Vector2 oldForce;


	void Update()
    {
        if (oldForce == forceToApply)
            return;

        foreach(var rigid in force_applier)
        {
            
            rigid.AddForce(forceToApply + new Vector2(Random.Range(-80, 80), Random.Range(-80, 80)));
        }

        oldForce = forceToApply;
	}
	
}
