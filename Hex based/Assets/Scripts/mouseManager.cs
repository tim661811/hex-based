using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class mouseManager : MonoBehaviour {

    public static mouseManager instance;

    GameObject selectedUnit;
    bool unitSelected = false;

    public Hex selectedHex;
    private bool unitPlacementMode;


    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Click");

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (unitPlacementMode)
                    {
                        selectedHex = hit.transform.parent.GetComponent<Hex>();
                        if (selectedHex != null && selectedHex.isWalkable)
                        {
                            unitPlacementMode = false;
                            Debug.Log("hex (" + selectedHex.x + "," + selectedHex.y + ") selected");
                            return;
                        }
                        Debug.LogWarning("compatible hex not found");
                    }
                    if (hit.transform.gameObject.layer == 8) // units layer
                    {
                        selectedUnit = hit.transform.parent.gameObject;
                        unitSelected = true;
                        Debug.Log(selectedUnit.name + " selected");
                    }
                    else if (unitSelected && hit.transform.gameObject.layer == 9) // map layer
                    {
                        Unit u = selectedUnit.GetComponent<Unit>();
                        if (u == null)
                        {
                            Debug.LogError("unit = null");
                        }
                        else
                        {
                            Hex hex = hit.transform.parent.GetComponent<Hex>();
                            u.moveTo(hex.x, hex.y);
                        }
                    }
                    else if (!unitSelected)
                    {
                        Debug.Log("no unit selected");
                    }
                    else
                    {
                        Debug.Log("unexpected behavior!");
                    }

                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("right click");
                selectedUnit = null;
                unitSelected = false;
            }
        }
        //end of update
	}

    public void setUnitPlacementMode()
    {
        unitPlacementMode = true;
    }



}
