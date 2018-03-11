using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class mapGenerator : MonoBehaviour
{
    public static mapGenerator instance;

    [Header("use this option to generate a map without a texture (debug options)")]
    public bool debug = false;
    public bool calcNeigboursOnStartUp = false;
    public bool generateDebugPlayer = false;
    public GameObject debugUnit;

    [Space(2)]
    [Header("map propperties")]
    public bool generateHeigthMap = false;
    [Range(15,150)]
    public float perlinNoiseScale = 20f;
    private Texture2D heigthMapTexture;
    [Space(2)]
    public Texture2D map;

    public ColorToPrefab[] colorMappings;

    // Size of the map in terms of number of hex tiles
    // This is NOT representative of the amount of 
    // world space that we're going to take up.
    // (i.e. our tiles might be more or less than 1 Unity World Unit)
    public int width;
    public int height;

    [Header("insert offset of x and z direction")]
    public float xOffset = 0.882f;
    public float zOffset = 0.764f;

    List<List<Hex>> tiles;
    List<Hex> temp;
    //Dictionary<Hex, GameObject> tilesToGameObjects;
    


    int tilesSpawned = 0;
    // Use this for initialization
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        tiles = new List<List<Hex>>();
        //tilesToGameObjects = new Dictionary<Hex, GameObject>();

        #region debug
        if (debug)
        {
            if (generateHeigthMap)
            {
                perlinNoiseScale = 25f;
                heigthMapTexture = GenerateHeigthMap();
            }
            for (int x = 0; x < width; x++)
            {
                temp = new List<Hex>();

                for (int y = 0; y < height; y++)
                {

                    generatePrefab(x, y, colorMappings[0]);
                    tilesSpawned++;
                }

                tiles.Add(temp);
            }
            if (calcNeigboursOnStartUp)
            {
                foreach (List<Hex> hexList in tiles)
                {
                    foreach (Hex hex in hexList)
                    {
                        Hex[] gos = hex.GetNeighbours();
                        //Debug.Log("Tile at pos " + hex.x + "," + hex.y + " has " + gos.Length + " neighbours");
                    }
                }
            }
            if (generateDebugPlayer)
            {
                GameObject go = Instantiate(debugUnit, GameObject.Find("Hex_0_0").transform.position, Quaternion.identity);
                go.name = "TestUnit";
            }


        }
        #endregion
        else
        {

            width = map.width;
            height = map.height;

            if (generateHeigthMap)
            {
                heigthMapTexture = GenerateHeigthMap();
            }

            for (int x = 0; x < width; x++)
            {
                temp = new List<Hex>();

                for (int y = 0; y < height; y++)
                {
                    
                    generateTile(x, y);

                }

                tiles.Add(temp);
            }
        }
        
        Debug.Log("nr of tiles spawned = " + tilesSpawned);
    }


    public Hex getHexInMap(int x, int y)
    {
        if (x > width || y > height)
        {
            Debug.LogError("Requested hex does not exist within the map");
            return null;
        }
        return tiles[x][y];

    }

    #region heigthmap generation
    private Texture2D GenerateHeigthMap()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalcColorFromPerlinNoise(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private Color CalcColorFromPerlinNoise(int x, int y)
    {
        float xCoord = (float)x / width * perlinNoiseScale;
        float yCoord = (float)y / height * perlinNoiseScale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }
    #endregion


    #region tile generation
    void generateTile(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);

        if (pixelColor.a == 0)
        {
            // The pixel is transparrent. Let's ignore it!
            temp.Add(null);//bad fix so that the pathfinding will work
            return;
        }

        foreach (ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                //Debug.Log("spawning tile at: " + x + "," + y);
                generatePrefab(x, y, colorMapping);
                tilesSpawned++;
            }
        }
    }

    private void generatePrefab(int x, int y, ColorToPrefab colorMapping)
    {
        float xPos = x * xOffset;

        // Are we on an odd row?
        if (y % 2 == 1)
        {
            xPos += xOffset / 2f;
        }

        GameObject hex_go = (GameObject)Instantiate(colorMapping.prefab, new Vector3(xPos, 0, y * zOffset), Quaternion.identity, this.transform);

        // Name the gameobject something sensible.
        hex_go.name = "Hex_" + x + "_" + y;

        // Make sure the hex is aware of its place on the map
        Hex hexClass = hex_go.GetComponent<Hex>();
        hexClass.x = x;
        hexClass.y = y;
        //assign properties to hex
        hexClass.isWalkable = colorMapping.isWalkable;
        hexClass.costToEnter = colorMapping.costToEnterTile;

        temp.Add(hexClass);
        //tilesToGameObjects.Add(hexClass, hex_go);

        if (generateHeigthMap)
        {   
            float heigthScale = heigthMapTexture.GetPixel(x, y).grayscale * colorMapping.heigthScaleMultiplier + 0.5f;
            hex_go.transform.Find("model").localScale = new Vector3(1, 1, heigthScale);
            hex_go.transform.position = new Vector3(hex_go.transform.position.x, hex_go.transform.position.y + heigthScale * 0.057f, hex_go.transform.position.z);
        }
    }
    #endregion


    //pathfinding should be in a seperate script in the future
    #region pathfinding

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        float xPos = x * xOffset;

        // Are we on an odd row?
        if (y % 2 == 1)
        {
            xPos += xOffset / 2f;
        }

        return new Vector3(xPos, 0, y * zOffset);
    }

    public bool UnitCanEnterTile(int x, int y)
    {

        // We could test the unit's walk/hover/fly type against various
        // terrain flags here to see if they are allowed to enter the tile.

        return tiles[x][y].isWalkable;
    }
    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
    {
        

        if (UnitCanEnterTile(targetX, targetY) == false)
            return Mathf.Infinity;

        float cost = tiles[targetX][targetY].costToEnter;

        if (sourceX != targetX && sourceY != targetY)
        {
            // We are moving diagonally!  Fudge the cost for tie-breaking
            // Purely a cosmetic thing!
            cost += 0.001f;
        }

        return cost;

    }

    /// <summary>
    /// path finding works maar het aanspreken van de tiles lijst niet op plek 22,9 staat namelijk niet tile (22,9) maar tile (22,12)
    /// </summary>
    public void GeneratePathTo(Unit unit, int destX, int destY)
    {
        // Clear out our unit's old path.
        unit.currentPath = null;

        if (UnitCanEnterTile(destX, destY) == false)
        {
            // We probably clicked on a mountain or something, so just quit out.
            return;
        }

        Dictionary<Hex, float> dist = new Dictionary<Hex, float>();
        Dictionary<Hex, Hex> prev = new Dictionary<Hex, Hex>();

        // Setup the "Q" -- the list of Hexs we haven't checked yet.
        List<Hex> unvisited = new List<Hex>();

        Hex source = tiles[unit.posX][unit.posY];

        Hex target = tiles[destX][destY];

        dist[source] = 0;
        prev[source] = null;

        // Initialize everything to have INFINITY distance, since
        // we don't know any better right now. Also, it's possible
        // that some Hexs CAN'T be reached from the source,
        // which would make INFINITY a reasonable value
        foreach (List<Hex> hexList in tiles)
        {
            foreach (Hex v in hexList)
            {
                if (v == null)
                {
                    continue;
                }
                if (v != source)
                {
                    dist[v] = Mathf.Infinity;
                    prev[v] = null;
                }

                unvisited.Add(v);
            }
        }

        while (unvisited.Count > 0)
        {
            // "u" is going to be the unvisited Hex with the smallest distance.
            Hex u = null;

            foreach (Hex possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break;  // Exit the while loop!
            }

            unvisited.Remove(u);

            

            foreach (Hex v in u.GetNeighbours())
            {
                if (v != null)
                {
                    //float alt = dist[u] + u.DistanceTo(v);
                    float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }
        }

        // If we get there, the either we found the shortest route
        // to our target, or there is no route at ALL to our target.
        if (prev[target] == null)
        {
            // No route between our target and the source
            Debug.LogError("No path available!");
            return;
        }

        List<Hex> currentPath = new List<Hex>();

        Hex curr = target;

        // Step through the "prev" chain and add it to our path
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }

        // Right now, currentPath describes a route from out target to our source
        // So we need to invert it!

        currentPath.Reverse();

        unit.currentPath = currentPath;

        Debug.Log("Path calculated :)");
    }

    #endregion

}
