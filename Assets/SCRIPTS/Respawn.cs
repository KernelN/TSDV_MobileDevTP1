using UnityEngine;
using System.Collections;
using Brinks.Gameplay;

public class Respawn : MonoBehaviour 
{
	public float AngMax = 90;//angulo maximo antes del cual se reinicia el camion
	int VerifPorCuadro = 20;
	int Contador = 0;
	
	public float RangMinDer = 0;
	public float RangMaxDer = 0;
	
	bool IgnorandoColision = false;
	public float TiempDeNoColision = 2;
	float Tempo = 0;

	ZoneManager zm;
	
	//--------------------------------------------------------//

	void Start () 
	{
		//restaura las colisiones
		Physics.IgnoreLayerCollision(8,9,false);
		zm = ZoneManager.inst;
	}
	void Update ()
	{
		Contador++;
		if (Contador == VerifPorCuadro)
		{
			Contador = 0;
			if(zm.playersCpByHash.TryGetValue(transform.GetHashCode(), out Transform cp))
				if (AngMax < Quaternion.Angle(transform.rotation, cp.rotation))
					Respawnear();
		}
		
		if(IgnorandoColision)
		{
			Tempo += T.GetDT();
			if(Tempo > TiempDeNoColision)
			{
				IgnorarColision(false);
			}
		}
		
	}
	
	//--------------------------------------------------------//
	
	public void Respawnear()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		
		gameObject.GetComponent<CarController>().SetGiro(0f);
		
		if(zm.playersCpByHash.TryGetValue(transform.GetHashCode(), out Transform cp))
		{
			Vector3 offset = cp.transform.right * Random.Range(RangMinDer, RangMaxDer);
			if (GetComponent<Visualizacion>().LadoAct == Visualizacion.Lado.Der)
				transform.position = cp.transform.position + offset;
			else 
				transform.position = cp.transform.position - offset;
			transform.forward = cp.transform.forward;
		}
		
		IgnorarColision(true);
	}
	public void Respawnear(Vector3 pos)
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		
		gameObject.GetComponent<CarController>().SetGiro(0f);

        transform.position = pos;
		
		IgnorarColision(true);
	}
	public void Respawnear(Vector3 pos, Vector3 dir)
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		
		gameObject.GetComponent<CarController>().SetGiro(0f);

        transform.position = pos;
		transform.forward = dir;
		
		IgnorarColision(true);
	}
	void IgnorarColision(bool b)
	{
		//no contempla si los dos camiones respawnean relativamente cerca en el espacio 
		//temporal y uno de ellos va contra el otro, 
		//justo el segundo cancela las colisiones e inmediatamente el 1º las reactiva, 
		//entonces colisionan, pero es dificil que suceda. 
		
		Physics.IgnoreLayerCollision(8,9,b);
		IgnorandoColision = b;	
		Tempo = 0;
	}
}