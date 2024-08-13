using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class Bolsa : MonoBehaviour
{
	public Pallet.Valores Monto;
	//public int IdPlayer = 0;
	public string TagPlayer = "";
	public Texture2D ImagenInventario;
	Player Pj = null;
	
	bool Desapareciendo;
	public GameObject Particulas;
	[FormerlySerializedAs("TiempParts")] public float TiempoRespawn = 2.5f;
	float timerRespawn;

	// Use this for initialization
	void Start () 
	{
		Monto = Pallet.Valores.Valor2;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!Desapareciendo) return;
		
		timerRespawn -= Time.deltaTime;
		if (timerRespawn <= 0)
		{
			GetComponent<Renderer>().enabled = true;
			GetComponent<Collider>().enabled = true;
			Desapareciendo = false;
		}
	}
	
	void OnTriggerEnter(Collider coll)
	{
		if(coll.tag == TagPlayer)
		{
			Pj = coll.GetComponent<Player>();
			if(Pj.AgregarBolsa(this))
				Desaparecer();
		}
	}
	
	public void Desaparecer()
	{
		Instantiate(Particulas, transform.position, Quaternion.identity);
		Desapareciendo = true;
		timerRespawn = TiempoRespawn;
		
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
	}
}