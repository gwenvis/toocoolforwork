 using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    [SerializeField]
    public Transform target;

    [SerializeField]
    private Vector3 offset;

    public void Start()
    {
        offset.z = transform.position.z;
    }

    public void LateUpdate()
    {
        if(target == null)
        {
            var wantedTarget = GameObject.FindGameObjectWithTag("Player");
            if(wantedTarget != null)
                target = wantedTarget.transform;
        }
        else
            transform.position = target.position + offset;
    }
}
