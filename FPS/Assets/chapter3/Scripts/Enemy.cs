using UnityEngine;
using System.Collections;


[AddComponentMenu("MyGameScripts/Enemy")]
public class Enemy : MonoBehaviour {
	Transform m_transform;

	Animator m_anti;
	Player m_player;
	NavMeshAgent m_agent;
	float m_movSpeed=1.5f;
	float m_rotSpeed = 30;
	float m_timer = 2;
	int m_life = 15;
	protected EnemySpawn m_spawn;

	// Use this for initialization
	void Start () {
	
		m_transform = this.transform;
		m_anti = GetComponent<Animator> ();
		m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();

		m_agent = GetComponent<NavMeshAgent> ();
		m_agent.speed = m_movSpeed;
		//m_agent.SetDestination (m_player.m_transform.position);
	}
	public void Init(EnemySpawn spawn)
	{
		m_spawn = spawn;
		m_spawn.m_enemyCount++;
	}
	// Update is called once per frame
	void Update () 
	{
		
		m_agent.speed = m_movSpeed;
		if (m_player.m_life <= 0) {
			Destroy (this.gameObject);
			return;
		}
		AnimatorStateInfo stateInfo = m_anti.GetCurrentAnimatorStateInfo(0);
		if(stateInfo.fullPathHash==Animator.StringToHash("Base Layer.idle")&& !m_anti.IsInTransition(0))
		{
			m_anti.SetBool ("idle",false);

			m_timer -= Time.deltaTime;
			if (m_timer > 0)
				return;

			if(Vector3.Distance(m_player.m_transform.position,m_transform.position)<=2.0f)
			{
				m_anti.SetBool ("attack",true );

			}
			else 
			{
				m_timer = 1;
				m_anti.SetBool ("run",true);
				//m_agent.SetDestination (m_player.m_transform.position);
			}
		}
		if(stateInfo.fullPathHash==Animator.StringToHash("Base Layer.run")&& !m_anti.IsInTransition(0))
		{
			m_anti.SetBool ("run",false);
			m_agent.Resume ();
			m_timer -= Time.deltaTime;
			if (m_timer < 0) 
			{
				m_agent.SetDestination (m_player.m_transform.position);
				m_timer = 1;
			}

			if(Vector3.Distance(m_player.m_transform.position,m_transform.position)<=2.0f)
			{
				m_agent.Stop ();
				m_anti.SetBool ("attack",true );

			}
		}
		if(stateInfo.fullPathHash==Animator.StringToHash("Base Layer.attack")&& !m_anti.IsInTransition(0))
		{
			RotateTo ();

			m_anti.SetBool ("attack",false);

			if (stateInfo.normalizedTime >= 1.0f && Vector3.Distance (m_player.m_transform.position, m_transform.position) <= 2.0f) {
				GameManager.Instance.SetLife (1);
				m_anti.SetBool ("idle", true);
				m_timer = 2;
				//m_player.OnDamage (1);
			} else if (stateInfo.normalizedTime >= 1.0f && Vector3.Distance (m_player.m_transform.position, m_transform.position) > 2.0f)
				m_anti.SetBool ("run",true);
		}
		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.death") && !m_anti.IsInTransition (0))
		{
			if (stateInfo.normalizedTime >= 1.0f)
			{
				m_spawn.m_enemyCount--;
				GameManager.Instance.SetScore (100);
				Destroy (this.gameObject);
			}
		}
	}
	void RotateTo()
	{
		Vector3 targetDir = m_player.m_transform.position - m_transform.position;
		Vector3 newDir = Vector3.RotateTowards (transform.forward,targetDir,m_rotSpeed*Time.deltaTime,0.0f);
		m_transform.rotation = Quaternion.LookRotation (newDir);
	}
	public void OnDamage(int damage)
	{
		m_life -= damage;
		if(m_life<=0)
		{
			m_anti.SetBool ("death",true);
		}
	}

}
