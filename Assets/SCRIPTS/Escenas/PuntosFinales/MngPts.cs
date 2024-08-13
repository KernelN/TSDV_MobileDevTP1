using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MngPts : MonoBehaviour
{
    Rect R = new Rect();

    public float TiempEmpAnims = 2.5f;
    float Tempo = 0;

    public Sprite[] Ganadores;
    
    public Image Ganador;
    public GameObject[] PanelesDinero;
    public TextMeshProUGUI[] TextosDinero;

    public float TiempEspReiniciar = 10;


    public float TiempParpadeo = 0.7f;
    float TempoParpadeo = 0;
    bool PrimerImaParp = true;

    public bool ActivadoAnims = false;

    Visualizacion Viz = new Visualizacion();

    //---------------------------------//

    // Use this for initialization
    void Start()
    {
        SetGanador();
    }

    // Update is called once per frame
    void Update()
    {
        //PARA JUGAR
        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(0);
        }

        //CIERRA LA APLICACION
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        TiempEspReiniciar -= Time.deltaTime;
        if (TiempEspReiniciar <= 0)
        {
            SceneManager.LoadScene(0);
        }


        if (ActivadoAnims)
        {
            TempoParpadeo += Time.deltaTime;

            if (TempoParpadeo >= TiempParpadeo)
            {
                TempoParpadeo = 0;

                if (!PrimerImaParp)
                    TempoParpadeo += 0.1f;
                
                PrimerImaParp = !PrimerImaParp;
                PanelesDinero[(int)DatosPartida.LadoGanadaor].SetActive(!PrimerImaParp);
            }
        }
        else
        {
            Tempo += Time.deltaTime;
            if (Tempo >= TiempEmpAnims)
            {
                Tempo = 0;
                ActivadoAnims = true;
                
                //Prender las cajas de dinero
                for (int i = 0; i < PanelesDinero.Length; i++)
                    PanelesDinero[i].SetActive(true);

                //Prender imagen del ganador
                Ganador.gameObject.SetActive(true);
            }
        }
    }

    //---------------------------------//


    void SetGanador()
    {
        switch (DatosPartida.LadoGanadaor)
        {
            case DatosPartida.Lados.Der:
                Ganador.sprite = Ganadores[1];
                TextosDinero[1].text = "$" + Viz.PrepararNumeros(DatosPartida.PtsGanador);
                TextosDinero[0].text = "$" + Viz.PrepararNumeros(DatosPartida.PtsPerdedor);
                break;

            case DatosPartida.Lados.Izq:
                Ganador.sprite = Ganadores[0];
                TextosDinero[0].text = "$" + Viz.PrepararNumeros(DatosPartida.PtsGanador);
                TextosDinero[1].text = "$" + Viz.PrepararNumeros(DatosPartida.PtsPerdedor);
                break;
        }
    }

    public void DesaparecerGUI()
    {
        ActivadoAnims = false;
        Tempo = -100;
    }
}