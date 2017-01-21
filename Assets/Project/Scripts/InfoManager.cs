using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{

	private static InfoManager instance;

	public static InfoManager Instance {
		get {
			return instance;
		}
	}

	public Text StatusLabel;
	public GameObject InfoPanel;
	public GameObject LifeStatusPanel;
	public Text LifeStatusText;
	public GameObject Renderer;
	public GameObject LifeStatusRenderer;
	public GameObject InventoryPanel;
	public Text InventoryText;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		InventoryPanel.SetActive (false);
		LifeStatusPanel.SetActive (false);
		LifeStatusRenderer.SetActive (false);
		InfoPanel.SetActive (false);
		Renderer.SetActive (false);
	}

	void Update ()
	{
		switch (GameManager.Instance.CurrerentProgram) {
		case SystemProgram.Boot:
			break;
		case SystemProgram.Login:
			break;
		case SystemProgram.End:
		case SystemProgram.Over:
			StatusLabel.text = "No Signal";
			LifeStatusText.text = "No Signal";
			InventoryText.text = "No Signal";
			LifeStatusRenderer.SetActive (false);
			Renderer.SetActive (false);
			break;
		case SystemProgram.Welcome:
			InfoPanel.SetActive (true);
			InventoryPanel.SetActive (true);
			LifeStatusPanel.SetActive (true);
			StatusLabel.text = "No Signal";
			LifeStatusText.text = "No Signal";
			InventoryText.text = "No Signal";
			break;
		default:
			Renderer.SetActive (true);
			LifeStatusRenderer.SetActive (true);
			StatusLabel.text = statusText;
			LifeStatusText.text = lifeStatus;
			InventoryText.text = inventoryStatus;
			break;
		}

		GraphManager.SineLevel = (Player.Instance.CurrentPoint.Alive * 0.2f) + 0.05f; 
	}

	private string inventoryStatus {
		get {
			if (Player.Instance.Inventory.Count < 1) {
				return "暂无货物信息";
			} else {
				string itemStr = "";
				foreach (int itemId in Player.Instance.Inventory) {
					switch(itemId) {
					case 3:
						itemStr += "海盗舰队通信模块\n";
						break;
					case 4:
						itemStr += "海盗敌我识别装置\n";
						break;
					}
				}
				return itemStr;
			}
		}
	}

	private string lifeStatus {
		get {
			if (Player.Instance.CurrentPoint.Alive > 0) {
				return "生命迹象 强";
			} else {
				return "生命迹象 弱";
			}
		}
	}

	private string statusText {
		get {
			if (Player.Instance.Target != null) {
				return string.Format (
					"当前坐标 X：{0} Y：{1} Z：{2}\n" +
					"正在前往 {3}", 
					(int)Player.Instance.gameObject.transform.position.x,
					(int)Player.Instance.gameObject.transform.position.y,
					(int)Player.Instance.gameObject.transform.position.z,
					Player.Instance.Target.PointName);
			} else {
				if (Player.Instance.CurrentPoint.Discovered) {
					return string.Format (
						"当前坐标 X：{0} Y：{1} Z：{2}\n" +
						"当前位置 {3}\n位置信息 {4}", 
						(int)Player.Instance.gameObject.transform.position.x,
						(int)Player.Instance.gameObject.transform.position.y,
						(int)Player.Instance.gameObject.transform.position.z,
						Player.Instance.CurrentPoint.PointName,
						Player.Instance.CurrentPoint.DiscoveredDescription);
				} else {
					return string.Format (
						"当前坐标 X：{0} Y：{1} Z：{2}\n" +
						"当前位置 {3}\n位置信息 {4}", 
						(int)Player.Instance.gameObject.transform.position.x,
						(int)Player.Instance.gameObject.transform.position.y,
						(int)Player.Instance.gameObject.transform.position.z,
						Player.Instance.CurrentPoint.PointName,
						Player.Instance.CurrentPoint.PointDescription);
				}
			}
		}
	}
}
