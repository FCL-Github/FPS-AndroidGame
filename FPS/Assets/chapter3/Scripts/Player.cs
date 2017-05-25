using UnityEngine;
using System.Collections;

[AddComponentMenu("MyGameScripts/Player")]
public class Player : MonoBehaviour {

	public Transform m_transform;

	// 记录手指触屏的位置
	Vector2 m_screenpos = new Vector2();

	// 角色控制器组件
	CharacterController m_ch;

	// 角色移动速度
	float m_movSpeed = 4.0f;

	// 重力
	float m_gravity = 2.0f;

	// 摄像机
	Transform m_camTransform;

	// 摄像机移动速度
	//float m_speed = 0.1f;

	// 摄像机旋转角度
	Vector3 m_camRot;

	// 摄像机高度
	float m_camHeight = 1.4f;

	// 生命值
	public int m_life =100;

	GameManager m_gameManager;

	// 射击时，射线能射到的碰撞层
	public LayerMask m_layer;

	// 射中目标后的粒子效果
	public Transform m_fx;

	// 射击音效
	public AudioClip m_audio;
	float m_shootTimer=0;

	// Use this for initialization
	void Start () {

		m_transform = this.transform;
		m_gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

		// 允许多点触控
		Input.multiTouchEnabled = true;

		// 获取角色控制器组件
		m_ch = this.GetComponent<CharacterController>();

		// 获取摄像机
		m_camTransform = Camera.main.transform;

		// 设置摄像机初始位置
		Vector3 pos = m_transform.position;
		pos.y += m_camHeight;
		m_camTransform.position = pos;

		// 设置摄像机的旋转方向与主角一致
		m_camTransform.rotation = m_transform.rotation;
		m_camRot = m_camTransform.eulerAngles;

		Cursor.visible = false;


	}

	// Update is called once per frame
	void Update () {
		m_life = m_gameManager.m_life;
		// 如果生命为0，什么也不做
		if (m_life <= 0) 
		{
			Cursor.visible = true;
			return;
		}
		// 按手机上的返回键退出当前程序
		if (Input.GetKeyUp(KeyCode.Escape))
			Application.Quit();

		#if !UNITY_EDITOR && ( UNITY_IOS || UNITY_ANDROID )
		MobileInput(); 
		#else
		Control();
		#endif

		m_shootTimer -= Time.deltaTime;
		if(m_gameManager.flag1==1 && m_shootTimer<=0)
		{
			m_shootTimer = 0.18f;
			this.GetComponent<AudioSource>().PlayOneShot(m_audio);
			// 减少弹药，更新弹药UI
			GameManager.Instance.SetAmmo(1);

			// RaycastHit用来保存射线的探测结果
			RaycastHit info;
			// 从muzzlepoint的位置，向摄像机面向的正方向射出一根射线
			// 射线只能与m_layer所指定的层碰撞
			bool hit = Physics.Raycast(m_camTransform.position, m_camTransform.TransformDirection(Vector3.forward), out info, 1000, m_layer);
			if (hit)
			{
				// 如果射中了tag为enemy的游戏体
				if (info.transform.tag.CompareTo("enemy") == 0)
				{
					Enemy enemy = info.transform.GetComponent<Enemy>();

					// 敌人减少生命
					enemy.OnDamage(1);
				}

				// 在射中的地方释放一个粒子效果
				Instantiate(m_fx, info.point, info.transform.rotation);
			}
		}
	}

	void Control()
	{

		//获取鼠标移动距离
		float rh = Input.GetAxis("Mouse X");
		float rv = Input.GetAxis("Mouse Y");

		// 旋转摄像机
		m_camRot.x -= rv;
		m_camRot.y += rh;
		m_camTransform.eulerAngles = m_camRot;

		// 使主角的面向方向与摄像机一致
		Vector3 camrot = m_camTransform.eulerAngles;
		camrot.x = 0; camrot.z = 0;
		m_transform.eulerAngles = camrot;

		// 定义3个值控制移动
		float xm = 0, ym = 0, zm = 0;

		// 重力运动
		ym -= m_gravity*Time.deltaTime;

		// 上下左右运动
		if (m_gameManager.flag==1){
			zm += m_movSpeed * Time.deltaTime;
		}
		else if (m_gameManager.flag==2){
			zm -= m_movSpeed * Time.deltaTime;
		}

		if (m_gameManager.flag==3){
			xm -= m_movSpeed * Time.deltaTime;
		}
		else if (m_gameManager.flag==4){
			xm += m_movSpeed * Time.deltaTime;
		}


		// 使用角色控制器提供的Move函数进行移动 它会自动检测碰撞
		m_ch.Move( m_transform.TransformDirection(new Vector3(xm, ym, zm)) );

		// 使摄像机的位置与主角一致
		Vector3 pos = m_transform.position;
		pos.y += m_camHeight;
		m_camTransform.position = pos;
	}

	// 移动平台触屏操作
	void MobileInput()
	{

		if (Input.touchCount <= 0)
			return;

		// 1个手指触摸屏幕
		if (Input.touchCount == 1)
		{

			if (Input.touches[0].phase == TouchPhase.Began)
			{
				// 记录手指触屏的位置
				m_screenpos = Input.touches[0].position;

			}
			// 手指移动
			else if (Input.touches[0].phase == TouchPhase.Moved)
			{
				// 移动摄像机
				//Camera.main.transform.Translate(new Vector3(Input.touches[0].deltaPosition.x * m_speed, Input.touches[0].deltaPosition.y * m_speed, 0));
			}

			// 手指离开屏幕 判断移动方向
			if (Input.touches[0].phase == TouchPhase.Ended && 
				Input.touches[0].phase != TouchPhase.Canceled)
			{

				Vector2 pos = Input.touches[0].position;

				// 手指水平移动
				if (Mathf.Abs(m_screenpos.x - pos.x) > Mathf.Abs(m_screenpos.y - pos.y))
				{
					if (m_screenpos.x > pos.x){
						//手指向左划动
						// 旋转摄像机
						m_camRot.y -= Mathf.Abs(m_screenpos.y - pos.y);
						m_camTransform.eulerAngles = m_camRot;
					}
					else{
						//手指向右划动
						// 旋转摄像机
						m_camRot.y += Mathf.Abs(m_screenpos.y - pos.y);
						m_camTransform.eulerAngles = m_camRot;

					}
				}
				else   // 手指垂直移动
				{
					if (m_screenpos.y > pos.y){
						//手指向下划动
						// 旋转摄像机

						//m_camRot.x += m_screenpos.x - pos.x;
						//m_camTransform.eulerAngles = m_camRot;

					}
					else{
						//手指向上划动
						// 旋转摄像机
						//m_camRot.x -= m_screenpos.x - pos.x;
						//m_camTransform.eulerAngles = m_camRot;
					}
				}
			}
		}
		// 使主角的面向方向与摄像机一致
		Vector3 camrot = m_camTransform.eulerAngles;
		camrot.x = 0; camrot.z = 0;
		m_transform.eulerAngles = camrot;

		// 定义3个值控制移动
		float xm = 0, ym = 0, zm = 0;

		// 重力运动
		ym -= m_gravity*Time.deltaTime;

		// 上下左右运动
		if (m_gameManager.flag==1){
			zm += m_movSpeed * Time.deltaTime;
		}
		else if (m_gameManager.flag==2){
			zm -= m_movSpeed * Time.deltaTime;
		}

		if (m_gameManager.flag==3){
			xm -= m_movSpeed * Time.deltaTime;
		}
		else if (m_gameManager.flag==4){
			xm += m_movSpeed * Time.deltaTime;
		}


		// 使用角色控制器提供的Move函数进行移动 它会自动检测碰撞
		m_ch.Move( m_transform.TransformDirection(new Vector3(xm, ym, zm)) );

		// 使摄像机的位置与主角一致
		Vector3 playerPos = m_transform.position;
		playerPos.y += m_camHeight;
		m_camTransform.position = playerPos;

	}

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
	}


}
