public class DatosPartida 
{
	public enum Lados{ Izq, Der, Solo}
	public enum Dificultad { Facil, Normal, Dificil }

	public static float TiempoDeJuego;
	public static Lados LadoGanador;
	public static int PtsGanador;
	public static int PtsPerdedor;
	public static Dificultad DificultadJuego = Dificultad.Facil;
}