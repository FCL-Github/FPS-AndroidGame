using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("MyGameScripts/GameManager")]
public class GameManager : MonoBehaviour {

	public static GameManager Instance = null;

	//Player m_player;
	// 游戏得分
	public int m_score = 0;
	public int m_life = 100;
	// 游戏最高得分
	public static int m_hiscore = 0;
	// 弹药数量
	public int m_ammo = 100;

	// UI文字
	Text txt_ammo;
	Text txt_hiscore;
	Text txt_life;
	Text txt_score;

	public int flag = 0,flag1=0;

	// Use this for initialization
	void Start () {

		Instance = this;

		//m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		// 获得设置的UI文字
		foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
		{

			if (t.name.CompareTo("txt_ammo") == 0)
			{
				txt_ammo = t.GetComponent<Text>();
			}
			else if (t.name.CompareTo("txt_hiscore") == 0)
			{
				txt_hiscore = t.GetComponent<Text>();
				txt_hiscore.text = "High Score " + m_hiscore;
			}
			else if (t.name.CompareTo("txt_life") == 0)
			{
				txt_life = t.GetComponent<Text>();
			}
			else if (t.name.CompareTo("txt_score") == 0)
			{
				txt_score = t.GetComponent<Text>();
			}
		}

	}
	void Update()
	{
		if (m_life <= 0)
			return;
	}

	void OnGUI()
	{
		GUIStyle fontStyle = new GUIStyle();  
		//fontStyle.normal.background = null;    //设置背景填充  
		fontStyle.normal.textColor= new Color(1,1,1);   //设置字体颜色  
		fontStyle.fontSize = 30;       //字体大小  

		if (m_life <= 0) {
			flag = 0;
			// 居中显示文字
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;

			// 改变文字大小
			GUI.skin.label.fontSize = 40;

			// 显示Game Over
			GUI.Label (new Rect (0, 0, Screen.width, Screen.height), "Game Over");

			// 显示重新游戏按钮
			GUI.skin.button.fontSize = 40;
			if (GUI.Button (new Rect (Screen.width * 0.5f - 150, Screen.height * 0.62f, 300, 100), "Try again")) {
				SceneManager.LoadScene ("demo");
			}

			GUI.skin.label.fontSize = 40;
			if (GUI.Button (new Rect (Screen.width * 0.5f - 150, Screen.height * 0.78f, 300, 100), "Quit")) {
				Application.Quit ();
			}
		} 
		else if (m_life > 0) 
		{
			flag = 0;flag1 = 0;
			GUI.skin.button.fontSize =30;
			if (GUI.RepeatButton (new Rect (Screen.width * 0.9f, Screen.height * 0.9f, 100, 70), "Shoot"))
			{
				flag1 = 1;
			} 
			if (GUI.RepeatButton (new Rect (Screen.width * 0.15f, Screen.height * 0.70f, 100, 70), "Up")) {
				flag = 1;
			} 
			if (GUI.RepeatButton (new Rect (Screen.width * 0.15f, Screen.height * 0.9f, 100, 70), "Down")) {
				flag = 2;
			} 
			if (GUI.RepeatButton (new Rect (Screen.width * 0.05f, Screen.height * 0.8f, 100, 70), "Left")) {
				flag = 3;
			} 
			if (GUI.RepeatButton (new Rect (Screen.width * 0.25f, Screen.height * 0.8f, 100, 70), "Right")) {
				flag = 4;
			} 
		}
	}

	// 更新分数
	public void SetScore(int score)
	{
		m_score+= score;

		if (m_score > m_hiscore)
			m_hiscore = m_score;

		txt_score.text = "Score <color=white>" + m_score  + "</color>";;
		txt_hiscore.text = "High Score " + m_hiscore;

	}

	// 更新弹药
	public void SetAmmo(int ammo)
	{
		m_ammo -= ammo;

		// 如果弹药为负数，重新填弹
		if (m_ammo <= 0)
			m_ammo = 100 - m_ammo;

		txt_ammo.text = m_ammo.ToString()+"/100";
	}

	// 更新生命
	public void SetLife(int life)
	{
		if (m_life >0) 
		{
			m_life -= life;
			txt_life.text = m_life.ToString ();
		}
		else
			m_life = 0;
	}
}