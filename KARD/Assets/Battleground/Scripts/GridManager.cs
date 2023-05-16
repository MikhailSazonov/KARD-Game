using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class GridManager : MonoBehaviour
{
    int WIDTH = 15;
    int HEIGHT = 7;

    public struct TileCoords {
        public float x;
        public float y;
    }

    [SerializeField] private Tile _tilePrefab;

    [HideInInspector] public TileCoords coords;
    [HideInInspector] public bool chosenTileChange;

    [HideInInspector] public List<List<TileCoords>> tiles_coords;
    [HideInInspector] public Dictionary<int, Dictionary<int, bool>> free_tiles;

    Dictionary<int, PlayerData> data;
    [HideInInspector] public InitiativeManager init_manager;

    [SerializeField] private HeroHuman human;

    [HideInInspector] public int turn;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        tiles_coords = new List<List<TileCoords>>();
        photonView = GetComponent<PhotonView>();
        data = new Dictionary<int, PlayerData>();
        data[PhotonNetwork.PlayerList[0].ActorNumber].player_params = new PlayerData.CreaturesParams(2, 0f);
        data[PhotonNetwork.PlayerList[1].ActorNumber].player_params = new PlayerData.CreaturesParams(12, 180f);
        if (photonView == null) {
            Debug.LogError("PhotonView component not found on game object!");
        }
        GenerateGrid();
        chosenTileChange = false;
        Spawn("HeroHuman", data[PhotonNetwork.LocalPlayer.ActorNumber].player_params.StartX, 3,
        data[PhotonNetwork.LocalPlayer.ActorNumber].player_params.RotateY);
    }


    // Update is called once per frame
    void Update()
    {
        if (chosenTileChange) {
            Debug.Log(string.Format("Grid clicked! Coordinates: {0} {1}", coords.x, coords.y));
            chosenTileChange = false;
        }  
    }

    void GenerateGrid()
    {
        for (int x = 0; x < WIDTH; ++x) {
            tiles_coords.Add(new List<TileCoords>());
            free_tiles.Add(x, new Dictionary<int, bool>());
            for (int y = 0; y < HEIGHT; ++y) {
                tiles_coords[x].Add(new TileCoords{x = x * 1.2f + transform.position.x, y = y * 0.6f + transform.position.y});
                free_tiles[x].Add(y, false);
                var spawnedTile = Instantiate(_tilePrefab,
                            new Vector3(tiles_coords[x][y].x, tiles_coords[x][y].y, 0), 
                            Quaternion.Euler(60f, 0f, 0f));
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.gridManager = this;
                spawnedTile.x = x;
                spawnedTile.y = y;
            }
        }
    }

    public void Spawn(string creatureName, int tile_x, int tile_y, int player)
    {
        human = PhotonNetwork.Instantiate(creatureName,
            new Vector3(tiles_coords[tile_x][tile_y].x, tiles_coords[tile_x][tile_y].y + 0.8f, 0),
            Quaternion.Euler(0f, data[player].player_params.RotateY, 0f)).GetComponent<HeroHuman>();
    }

    public void Go(int x, int y)
    {
        var creature = init_manager.getCreature();
        if (creature.ownerPlayer != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        creature.photonView.RPC("Go", RpcTarget.All, x, y);
    }

    public void FinishTurn()
    {
        init_manager.TurnEnd();
    }
}
