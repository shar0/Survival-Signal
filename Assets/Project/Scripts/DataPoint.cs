using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataPoint : MonoBehaviour
{
	private int scans = 0;
	public string[] EntryMap;
	public int[] ScanList;
	public int Type = 0;
	public string PointName;
	public string PointDescription;
	public string DiscoveredDescription;
	public int Alive = 0;
	public bool Discovered = false;

	// Initialization before "start"
	void Awake ()
	{
		Init ();
	}

	void Init ()
	{
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public bool Discovery()
	{
		switch(Type)
		{
		case 3:
		case 0:
			Discovered = true;
			return true;
		case 1:
			break;
		case 2:
			if (!Player.Instance.Inventory.Exists (n => n == 4)) {
				Terminal.Instance.AppendLine ("系统受到了陌生舰船的袭击\n");
				GameManager.Instance.GameOver ();
			} else {
				Discovered = true;
				return true;
			}
			break;
		}
		return true;
	}

	public bool Scan()
	{
		bool result = false;
		if (scans < ScanList.Length) {
			int itemId = ScanList [scans];
			if (itemId == 1) {
				GameManager.Instance.UnlockJournal(1);
			} else if (itemId == 2) {
				GameManager.Instance.UnlockJournal(2);
			} else if (itemId == 10) {
				GameManager.Instance.UnlockJournal(3);
			} else if (itemId == 11) {
				Alive = 0;
				GameManager.Instance.UnlockJournal(4);
				GameManager.Instance.GameEnd ();
			} else if (itemId == 12) {
				GameManager.Instance.UnlockJournal(5);
			} else if (itemId > 0) {
				Player.Instance.Inventory.Add (itemId);
			}
			scans++;
			result = true;
		}
		return result;
	}

	void FixedUpdate ()
	{
	}
}
