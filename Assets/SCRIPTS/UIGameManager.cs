using System;
using UnityEngine;

namespace Brinks
{
    public class UIGameManager : MonoBehaviour
    {
        //[Header("Set Values")]
        //[Header("Runtime Values")]
        GameManager inst;

        //Unity Events
        void Start()
        {
            inst = GameManager.inst;
        }

        //Methods
        public void QuitGame() => inst.QuitGame();
        public void SetDifficulty(int dif) => inst.SetDifficulty((DatosPartida.Dificultad)dif);
    }
}
