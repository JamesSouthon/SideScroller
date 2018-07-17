using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBlocks : MonoBehaviour
{
    public SpriteRenderer[] m_Blocks;
    public Player m_MyPlayer;

    private int m_HealthPerBlock;


	// Use this for initialization
	void Awake ()
    {
        m_HealthPerBlock = m_MyPlayer.m_Stats.MaxHealth / m_Blocks.Length;

    }
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < m_Blocks.Length; i++)
        {
            int lifeToCheck = m_HealthPerBlock * (i+1);
            if (m_MyPlayer.m_Stats.Health < lifeToCheck)
            {
                m_Blocks[i].enabled = false;
            }
            else
            {
                m_Blocks[i].enabled = true;
            }
        }
	}
}
