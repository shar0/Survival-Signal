using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SystemProgram
{
	Boot,
	Login,
	Welcome,
	Main,
	Scan,
	Move,
	Over,
	End
}

public enum OverPhase
{
	Ready,
	Down
}

public enum EndPhase
{
	Ready,
	Down
}

public enum MainPhase
{
	Hint,
	WaitInput,
	InvalidInput
}

public enum BootPhase
{
	Loading,
	Loaded
}

public enum LoginPhase
{
	HintInputName,
	WaitName,
	Login
}

public enum MovePhase
{
	Hint,
	WaitInput,
	InvalidInput
}

public class GameManager : MonoBehaviour
{
	public const float IntervalTime = 1.8f;
	private float intervals = IntervalTime;

	private SystemProgram currProgram = SystemProgram.Boot;
	private MovePhase currMovePhase;
	private MainPhase currMainPhase = MainPhase.Hint;
	private LoginPhase currLoginPhase = LoginPhase.HintInputName;
	private int loginWaitTicks = 10;
	private BootPhase currBootPhase = BootPhase.Loading;
	private EndPhase endPhase = EndPhase.Ready;
	private OverPhase overPhase = OverPhase.Ready;
	private string journalLog = "";
	private List<int> unlockedJournal = new List<int>();

	public SystemProgram CurrerentProgram {
		get {
			return currProgram;
		}
	}

	private static GameManager instance;

	public static GameManager Instance {
		get {
			return instance;
		}
	}

	private static GameConfig gameConfig;
	private static Journal journal;

	void Awake ()
	{
		instance = this;
		Application.targetFrameRate = 45;
	}

	// Use this for initialization
	void Start ()
	{
	}

	void InitConfig ()
	{
		TextAsset configFile = Resources.Load<TextAsset> ("GameConfig");
		gameConfig = JsonUtility.FromJson<GameConfig> (configFile.text);
		TextAsset journalFile = Resources.Load<TextAsset> ("Journal");
		journal = JsonUtility.FromJson<Journal> (journalFile.text);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ((currProgram == SystemProgram.Over && overPhase == OverPhase.Down) || 
			(currProgram == SystemProgram.End && endPhase == EndPhase.Down)) {
			return;
		}

		NextTick ();
	}

	void NextTick ()
	{
		intervals -= Time.deltaTime;
		if (intervals <= 0) {
			ProcessProgram ();
			intervals = IntervalTime;
		}
	}

	void ProcessProgram ()
	{
		switch (currProgram) {
		case SystemProgram.Boot:
			if (ProcessBoot ()) {
				currProgram = SystemProgram.Login;
			}
			break;
		case SystemProgram.Login:
			if (ProcessLogin ()) {
				currProgram = SystemProgram.Welcome;
			}
			break;
		case SystemProgram.Welcome:
			Terminal.Instance.AppendLine (gameConfig.welcomeMessage+"\n");
			currProgram = SystemProgram.Main;
			UnlockJournal(0);
			break;
		case SystemProgram.Main:
			ProcessMain ();
			break;
		case SystemProgram.Move:
			ProcessMove ();
			break;
		case SystemProgram.End:
			if (endPhase == EndPhase.Ready) {
				Terminal.Instance.AppendLine ("▇▇▇▇▇ 航海日志 ▇▇▇▇▇\n" + journalLog);
				Terminal.Instance.AppendLine ("<b>你成功救出了目标</b>\n");
				Terminal.Instance.AppendLine ("<b>游戏结束?</b>\n");
				Terminal.Instance.AppendLine ("\n\n\n输入“r”重新开始\n");
				endPhase = EndPhase.Down;
			} else {
			}
			break;
		case SystemProgram.Over:
			if (overPhase == OverPhase.Ready) {
				Terminal.Instance.AppendLine ("SYS_ERROR\n");
				Terminal.Instance.AppendLine ("SYS_DOWN\n");
				Terminal.Instance.AppendLine ("SYS_NEED_RESTART\n");
				Terminal.Instance.AppendLine ("SYS_ERROR\n");
				Terminal.Instance.AppendLine ("SYS_DOWN\n");
				Terminal.Instance.AppendLine ("SYS_NEED_RESTART\n");
				Terminal.Instance.AppendLine ("SYS_ERROR\n");
				Terminal.Instance.AppendLine ("SYS_DOWN\n");
				Terminal.Instance.AppendLine ("SYS_NEED_RESTART\n");
				Terminal.Instance.AppendLine ("SYS_ERROR\n");
				Terminal.Instance.AppendLine ("SYS_DOWN\n");
				Terminal.Instance.AppendLine ("SYS_NEED_RESTART\n");
				Terminal.Instance.AppendLine ("\n\n\n输入“r”重新开始\n");
				overPhase = OverPhase.Down;
			} else if (overPhase == OverPhase.Down) {
			}
			break;
		}
	}

	public void ReloadGame()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	private DataPoint[] locationArray;

	public void FinishMove ()
	{
		currProgram = SystemProgram.Main;
		currMainPhase = MainPhase.Hint;
		Terminal.Instance.AppendLine ("跃迁程序已停止\n");
		if (Player.Instance.CurrentPoint.Type == 1) {
			Terminal.Instance.AppendLine ("你进入了虫洞，本系统尚未被设计于虫洞航行！\n");
			GameOver ();
		}
	}
		
	public void GameEnd()
	{
		currProgram = SystemProgram.End;
	}

	public void GameOver()
	{
		currProgram = SystemProgram.Over;
	}

	bool ProcessMove ()
	{
		switch (currMovePhase) {
		case MovePhase.Hint:
			Terminal.Instance.AppendLine ("输入您要前往的地点：");
			string hintStr = "";
			locationArray = new DataPoint[Player.Instance.CurrentPoint.EntryMap.Length];

			for (var i = 0; i < Player.Instance.CurrentPoint.EntryMap.Length; i++) {
				string name = Player.Instance.CurrentPoint.EntryMap [i];
				locationArray [i] = GameObject.Find (name).GetComponent<DataPoint> ();
				if (!locationArray [i].Discovered) {
					hintStr += i + ": " + locationArray [i].PointName + "（未扫描）" + Coord (locationArray [i]) + "\n";
				} else {
					hintStr += i + ": " + locationArray [i].PointName + " " + Coord (locationArray [i]) + "\n";
				}
			}

			hintStr += "x: 停止跃迁程序\n";

			Terminal.Instance.AppendLine (hintStr);
			currMovePhase = MovePhase.WaitInput;
			break;
		case MovePhase.WaitInput:
			break;
		case MovePhase.InvalidInput:
			Terminal.Instance.AppendLine ("地点输入错误！");
			currMovePhase = MovePhase.WaitInput;
			break;
		}
		return false;
	}

	public void UnlockJournal(int id)
	{
		if (!unlockedJournal.Exists (n => n == id)) {
			Terminal.Instance.AppendLine ("*** 航海日志已更新 ***\n");
			journalLog += "===========================================\n" + journal.journals [id] + "\n";
			unlockedJournal.Add (id);
		}
	}

	private string Coord (DataPoint point)
	{
		return string.Format ("x：{0}, y：{1}, z：{2}", 
			point.transform.position.x,
			point.transform.position.y,
			point.transform.position.z
		);
	}

	public void OnCommandInput (string text)
	{
		if (Player.Instance.IsMove) {
			Terminal.Instance.AppendLine ("跃迁中无法使用系统！");
			return;
		}

		if (text == "whosyourdaddy") {
			Terminal.Instance.AppendLine ("\n\n\n;-)\n\n\n");
			return;
		}

		if (currProgram == SystemProgram.Over || currProgram == SystemProgram.End) {
			if (text == "r") {
				ReloadGame ();
			}
		}

		if (currProgram == SystemProgram.Login &&
		    currLoginPhase == LoginPhase.WaitName &&
		    text.Length > 0) {
			gameConfig.username = text;
			Terminal.Instance.AppendText (" " + gameConfig.username);
			Terminal.Instance.AppendLine ("密码: ******");
			currLoginPhase = LoginPhase.Login;
		}

		if (currProgram == SystemProgram.Main &&
		    currMainPhase == MainPhase.WaitInput &&
		    text.Length > 0) {
			switch (text) {
			case "m":
				currProgram = SystemProgram.Move;
				currMovePhase = MovePhase.Hint;
				break;
			case "s":
				if (Player.Instance.CurrentPoint.Discovery ()) {
					Terminal.Instance.AppendLine ("扫描完成\n");
				}
				break;
			case "o":
				if (!Player.Instance.CurrentPoint.Discovered) {
					Terminal.Instance.AppendLine ("地点尚未扫描，无法启动打捞程序\n");
					return;
				}
				bool result = Player.Instance.CurrentPoint.Scan ();
				if (result) {
					Terminal.Instance.AppendLine ("打捞完成\n");
				} else {
					Terminal.Instance.AppendLine ("已完全打捞\n");
				}
				break;
			case "j":
				Terminal.Instance.AppendLine ("▇▇▇▇▇ 航海日志 ▇▇▇▇▇\n" + journalLog);
				break;
			default:
				currMainPhase = MainPhase.InvalidInput;
				break;
			}
		}

		if (currProgram == SystemProgram.Move &&
		    currMovePhase == MovePhase.WaitInput &&
		    text.Length > 0) {
			if (text == "x") {
				currProgram = SystemProgram.Main;
				currMainPhase = MainPhase.WaitInput;
				Terminal.Instance.AppendLine ("跃迁程序已停止\n");
				return;
			}
			int number;
			bool result = int.TryParse (text, out number);
			if (!result) {
				currMovePhase = MovePhase.InvalidInput;
				return;
			}
			if (number >= locationArray.Length || number < 0) {
				currMovePhase = MovePhase.InvalidInput;
			} else {
				// TODO: check item
				if (locationArray [number].Type == 3 || locationArray [number].Type == 2) {
					if (!Player.Instance.Inventory.Exists (n => n == 3)) {
						Terminal.Instance.AppendLine ("通信模块被干扰，无法准确定位目的地\n");
						return;
					}
				}
				Player.Instance.Target = locationArray [number];
				Terminal.Instance.AppendLine ("即将跃迁至目的地：" + locationArray [number].PointName);
				locationArray = null;
			}
		}
	}

	bool ProcessMain ()
	{
		switch (currMainPhase) {
		case MainPhase.Hint:
			Terminal.Instance.AppendLine ("\n" + gameConfig.mainMessage);
			currMainPhase = MainPhase.WaitInput;
			break;
		case MainPhase.WaitInput:
			break;
		case MainPhase.InvalidInput:
			Terminal.Instance.AppendLine ("指令输入错误！\n");
			currMainPhase = MainPhase.WaitInput;
			break;
		}
		return false;
	}

	bool ProcessBoot ()
	{
		switch (currBootPhase) {
		case BootPhase.Loading:
			// TODO:Sync load, modify to async in future
			Terminal.Instance.AppendText ("Initialization System...");
			InitConfig ();
			currBootPhase = BootPhase.Loaded;
			break;
		case BootPhase.Loaded:
			Terminal.Instance.AppendLine ("System Language : Chinese\n系统初始化成功！");
			return true;
		}
		return false;
	}

	bool ProcessLogin ()
	{
		switch (currLoginPhase) {
		case LoginPhase.HintInputName:
			Terminal.Instance.AppendLine ("用户名:");
			currLoginPhase = LoginPhase.WaitName;
			break;
		case LoginPhase.WaitName:
			if ((loginWaitTicks--) < 0) {
				Terminal.Instance.AppendText (" " + gameConfig.username);
				Terminal.Instance.AppendLine ("密码: ******");
				currLoginPhase = LoginPhase.Login;
			}
			break;
		case LoginPhase.Login:
			Terminal.Instance.AppendLine ("正在登陆系统(192.168.1.6)...\n系统登录成功！");
			return true;
		}
		return false;
	}
}
