using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : Ownable
{
    
    public Unit Prototype;

    public int x;
    public int y;

    public List<ResourceCost> Costs;

    public void SpawnUnit()
    {
        if (mouseManager.instance.selectedHex == null)
        {
            Debug.LogError("Select a hex with the 'Hex selection mode' button!");
            return;
        }
        x = mouseManager.instance.selectedHex.x;
        y = mouseManager.instance.selectedHex.y;

        bool canAfford = true;
        for (int i = 0; i < Costs.Count; i++)
        {
            if (!Costs[i].CanAfford())
            {
                canAfford = false;
            }
        }

        if (canAfford)
        {
            for (int i = 0; i < Costs.Count; i++)
            {
                Costs[i].Pay();
            }
            Unit newUnit = Instantiate(Prototype);
            Hex hex = mapGenerator.instance.getHexInMap(x, y);
            newUnit.posX = x;
            newUnit.posY = y;
            newUnit.owner = owner;

            newUnit.transform.SetParent(hex.transform, false);
        }
        else
        {
            Debug.Log("You don't have enough recources!");
        }

        mouseManager.instance.selectedHex = null;


    }

    [System.Serializable]
    public class ResourceCost
    {
        public Resource Resource;
        public int Cost;

        public bool CanAfford()
        {
            return Resource.CanAfford(Cost);
        }

        public void Pay()
        {
            Resource.SubtractAmount(Cost);
        }
    }
}
