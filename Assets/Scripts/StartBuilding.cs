using UnityEngine;
using System.Collections;

public class StartBuilding : MonoBehaviour {

    public Transform playerStartPoint;
    public GameObject brokenWindow;
    public Transform firstBuildingStartPoint;

    public GameObject Debris;
    public int DebriAmount;

    public AudioClip GlassBreak;
    AudioSource valve;
    

    public void Start()
    {
        valve = GetComponent<AudioSource>();
    }

    public void Break()
    {
        for(int i = 0; i < DebriAmount; i++)
        {
            Instantiate(Debris, playerStartPoint.position, playerStartPoint.rotation);
        }
        var window = brokenWindow.GetComponent<Animator>();
        window.Play("GlassBreak", -1, 0);
        valve.PlayOneShot(GlassBreak);
    }
}
