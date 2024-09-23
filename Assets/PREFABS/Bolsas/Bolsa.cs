using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class Bolsa : MonoBehaviour
{
	public Pallet.Valores Monto;
	//public int IdPlayer = 0;
	public string TagPlayer = "";
	
	public GameObject Particulas;
	float timerRespawn;
	[SerializeField] Renderer rend;
	[SerializeField] Collider coll;
	bool activada = true;

	public System.Action<bool> Activada;

	void Awake () 
	{
		Monto = Pallet.Valores.Valor2;
		
		if(!rend) rend = GetComponent<Renderer>();
		if(!coll) coll = GetComponent<Collider>();

		//if both components are enabled, bag is active
		activada = rend.enabled && coll.enabled; 
	}
	void OnTriggerEnter(Collider coll)
	{
		if(coll.tag == TagPlayer)
		{
			coll.TryGetComponent(out Player Pj);
			if(Pj.AgregarBolsa(this))
				Desaparecer();
		}
	}
	public void Aparecer()
	{
		if(activada) return;
		activada = true;
		
		rend.enabled = true;
		coll.enabled = true;
		
		Activada?.Invoke(true);
	}
	public void Desaparecer(bool discreto = false)
	{
		if(!activada) return;
		activada = false;
		
		if(!discreto)
			Instantiate(Particulas, transform.position, Quaternion.identity);
		
		rend.enabled = false;
		coll.enabled = false;
		
		Activada?.Invoke(false);
	}
}