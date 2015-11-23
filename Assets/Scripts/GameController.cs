using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {

    public static GameController gc;

    public GameObject Player { get; private set; }

    public GameObject PlayerGameObject;
    public GameObject[] buildings;
    public GameObject StartBuilding;
    List<GameObject> spawnedBuildings;
    GameObject currentBuilding;
    GameObject oldBuilding;
    Building currentBuildingScript;
    AudioSource valve;

    public int startBuildingsAmount;

    public bool gameStarted = false;
    public bool onMainMenu = true;
    bool generating = false;
    bool newBuilding = true;

	void Awake ()
    {
        //Restart();
        gc = gameObject.GetComponent<GameController>();
        valve = GetComponent<AudioSource>();
	}
	
    public void StartMusic()
    {

    }
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Restart();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (onMainMenu)
            return;

        if(spawnedBuildings != null)
            Debug.Log("Current building amount " + spawnedBuildings.Count);

        if(Player == null)
        {
            return;
        }

        if (newBuilding)
        {
            currentBuildingScript = currentBuilding.GetComponent<Building>();
            newBuilding = false;
        }
        else if(currentBuildingScript.lastPoint.position.x < Player.transform.position.x - 12)
            {
                Destroy(currentBuilding, 5);
                spawnedBuildings.RemoveAt(0);
                currentBuilding = spawnedBuildings[0];
                AddNewBuilding();
                newBuilding = true;
            }
            
	}

    void Generate()
    {
        generating = true;

        for(int i = 0; i < startBuildingsAmount; i++)
        {
            Vector3 positionToStartIn;
            int buildingToSpawn = Random.Range(0, buildings.Length);
            if(i == 0)
            {
                var buildingStart = StartBuilding.GetComponent<StartBuilding>();
                positionToStartIn = buildingStart.firstBuildingStartPoint.position;
            }
            else
            {
                positionToStartIn = spawnedBuildings[i - 1].GetComponent<Building>().lastPoint.position;
                positionToStartIn.x += 4f;
            }


            spawnedBuildings.Add(Instantiate(buildings[buildingToSpawn], positionToStartIn, Quaternion.Euler(0, 0, 0)) as GameObject);
        }

        currentBuilding = spawnedBuildings[0];

        generating = false;
    }

    void AddNewBuilding()
    {
        int buildingToSpawn = Random.Range(0, buildings.Length);
        var positionToStartIn = spawnedBuildings[spawnedBuildings.Count - 1].GetComponent<Building>().lastPoint.position;
        positionToStartIn.x += 4f;
        spawnedBuildings.Add(Instantiate(buildings[buildingToSpawn], positionToStartIn, Quaternion.Euler(0, 0, 0)) as GameObject);
    }

    public IEnumerator startGame()
    {
        gameStarted = false;
        if(!valve.isPlaying)
            valve.Play();
        var startBuildingScript = StartBuilding.GetComponent<StartBuilding>();
        var window = startBuildingScript.brokenWindow.GetComponent<Animator>();
        window.Play("GlassNormal", 0);

        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
            GameObject.Destroy(Player);

        var ragdoll = GameObject.FindGameObjectWithTag("Ragdoll");

        if (ragdoll != null)
            GameObject.Destroy(ragdoll);

        newBuilding = true;
        Vector3 PlayerSpawnPoint = new Vector3(startBuildingScript.playerStartPoint.position.x, startBuildingScript.playerStartPoint.position.y, startBuildingScript.playerStartPoint.position.z);
        
        Player = Instantiate(PlayerGameObject, PlayerSpawnPoint, Quaternion.Euler(0, 0, 0)) as GameObject;
        Player.GetComponent<SkateboardController>().StopMoving(true, PlayerSpawnPoint);
        spawnedBuildings = new List<GameObject>();
        Generate();

        yield return new WaitForSeconds(0.2f);
        startBuildingScript.Break();
        yield return new WaitForSeconds(0.2f);
        Player.GetComponent<SkateboardController>().StopMoving(false, PlayerSpawnPoint);
    }

    public void Restart()
    {
        if (spawnedBuildings.Count > 0)
        {
            foreach (var building in spawnedBuildings)
            {
                Destroy(building);
            }
        }
        StartCoroutine(startGame());
    }
}