using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class DestroyParticleOnFin : MonoBehaviour {

    private ParticleSystem m_PS;
	// Use this for initialization
	void Start () {
        m_PS = GetComponent<ParticleSystem>();

    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (m_PS.isStopped)
        {
            Destroy(gameObject);
        }
	}
}
