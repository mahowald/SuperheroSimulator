using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppearanceItem 
{
	public enum Slot
	{
		Head,
		Hair,
		Suit,
		Belt,
		Boots,
		Gloves,
		Cape
	};

	public GameObject prefab;

	public HashSet<Slot> slots;

	public AppearanceItem()
	{
		slots = new HashSet<Slot>();
	}

	public AppearanceItem(GameObject pPrefab, HashSet<Slot> pSlots)
	{
		pPrefab = prefab;
		pSlots = slots;
	}
}
