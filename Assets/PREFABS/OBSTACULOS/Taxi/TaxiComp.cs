using UnityEngine;

/// <summary>
/// basicamente lo que hace es que viaja en linea recta y ocacionalmente gira para un cosatado
/// previamente verificado, tambien cuando llega al final del recorrido se reinicia en la pos. orig.
/// </summary>
public class TaxiComp : MonoBehaviour 
{
	public string FinTaxiTag = "FinTaxi";
	public string LimiteTag = "Terreno";

	public float Vel = 0;
	float velFinal;
	const float velDifMod = 1.5f;
	
	public Vector2 TiempCadaCuantoDobla_MaxMin = Vector2.zero;
	const float TiempEntreGiroFacMod = 1.5f;
	
	public float DuracionGiroMax = 0.5f;
	
	public float DuracionGiroMin = 0.1f;
	
	public float DuracionGiro = 0;
	float TempoDurGir = 0;
	
	public float AlcanceVerif = 0;
	
	public string TagTerreno = "";
	
	public bool Girando = false;
	Vector3 RotIni;//pasa saber como volver a su posicion original
	Vector3 PosIni;//para saber donde reiniciar al taxi
	
	float TiempEntreGiro = 0;
	float TempoEntreGiro = 0;
	
	public float AngDeGiro = 30;
	
	RaycastHit RH;
	
	bool Respawneando = false;

	Transform[] players;
	
	
	enum Lado{ Der, Izq }
	
	//-----------------------------------------------------------------//

	// Use this for initialization
	void Start () 
	{
		TiempEntreGiro = Random.Range(TiempCadaCuantoDobla_MaxMin.x,
										TiempCadaCuantoDobla_MaxMin.y);
		if(DatosPartida.DificultadJuego == DatosPartida.Dificultad.Facil)
				TiempEntreGiro *= TiempEntreGiroFacMod;

		velFinal = Vel;
		if (DatosPartida.DificultadJuego == DatosPartida.Dificultad.Dificil)
			velFinal *= velDifMod;
		
		RotIni = transform.localEulerAngles;
		PosIni = transform.position;

		
		GameplayManager gm = GameplayManager.Instancia;
		if (gm.DosJugadores)
			players = new[] { gm.Player1.transform, gm.Player2.transform };
		else
			players = new[] { gm.Player1.transform };
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Respawneando)
		{
			if(Medicion())
				Respawn();
		}
		else
		{
			if(Girando)
			{
				TempoDurGir += Time.deltaTime;
				if(TempoDurGir > DuracionGiro)
				{
					TempoDurGir = 0;
					DejarDoblar();
				}
			}
			else
			{
				TempoEntreGiro += Time.deltaTime;
				if(TempoEntreGiro > TiempEntreGiro)
				{
					TempoEntreGiro = 0;
					Doblar();
				}
			}
		}
	} 
	void OnTriggerEnter(Collider coll)
	{
		if(coll.tag == FinTaxiTag)
		{
			transform.position = PosIni;
			transform.localEulerAngles = RotIni;
		}		
	}
	void OnCollisionEnter(Collision coll)
	{
		if(coll.transform.tag == LimiteTag)
		{
			Respawneando = true;
		}
	}
	void FixedUpdate () 
	{
		transform.position += transform.forward * (Time.fixedDeltaTime * velFinal);
	}
	
	//--------------------------------------------------------------------//
	
	bool VerificarCostado(Lado lado)
	{
		switch (lado)
		{
		case Lado.Der:
			if(Physics.Raycast(transform.position, transform.right, out RH, AlcanceVerif))
			{
				if(RH.transform.tag == TagTerreno)
				{
					return false;
				}
			}
			break;
			
		case Lado.Izq:
			if(Physics.Raycast(transform.position, transform.right * (-1), out RH, AlcanceVerif))
			{
				if(RH.transform.tag == TagTerreno)
				{
					return false;
				}
			}
			break;
		}
		
		return true;
	}	
	
	void Doblar()
	{
		Girando = true;
		//escoje un lado
		Lado lado;
		if((int)Random.Range(0,2) == 0)
		{
			lado = TaxiComp.Lado.Izq;
			//verifica, si no da cambia a derecha
			if(!VerificarCostado(lado))
				lado = TaxiComp.Lado.Der;
		}
		else
		{
			lado = TaxiComp.Lado.Der;
			//verifica, si no da cambia a izq
			if(!VerificarCostado(lado))
				lado = TaxiComp.Lado.Izq;
		}
		
		
		if(lado == TaxiComp.Lado.Der)
		{
			Vector3 vaux = transform.localEulerAngles;
			vaux.y += AngDeGiro;
			transform.localEulerAngles = vaux;
		}
		else
		{
			Vector3 vaux = transform.localEulerAngles;
			vaux.y -= AngDeGiro;
			transform.localEulerAngles = vaux;
		}
	}
	
	void DejarDoblar()
	{
		Girando = false;
		TiempEntreGiro = (float) Random.Range(TiempCadaCuantoDobla_MaxMin.x, TiempCadaCuantoDobla_MaxMin.y);
		
		transform.localEulerAngles = RotIni;
	}
	
	void Respawn()
	{
		Respawneando = false;
		
		transform.position = PosIni;
		transform.localEulerAngles = RotIni;
	}
	
	bool Medicion()
	{
		const int distMinAlCuadrado = 4 * 4;

		//Si algun player está más cerca que la distancia minima, la medicion falló
		for (int i = 0; i < players.Length; i++)
			if ((players[i].position - PosIni).sqrMagnitude < distMinAlCuadrado)
				return false;

		//Si ninguno está cerca, funcionó
		return true;
	}
}