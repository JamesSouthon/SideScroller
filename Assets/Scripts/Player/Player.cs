using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStats
    {
        public int MaxHealth = 100;
        public int Health = 0;

        public PlayerStats()
        {
            Reset();
        }

        public bool IsDead
        {
            get { return Health <= 0.0f; }
        }

        public void Reset()
        {
            Health = MaxHealth;
        }
    }


    public float m_TimeUntilRespawn;
    private float m_DeathTime = 0;

    public GameObject op;

    public Transform pSpawn;
    public Transform opSpawn;
    
    
    public PlayerStats m_Stats = new PlayerStats();

    private RaycastHit2D[] m_Hits = new RaycastHit2D[HIT_BACKLOG];
    private const int HIT_BACKLOG = 4;
    private const int HIT_MULTIPLIER = 2;

    [Header("Attack Settings")]
    private BoxCollider2D m_BaseCollider;
    [SerializeField] private int m_BaseDamage;
    [SerializeField] private Vector2 m_AttackCenter;
    [SerializeField] private Vector2 m_AttackSize;
    [SerializeField] private LayerMask m_AttackLayer;
    [SerializeField] private float m_AttackCooldown = 1;
    [SerializeField] private float m_AttackAnimResetTime = 1f;
    private float m_LastAttackTime;
    private bool m_CanAttack = true;

    [Header("Particles")]
    public GameObject m_BloodPS;
    public GameObject m_RespawnPS;
    public GameObject m_DeathPS;

    [Header("Debug")]
    [SerializeField] private bool m_Debug_Respawn;

    public GameObject m_Visuals;
    private PlatformerMotor2D m_Motor;
    private bool m_IsFacingleft = true;

    public bool m_AttackingAnim;

    public BoxCollider2D Collider
    {
        get
        {
            if (m_BaseCollider == null)
            {
                m_BaseCollider = GetComponent<BoxCollider2D>();
            }
            return m_BaseCollider;   
        }

    }

    void Awake()
    {
        m_Motor = GetComponent<PlatformerMotor2D>();
        m_BaseCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (op.gameObject.tag == "Dead")
        {
            m_Stats.Reset();
        }

        

        if (m_Stats.IsDead)
        {
            m_Motor.frozen = true;
            if (m_DeathTime + m_TimeUntilRespawn < Time.time) ;
            {
                Respawn();
                
            }
            return;
        }

        if (m_Motor.velocity.x <= -0.1f)
        {
            m_IsFacingleft = false;
        }
        else if (m_Motor.velocity.x >= 0.1f)
        {
            m_IsFacingleft = true;
        }

        if (m_LastAttackTime + m_AttackAnimResetTime < Time.time)
        {
            m_AttackingAnim = false;
        }

        if (m_LastAttackTime + m_AttackCooldown < Time.time)
        {
            m_CanAttack = true;
           
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 size = m_AttackSize;

        Vector2 center = m_AttackCenter + Collider.offset;
        center.x *= m_IsFacingleft ? -1 : 1;
        Debug.Log(center.x);

        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Gizmos.DrawSphere(pos + center, 0.01f);
        Gizmos.DrawWireCube(pos + center, size);
    }

    private int GetNearbyHitsBox(Vector2 _center, Vector2 _size, Vector2 _direction, float _distance, LayerMask _layer)
    {
        int num = Physics2D.BoxCastNonAlloc(
            _center,
            _size,
            0f,
            _direction,
            m_Hits,
            _distance,
            _layer);
        if (num <= m_Hits.Length)
        {
            return num;
        }
        m_Hits = new RaycastHit2D[(int)(HIT_MULTIPLIER * num)];
        num = Physics2D.BoxCastNonAlloc(
            _center,
            _size,
            0f,
            _direction,
            m_Hits,
            _distance,
            _layer);
        return num;
    }

    public void Attack()
    {
        if (!m_CanAttack) return;
        m_CanAttack = false;
        m_AttackingAnim = true;
        m_LastAttackTime = Time.time;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 center = m_AttackCenter;
        float facing = (m_IsFacingleft ? -1 : 1);

        center.x *= facing;

        Debug.DrawLine(pos + center, transform.position + new Vector3(center.x, center.y) + (Vector3.left * facing * m_AttackSize.x/2f));
        int noHits = GetNearbyHitsBox(pos + center,
                                      m_AttackSize,
                                      Vector3.left * facing,
                                      0,
                                      m_AttackLayer);

        for (int i = 0; i < noHits; i++)
        {
            if (m_Hits[i])
            {
                Player p = m_Hits[i].transform.gameObject.GetComponent<Player>();
                if (p != null)
                {
                    p.TakeDamage(m_BaseDamage, gameObject);

                }
            }
        }
    }

    public void TakeDamage(int _damage, GameObject _attacker)
    {
        if (m_Stats.IsDead) return;
        m_Stats.Health -= _damage;

        if (m_Stats.IsDead)
        {
            GameObject go = Instantiate(m_DeathPS, transform);
            go.transform.localPosition = Vector3.zero;
            m_Visuals.SetActive(false);

            m_DeathTime = Time.time;
        }
        else
        {
            GameObject go = Instantiate(m_BloodPS, transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.LookAt(_attacker.transform);
        }
    }

    public void Respawn()
    {
        m_Motor.frozen = false;
        m_Stats.Reset();
        transform.position = pSpawn.position;
        GameObject go = Instantiate(m_RespawnPS, transform);
        go.transform.localPosition = pSpawn.position;
        m_Visuals.SetActive(true);

        //Scoreboard Text
        this.gameObject.tag = "Dead";

        op.transform.position = opSpawn.position;
        Debug.Log("Respawn");
    }
}
