using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    List<Player> Players = new List<Player>();

	// Use this for initialization
	void Start () {
        Players.AddRange(GetComponentsInChildren<Player>());
        for (int i = 1; i < Players.Count; i++)
        {
            Players[i].gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    int counter=0;
    public void NextPlayer()
    {
        Players[counter % Players.Count].gameObject.SetActive(false);
        counter++;
        Players[counter % Players.Count].gameObject.SetActive(true);

        Debug.Log(counter / Players.Count);
    }
}
