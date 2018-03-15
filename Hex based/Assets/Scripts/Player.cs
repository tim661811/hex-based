using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Ownable[] Ownables = GetComponentsInChildren<Ownable>();

        for (int i = 0; i < Ownables.Length; i++)
        {
            Ownables[i].owner = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
