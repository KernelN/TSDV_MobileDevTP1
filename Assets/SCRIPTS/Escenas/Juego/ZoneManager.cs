using System.Collections.Generic;
using UnityEngine;

namespace Brinks.Gameplay
{
    public class ZoneManager : MonoBehaviour
    {
        public static ZoneManager inst;
        
        [Header("Set Values")]
        [SerializeField] List<Transform> checkpoints;
        [SerializeField, Min(0)] float checkpointRadius;
        [SerializeField, Min(0)] float updateCheckpointInterval = 1;
        //[Header("Runtime Values")]
        Transform[] players;
        int[] playerCurrentCheckpoints;
        float timer;
        [Header("DEBUG")]
        [SerializeField] bool drawGizmos;
        [SerializeField] Transform checkpointsGizmoHolder;
        [SerializeField] Color gizmosColor = Color.Lerp(Color.clear, Color.yellow, .5f);

        public System.Action ZoneRangeUpdated;
        
        public Dictionary<int, Transform> playersCpByHash { get; private set; }
        public Vector2Int zoneRange { get; private set; }
        
        float SqrCpRadius => checkpointRadius * checkpointRadius;

        //Unity Events
        void Awake()
        {
            if (inst != null)
            {
                Destroy(this);
                return;
            }
            
            inst = this;
            
            zoneRange = new Vector2Int(0, 1);
            playersCpByHash = new Dictionary<int, Transform>();
        }
        void Start()
        {
            GameplayManager gm = GameplayManager.Instancia;

            if (gm.DosJugadores)
                players = new[] { gm.Player1.transform, gm.Player2.transform };
            else
                players = new[] { gm.Player1.transform };
            
            playerCurrentCheckpoints = new int[players.Length];
            for (int i = 0; i < players.Length; i++)
            {
                playerCurrentCheckpoints[i] = 0;
                playersCpByHash.Add(players[i].GetHashCode(), checkpoints[0]);
            }
        }
        void Update()
        {
            timer += T.GetDT();
            if(timer < updateCheckpointInterval) return;
            
            timer = 0;
            
            float sqrDist;
            List<Transform> players = new List<Transform>(this.players);
            Vector2Int newZoneRange = new Vector2Int(int.MaxValue, zoneRange.y);
            
            //Update players checkpoint;
            for (int i = zoneRange.x; i < checkpoints.Count && players.Count > 0; i++)
            {
                int j = 0;
                while (players.Count > 0 && j < players.Count)
                {
                    //Get dist between player and checkpoint
                    sqrDist = (players[j].position - checkpoints[i].position).sqrMagnitude;

                    //If player is close enough, set checkpoint and remove from list
                    if (sqrDist < SqrCpRadius)
                    {
                        playersCpByHash[players[j].GetHashCode()] = checkpoints[i];
                        playerCurrentCheckpoints[j] = i;
                        players.RemoveAt(j);
                        
                        //if checkpoint is higher than zone range, update zone range
                        if (i >= newZoneRange.y) newZoneRange.y = i+1;
                    }

                    //Else, go to next player
                    else j++;
                }
            }

            //Get oldest checkpoint
            for (int i = 0; i < this.players.Length; i++)
            {
                if(playerCurrentCheckpoints[i] <= zoneRange.x) 
                    newZoneRange.x = playerCurrentCheckpoints[i];
            }
            
            if(newZoneRange.x > checkpoints.Count)
                newZoneRange.x = zoneRange.x > checkpoints.Count ? 0 : zoneRange.x;
            
            //Update zone range
            if (newZoneRange.x != zoneRange.x || newZoneRange.y != zoneRange.y)
            {
                zoneRange = newZoneRange;
                ZoneRangeUpdated?.Invoke();
            }
        }
        void OnDrawGizmos()
        {
            if(!drawGizmos) return;
            
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.color = gizmosColor;

            Transform[] checkpoints;
            if(checkpointsGizmoHolder)
                checkpoints = checkpointsGizmoHolder.GetComponentsInChildren<Transform>();
            else
                checkpoints = this.checkpoints.ToArray();
            
            Vector3 gizmoSize = checkpointRadius * new Vector3(1,.1f,1);
            for (int i = 0; i < checkpoints.Length; i++)
            {
                Gizmos.matrix = Matrix4x4.TRS(checkpoints[i].position, 
                                                checkpoints[i].rotation, gizmoSize);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
            
            Gizmos.matrix = oldMatrix;
        }
        void OnDestroy()
        {
            if (inst == this) inst = null;
        }

        //Methods
    }
}