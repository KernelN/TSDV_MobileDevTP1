using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Brinks;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instancia;

    public float TiempoDeJuego = 60;

    public enum EstadoJuego
    {
        Calibrando,
        Jugando,
        Finalizado
    }

    public EstadoJuego EstAct = EstadoJuego.Calibrando;

    public Player Player1;
    public Player Player2;
    public bool DosJugadores => Player2 != null;

    bool ConteoRedresivo = true;
    public Rect ConteoPosEsc;
    public float ConteoParaInicion = 3;
    public TextMeshProUGUI ConteoInicio;
    public TextMeshProUGUI TiempoDeJuegoText;

    public float TiempEspMuestraPts = 3;

    //posiciones de los camiones dependientes del lado que les toco en la pantalla
    //la pos 0 es para la izquierda y la 1 para la derecha
    public Vector3[] PosCamionesCarrera = new Vector3[2];

    //posiciones de los camiones para el tutorial
    public Vector3 PosCamion1Tuto = Vector3.zero;
    public Vector3 PosCamion2Tuto = Vector3.zero;

    //listas de GO que activa y desactiva por sub-escena
    //escena de tutorial
    public GameObject[] ObjsCalibracion1;

    public GameObject[] ObjsCalibracion2;

    //la pista de carreras
    public GameObject[] ObjsCarrera;

    //--------------------------------------------------------//

    void Awake()
    {
        if (Instancia)
        {
            Destroy(this);
            return;
        }

        Instancia = this;
    }
    IEnumerator Start()
    {
        InputManager.inst.has2Players = DosJugadores;
        yield return null;
        IniciarTutorial();
    }
    void Update()
    {
        //REINICIAR
        if (Input.GetKey(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //CIERRA LA APLICACION
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.inst.QuitGame();
        }

        switch (EstAct)
        {
            case EstadoJuego.Calibrando:
                if (InputManager.inst.Axis1.y > 0)
                    Player1.Seleccionado = true;
                if (DosJugadores && InputManager.inst.Axis2.y > 0)
                    Player2.Seleccionado = true;
                break;
            case EstadoJuego.Jugando:

                //SKIP LA CARRERA
                if (Input.GetKey(KeyCode.Alpha9))
                {
                    TiempoDeJuego = 0;
                }

                if (TiempoDeJuego <= 0)
                {
                    FinalizarCarrera();
                }

                if (ConteoRedresivo)
                {
                    ConteoParaInicion -= T.GetDT();
                    if (ConteoParaInicion < 0)
                    {
                        EmpezarCarrera();
                        ConteoRedresivo = false;
                    }
                }
                else
                {
                    //baja el tiempo del juego
                    TiempoDeJuego -= T.GetDT();
                }

                if (ConteoRedresivo)
                {
                    if (ConteoParaInicion > 1)
                    {
                        ConteoInicio.text = ConteoParaInicion.ToString("0");
                    }
                    else
                    {
                        ConteoInicio.text = "GO";
                    }
                }

                ConteoInicio.gameObject.SetActive(ConteoRedresivo);

                TiempoDeJuegoText.text = TiempoDeJuego.ToString("00");

                break;

            case EstadoJuego.Finalizado:

                //muestra el puntaje

                TiempEspMuestraPts -= Time.deltaTime;
                if (TiempEspMuestraPts <= 0)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

                break;
        }

        TiempoDeJuegoText.transform.parent.gameObject.SetActive(EstAct == EstadoJuego.Jugando && !ConteoRedresivo);
    }
    void OnDestroy()
    {
        if (Instancia == this)
            Instancia = null;
    }

    //----------------------------------------------------------//

    public void IniciarTutorial()
    {
        for (int i = 0; i < ObjsCalibracion1.Length; i++)
        {
            ObjsCalibracion1[i].SetActive(true);
            if(DosJugadores)
                ObjsCalibracion2[i].SetActive(true);
        }

        for (int i = 0; i < ObjsCarrera.Length; i++)
        {
            ObjsCarrera[i].SetActive(false);
        }

        Player1.CambiarATutorial();
        if(DosJugadores)
            Player2.CambiarATutorial();

        TiempoDeJuegoText.transform.parent.gameObject.SetActive(false);
        ConteoInicio.gameObject.SetActive(false);
    }

    void EmpezarCarrera()
    {
        Player1.GetComponent<Frenado>().RestaurarVel();
        Player1.GetComponent<ControlDireccion>().Habilitado = true;

        if(!DosJugadores) return;
        
        Player2.GetComponent<Frenado>().RestaurarVel();
        Player2.GetComponent<ControlDireccion>().Habilitado = true;
    }

    void FinalizarCarrera()
    {
        EstAct = GameplayManager.EstadoJuego.Finalizado;

        TiempoDeJuego = 0;

        if (DosJugadores)
        {
            if (Player1.Dinero > Player2.Dinero)
            {
                //lado que gano
                if (Player1.LadoActual == Visualizacion.Lado.Der)
                    DatosPartida.LadoGanador = DatosPartida.Lados.Der;
                else
                    DatosPartida.LadoGanador = DatosPartida.Lados.Izq;
                //puntajes
                DatosPartida.PtsGanador = Player1.Dinero;
                DatosPartida.PtsPerdedor = Player2.Dinero;
            }
            else
            {
                //lado que gano
                if (Player2.LadoActual == Visualizacion.Lado.Der)
                    DatosPartida.LadoGanador = DatosPartida.Lados.Der;
                else
                    DatosPartida.LadoGanador = DatosPartida.Lados.Izq;

                //puntajes
                DatosPartida.PtsGanador = Player2.Dinero;
                DatosPartida.PtsPerdedor = Player1.Dinero;
            }
        }
        else
        {
            DatosPartida.PtsGanador = Player1.Dinero;
            DatosPartida.LadoGanador = DatosPartida.Lados.Solo;
        }

        Player1.GetComponent<Frenado>().Frenar();
        Player1.ContrDesc.FinDelJuego();

        if (DosJugadores)
        {
            Player2.GetComponent<Frenado>().Frenar();
            Player2.ContrDesc.FinDelJuego();
        }
    }

    //se encarga de posicionar la camara derecha para el jugador que esta a la derecha y viseversa
    //void SetPosicion(PlayerInfo pjInf) {
    //    pjInf.PJ.GetComponent<Visualizacion>().SetLado(pjInf.LadoAct);
    //    //en este momento, solo la primera vez, deberia setear la otra camara asi no se superponen
    //    pjInf.PJ.ContrCalib.IniciarTesteo();
    //
    //
    //    if (pjInf.PJ == Player1) {
    //        if (pjInf.LadoAct == Visualizacion.Lado.Izq)
    //            Player2.GetComponent<Visualizacion>().SetLado(Visualizacion.Lado.Der);
    //        else
    //            Player2.GetComponent<Visualizacion>().SetLado(Visualizacion.Lado.Izq);
    //    }
    //    else {
    //        if (pjInf.LadoAct == Visualizacion.Lado.Izq)
    //            Player1.GetComponent<Visualizacion>().SetLado(Visualizacion.Lado.Der);
    //        else
    //            Player1.GetComponent<Visualizacion>().SetLado(Visualizacion.Lado.Izq);
    //    }
    //
    //}

    //cambia a modo de carrera
    void CambiarACarrera()
    {
        EstAct = GameplayManager.EstadoJuego.Jugando;

        for (int i = 0; i < ObjsCarrera.Length; i++)
        {
            ObjsCarrera[i].SetActive(true);
        }

        //desactivacion de la calibracion
        Player1.FinCalibrado = true;

        for (int i = 0; i < ObjsCalibracion1.Length; i++)
        {
            ObjsCalibracion1[i].SetActive(false);
        }

        if (DosJugadores)
        {
            Player2.FinCalibrado = true;
            for (int i = 0; i < ObjsCalibracion2.Length; i++)
                ObjsCalibracion2[i].SetActive(false);
        }

        Player1.gameObject.transform.position = PosCamionesCarrera[0];

        Player1.transform.forward = Vector3.forward;
        Player1.GetComponent<Frenado>().Frenar();
        Player1.CambiarAConduccion();

        Player1.GetComponent<Frenado>().RestaurarVel();
        Player1.GetComponent<ControlDireccion>().Habilitado = false;
        Player1.transform.forward = Vector3.forward;

        TiempoDeJuegoText.transform.parent.gameObject.SetActive(false);
        ConteoInicio.gameObject.SetActive(false);

        if (!DosJugadores) return;

        //posiciona los camiones dependiendo de que lado de la pantalla esten
        if (Player1.LadoActual == Visualizacion.Lado.Izq)
        {
            Player1.gameObject.transform.position = PosCamionesCarrera[0];
            Player2.gameObject.transform.position = PosCamionesCarrera[1];
        }
        else
        {
            Player1.gameObject.transform.position = PosCamionesCarrera[1];
            Player2.gameObject.transform.position = PosCamionesCarrera[0];
        }

        Player2.transform.forward = Vector3.forward;
        Player2.GetComponent<Frenado>().Frenar();
        Player2.CambiarAConduccion();

        //los deja andando
        Player2.GetComponent<Frenado>().RestaurarVel();
        //cancela la direccion
        Player2.GetComponent<ControlDireccion>().Habilitado = false;
        //les de direccion
        Player2.transform.forward = Vector3.forward;
    }

    public void FinCalibracion(int playerID)
    {
        if (playerID == 0)
            Player1.FinTuto = true;

        if (!DosJugadores)
        {
            if(Player1.FinTuto) CambiarACarrera();
            return;
        }
        
        if (playerID == 1)
            Player2.FinTuto = true;

        if (Player1.FinTuto && Player2.FinTuto)
            CambiarACarrera();
    }
}