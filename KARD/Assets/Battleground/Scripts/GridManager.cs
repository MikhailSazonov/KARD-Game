using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Threading;

public class GridManager : MonoBehaviour
{
    Color tileColor = new Color(0, 0, 0);

    static public int WIDTH = 15;
    static public int HEIGHT = 7;

    public struct TileInfo {
        public float x;
        public float y;
        public Creature standing;
        public List<Creature> dead;
    }


    [SerializeField] private Tile _tilePrefab;
    [HideInInspector] public Declarations declarations;

    static public GridManager gridManagerSingletone;
    private PhotonView photonView;

    [HideInInspector] public List<List<TileInfo>> tiles_info = new List<List<TileInfo>>();
    [HideInInspector] public List<List<Tile>> tiles = new List<List<Tile>>();

    [HideInInspector] public InitiativeManager init_manager = new InitiativeManager();
    [HideInInspector] public TurnManager turn_manager = new TurnManager();

    public List<Creature> heroes = new List<Creature>();

    [HideInInspector] public Dictionary<int, PlayerData> data = new Dictionary<int, PlayerData>();

    // Start is called before the first frame update
    void Start()
    {
        declarations.Disable();
        photonView = GetComponent<PhotonView>();
        gridManagerSingletone = this;

        int pl1 = PhotonNetwork.PlayerList[0].ActorNumber;
        int pl2 = PhotonNetwork.PlayerList[1].ActorNumber; 

        for (int i = 0; i < 3; ++i)
        {
            heroes.Add(null);
        }

        data.Add(pl1, new PlayerData(2, 0f));
        data.Add(pl2, new PlayerData(12, 180f));

        if (photonView == null) {
            Debug.LogError("PhotonView component not found on game object!");
        }

        GenerateGrid();

        Debug.Log(string.Format("You are player {0}", PhotonNetwork.LocalPlayer.ActorNumber));

        DeckManager.Init();

        SpawnLocal(DeckManager.hero_name,
        data[PhotonNetwork.LocalPlayer.ActorNumber].player_params.StartX, 3, PhotonNetwork.LocalPlayer.ActorNumber);

        photonView.RPC("Spawn", RpcTarget.Others, DeckManager.hero_name, 
        data[PhotonNetwork.LocalPlayer.ActorNumber].player_params.StartX, 3, PhotonNetwork.LocalPlayer.ActorNumber);
    }


    // Update is called once per frame
    void Update()
    {
    }

    void GenerateGrid()
    {
        for (int x = 0; x < WIDTH; ++x) {
            tiles_info.Add(new List<TileInfo>());
            tiles.Add(new List<Tile>());
            for (int y = 0; y < HEIGHT; ++y) {
                tiles_info[x].Add(new TileInfo{x = x * 1.2f + transform.position.x, y = y * 0.6f + transform.position.y,
                standing = null, dead = new List<Creature>()});
                var spawnedTile = Instantiate(_tilePrefab,
                            new Vector3(tiles_info[x][y].x, tiles_info[x][y].y, tiles_info[x][y].y + 6), 
                            Quaternion.Euler(60f, 0f, 0f));
                if (tileColor.r == 0.0f)
                {
                    tileColor = spawnedTile.GetComponent<Renderer>().material.color;
                }
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.gridManager = this;
                spawnedTile.x = x;
                spawnedTile.y = y;
                tiles[x].Add(spawnedTile);
            }
        }
    }

    Tuple<int, int> Convert(int x, int y, float m_x, float m_y)
    {
        int[] dx = {0, 1, 1, 1, 0, -1, -1, -1};
        int[] dy = {1, 1, 0, -1, -1, -1, 0, 1};
        float min_dist_sq = 10000;
        Tuple<int, int> ans = null;
        for (int i = 0; i < 8; ++i)
        {
            int x_p = x + dx[i];
            int y_p = y + dy[i]; 
            if (x_p >= 0 && x_p < WIDTH && y_p >= 0 && y_p < HEIGHT && tiles_info[x_p][y_p].standing == null)
            {
                float x_r = tiles_info[x_p][y_p].x + 0.6f;
                float y_r = tiles_info[x_p][y_p].y + 0.3f;
                float new_dist_sq = (x_r - m_x) * (x_r - m_x) + (y_r - m_y) * (y_r - m_y);
                if (new_dist_sq < min_dist_sq)
                {
                    min_dist_sq = new_dist_sq;
                    ans = Tuple.Create(x_p, y_p);
                }
            }
        }
        return ans;
    }

    public bool MyTurn()
    {
        if (!init_manager.Ready())
        {
            return false;
        }
        Creature creature = init_manager.getCreature();
        if (declarations.isEnd() || creature.ownerPlayer != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return false;
        }
        return true;
    }

    public void Go(int x, int y, float m_x, float m_y)
    {
        Creature creature = init_manager.getCreature();
        if (tiles_info[x][y].standing == null)
        {
            if (TurnManagerSetActionLocal(x, y, -1, -1))
            {
                photonView.RPC("TurnManagerSetAction", RpcTarget.Others, x, y, -1, -1, turn_manager.cntAdd);
                FinishTurn();
            }
        }
        else if (tiles_info[x][y].standing.ownerPlayer != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Tuple<int, int> converted = Convert(x, y, m_x, m_y);
            if (converted != null)
            {
                int g_x = converted.Item1;
                int g_y = converted.Item2;
                if (TurnManagerSetActionLocal(g_x, g_y, x, y))
                {
                    photonView.RPC("TurnManagerSetAction", RpcTarget.Others, g_x, g_y, x, y, turn_manager.cntAdd);
                    FinishTurn();
                }
            }
        }
    }

    public void FinishTurn()
    {
        init_manager.TurnEnd();
    }

    void Place(string creatureName, int tile_x, int tile_y, int player)
    {
        Creature creature = ((GameObject)Instantiate(Resources.Load(creatureName),
        new Vector3(tiles_info[tile_x][tile_y].x, tiles_info[tile_x][tile_y].y, tiles_info[tile_x][tile_y].y + 6.0f),
        Quaternion.Euler(0f, data[player].player_params.RotateY, 0f))).GetComponent<Creature>();

        Vector3 tr = creature.transform.position;
        tr.y += creature.bonus_y;
        tr.z += creature.bonus_y;
        creature.transform.position = tr;
        
        creature.x = tile_x;
        creature.y = tile_y;

        TileInfo info1 = new TileInfo();
        info1.x = tiles_info[tile_x][tile_y].x;
        info1.y = tiles_info[tile_x][tile_y].y;
        info1.standing = creature;
        info1.dead = tiles_info[tile_x][tile_y].dead;

        tiles_info[tile_x][tile_y] = info1;

        creature.ownerPlayer = player;
        init_manager.AddSummoned(creature);
    }

    public bool SpawnLocal(string creatureName, int tile_x, int tile_y, int player)
    {
        if (tiles_info[tile_x][tile_y].standing != null)
        {
            return false;
        }

        turn_manager.cntAdd = 1;

        Place(creatureName, tile_x, tile_y, player);

        SetBackTheColor();
        if (init_manager.getCreature() != null)
        {
            init_manager.getCreature().LightenTheCells();
        }
        
        return true;
    }

    [PunRPC]
    public void Spawn(string creatureName, int tile_x, int tile_y, int player)
    {
        bool ready = init_manager.Ready();

        if (tiles_info[tile_x][tile_y].dead.Count > 0)
        {
            int cnt = tiles_info[tile_x][tile_y].dead.Count;
            var res_creature = tiles_info[tile_x][tile_y].dead[cnt - 1];
            Destroy(res_creature.gameObject);
        }

        Place(creatureName, tile_x, tile_y, player);

        if (!ready)
        {
            init_manager.Init();
        }
        else
        {
            turn_manager.execution.incCounter();
        }
    }

    public bool TurnManagerSetActionLocal(int x, int y, int x_a, int y_a)
    {
        if (declarations.isEnd())
        {
            return false;
        }
        if (x_a == -1)
        {
            return turn_manager.setAction(x, y, null, turn_manager.cntAdd);
        }
        else
        {
            return turn_manager.setAction(x, y, tiles_info[x_a][y_a].standing, turn_manager.cntAdd);
        }
    }

    [PunRPC]
    public void TurnManagerSetAction(int x, int y, int x_a, int y_a, int addCnt)
    {
        if (x_a == -1)
        {
            turn_manager.setAction(x, y, null, addCnt);
        }
        else
        {
            turn_manager.setAction(x, y, tiles_info[x_a][y_a].standing, addCnt);
        }
    }

    public void DieCallback(Creature creature)
    {
        var info = tiles_info[creature.x][creature.y];
        info.standing = null;
        if (info.dead == null)
        {
            info.dead = new List<Creature>();
        }
        info.dead.Add(creature);
        tiles_info[creature.x][creature.y] = info;

        Vector3 position = creature.transform.position;
        position.z = -8;

        if (creature.type == Type.HERO)
        {
            if (creature.ownerPlayer == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                declarations.Enable("Defeat!");   
            }
            else
            {
                declarations.Enable("Victory!");
            }
        }
        else
        {
            init_manager.RemoveSummoned(creature);
        }
    }

    public void SetBackTheColor()
    {
        for (int i_x = 0; i_x < WIDTH; ++i_x)
        {
            for (int j_y = 0; j_y < HEIGHT; ++j_y)
            {
                tiles[i_x][j_y].GetComponent<Renderer>().material.color = tileColor;
            }
        }
    }
}
