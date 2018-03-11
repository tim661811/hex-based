using UnityEngine;
using System.Collections;

public class Hex : MonoBehaviour {

	// Our coordinates in the map array
	public int x;
	public int y;
    public Hex[] neighbours;
    public bool isWalkable = true;
    public float costToEnter = Mathf.Infinity;

   

    public Hex[] GetNeighbours() {

        if (neighbours == null || neighbours.Length == 0)
        {
            Hex hexL = mapGenerator.instance.getHexInMap(x - 1, y);
            Hex hexR = mapGenerator.instance.getHexInMap(x + 1, y);

            Hex hexTL, hexTR, hexBL, hexBR;

            if (y % 2 == 1)//we are on an odd row
            {
                hexTL = mapGenerator.instance.getHexInMap(x, y + 1);
                hexTR = mapGenerator.instance.getHexInMap(x + 1, y + 1);
                hexBR = mapGenerator.instance.getHexInMap(x + 1, y - 1);
                hexBL = mapGenerator.instance.getHexInMap(x, y - 1);
            }
            else
            {
                hexTL = mapGenerator.instance.getHexInMap(x - 1, y + 1);
                hexTR = mapGenerator.instance.getHexInMap(x, y + 1);
                hexBR = mapGenerator.instance.getHexInMap(x, y - 1);
                hexBL = mapGenerator.instance.getHexInMap(x - 1, y - 1);
            }

            neighbours = new Hex[] { hexL, hexTL, hexTR, hexR, hexBR, hexBL };

            return neighbours;
        }
        else
        {
            return neighbours;
        }
		
	}

}
