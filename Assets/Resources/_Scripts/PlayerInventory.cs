using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Item/count pairs
    public Dictionary<string, int> inventory;

    void Start()
    {
        inventory = new Dictionary<string, int>();
        inventory.Add("Hull", 2);
        inventory.Add("Laser X02", 1);
        inventory.Add("Quazite", 45);
    }

    public void AddToInventory(string item, int quantity)
    {
        inventory.Add(item, quantity);
    }

}

