using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Brinks.Gameplay
{
    public class MoneyManager : MonoBehaviour
    {
        [System.Serializable]
        struct BagZone
        {
            [Header("Set Values")]
            public Transform[] spawnPos;
            [Header("Runtime Values")]
            public bool[] free;
        }
        
        //[Header("Set Values")]
        [FormerlySerializedAs("easyMaxZoneOffset")]
        [SerializeField] int maxZoneOffset = 2;
        [SerializeField] BagZone[] spawnPosByZone;
        [SerializeField] Bolsa[] moneyBags;
        //[Header("Runtime Values")]
        List<Bolsa> inactiveBags = new List<Bolsa>();
        List<Bolsa> activeBags = new List<Bolsa>();
        ///x = zone, y = bag
        List<Vector2Int> activeBagsZone = new List<Vector2Int>();

        //Unity Events
        void Start()
        {
            for (int i = 0; i < spawnPosByZone.Length; i++)
            {
                spawnPosByZone[i].free = new bool[spawnPosByZone[i].spawnPos.Length];
                for (int j = 0; j < spawnPosByZone[i].free.Length; j++)
                    spawnPosByZone[i].free[j] = true;
            }
            
            activeBags = new List<Bolsa>();
            activeBagsZone = new List<Vector2Int>();
            
            inactiveBags = new List<Bolsa>();
            
            for (int i = 0; i < moneyBags.Length; i++)
            {
                moneyBags[i].Desaparecer(true); //disable it, just in case
                
                //If easy|normal, use all spawn points
                //If hard, use every other spawn point
                switch (DatosPartida.DificultadJuego)
                {
                    case DatosPartida.Dificultad.Facil: break;
                    case DatosPartida.Dificultad.Dificil:
                        if(i % 2 == 0) continue;
                        break;
                }
                
                inactiveBags.Add(moneyBags[i]);

                //Update pools when bags are picked
                int index = i;
                moneyBags[i].Activada += (isActive) =>
                {
                    if (isActive)
                    {
                        activeBags.Add(moneyBags[index]);
                        inactiveBags.Remove(moneyBags[index]);
                    }
                    else
                    {
                        inactiveBags.Add(moneyBags[index]);
                        int tempIndex = activeBags.IndexOf(moneyBags[index]);
                        activeBagsZone.RemoveAt(tempIndex);
                        activeBags.RemoveAt(tempIndex);
                    }
                };
                
                //Now that bag is linked to object pool, spawn it
                TrySpawnBag();
            }

            //Remove old bags on Zone Update & add new ones to the newest zone
            ZoneManager.inst.ZoneRangeUpdated += () =>
            {
                int oldestZone = ZoneManager.inst.zoneRange.x;
                
                //Check all active bags
                for (int i = 0; i < activeBags.Count; i++)
                {
                    //If there are more bags after this index
                    //AND current bag is in an older zone than the oldest zone
                    //Remove it
                    while(activeBagsZone.Count > i && activeBagsZone[i].x < oldestZone)
                        activeBags[i].Desaparecer(true);
                }

                int newestZone = ZoneManager.inst.zoneRange.y;
                int maxZone = newestZone+maxZoneOffset;
                
                if(newestZone > spawnPosByZone.Length)
                    newestZone = spawnPosByZone.Length;
                if (maxZone > spawnPosByZone.Length)
                    maxZone = spawnPosByZone.Length;
                
                //Try to spawn bags for each new zone
                for (int i = newestZone; i <= maxZone; i++)
                    for (int j = 0; j < spawnPosByZone[i].spawnPos.Length; j++)
                        ForceSpawnBag();
            };
        }

        //Methods
        void TrySpawnBag()
        {
            if (inactiveBags.Count > 0) 
                SpawnBag();
        }
        void ForceSpawnBag()
        {
            if(inactiveBags.Count == 0 && activeBags.Count > 0)
                activeBags[0].Desaparecer(true);

            SpawnBag();
        }
        void SpawnBag()
        {
            Bolsa bag = inactiveBags[0];

            //Get zone range, taking offset into account
            Vector2Int zR = ZoneManager.inst.zoneRange;
            zR.y += maxZoneOffset;

            //Run through all zones in range
            for (int i = zR.x; i <= zR.y && i < spawnPosByZone.Length; i++)
                //Run through all spawn pos in zone
                for (int j = 0; j < spawnPosByZone[i].spawnPos.Length; j++)
                    //If spawn pos is free, spawn bag
                    if (spawnPosByZone[i].free[j])
                    {
                        bag.transform.position = spawnPosByZone[i].spawnPos[j].position;
                        spawnPosByZone[i].free[j] = false;

                        activeBagsZone.Add(new Vector2Int(i, j));
                        bag.Aparecer();
                        return;
                    }
        }
    }
}