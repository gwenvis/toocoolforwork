using UnityEngine;
using System.Collections;

public class StartMenuController : MonoBehaviour {

    public GameObject[] objectsToDisable;
    public GameObject helpObject;
    bool controlsShown = false;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameController.gc.onMainMenu = false;
            StartCoroutine(GameController.gc.startGame());
            GameController.gc.Restart();
            gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.C))
        {
            if (controlsShown)
                return;

            helpObject.SetActive(true);
            for (int i = 0; i < objectsToDisable.Length; i++)
            {
                objectsToDisable[i].SetActive(false);
            }
            controlsShown = true;
        }
        else
        {
            if (!controlsShown)
                return;

            helpObject.SetActive(false);
            for (int i = 0; i < objectsToDisable.Length; i++)
            {
                objectsToDisable[i].SetActive(true);
            }
            controlsShown = false;
        }
        

        
    }
    
}
