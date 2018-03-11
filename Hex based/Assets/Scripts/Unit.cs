using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    

    public List<Hex> currentPath = null;

    public int posX, posY;

    Vector3 destination;

	// Use this for initialization
	void Start () {
        destination = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //if (Vector3.Distance(transform.position,destination)>0.05)
        //{
        //    transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * 2);
        //}
        //else
        //{
        //    if (transform.position != destination)
        //    {
        //        transform.position = destination;
        //    }
        //}
	}

    private void Update()
    {
        if (currentPath != null)
        {

            int currNode = 0;

            while (currNode < currentPath.Count - 1)
            {

                Vector3 start = mapGenerator.instance.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
                    new Vector3(0, 1f, 0);
                Vector3 end = mapGenerator.instance.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
                    new Vector3(0, 1f, 0);

                Debug.DrawLine(start, end, Color.red);

                currNode++;
            }

        }
    }

    public void moveTo(int x, int y)
    {
        //destination = dest;
        //if (map = null)
        //{
        //    map = ;
        //}
        mapGenerator.instance.GeneratePathTo(this, x, y);
    }
}
