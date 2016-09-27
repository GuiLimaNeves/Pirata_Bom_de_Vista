﻿using UnityEngine;
using System.Collections;

public class TriggerBehaviour : MonoBehaviour {

    public bool ativo = true;
    public bool utilizarSphere = true;
    public bool interagindo = false;
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter(Collider objeto) {
        if (ativo) {
            if (utilizarSphere)
            {
                if (objeto.gameObject.CompareTag("Player"))
                {
                    ObjetivosController.instance.ProximoObjetivo();
                    ativo = false;
                }
            }
        }
        
    }

    void OnTriggerExit(Collider objeto)
    {
        if (utilizarSphere)
        {
            if (objeto.gameObject.CompareTag("Player"))
            {
                interagindo = false;
            }
        }
    }
}