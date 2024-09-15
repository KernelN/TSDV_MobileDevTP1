using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletMover : ManejoPallets
{
    public bool isPlayer1 = true;
    public ManejoPallets Desde, Hasta;
    int paso = 1;

    private void Update() 
    {
        Vector2 axis = isPlayer1 ? InputManager.inst.Axis1 : InputManager.inst.Axis2;
        
        if(axis.sqrMagnitude == 0) return;
        
        bool tenencia = Tenencia();

        switch (paso)
        {
            case 1:
                if (!tenencia && Desde.Tenencia() && axis.x < 0)
                    PrimerPaso();
                break;
            case 2:
                if (tenencia && axis.y < 0)
                    SegundoPaso();
                break;
            case 3:
                if (tenencia && axis.x > 0)
                    TercerPaso();
                break;
        }
    }

    void PrimerPaso() {
        Desde.Dar(this);
        paso++;
    }
    void SegundoPaso() {
        base.Pallets[0].transform.position = transform.position;
        paso++;
    }
    void TercerPaso() {
        Dar(Hasta);
        paso = 1;
    }

    public override void Dar(ManejoPallets receptor) {
        if (Tenencia()) {
            if (receptor.Recibir(Pallets[0])) {
                Pallets.RemoveAt(0);
            }
        }
    }
    public override bool Recibir(Pallet pallet) {
        if (!Tenencia()) {
            pallet.Portador = this.gameObject;
            base.Recibir(pallet);
            return true;
        }
        else
            return false;
    }
}
