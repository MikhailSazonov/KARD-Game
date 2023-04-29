using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GridManager : MonoBehaviour
{
    public struct TileCoords {
        public float x;
        public float y;
    }

    struct CreaturesParams {
        public int StartX;
        public float RotateY;

        public CreaturesParams(int X, float Y)
        {
            StartX = X;
            RotateY = Y;
        }
    }

    [SerializeField] private Tile _tilePrefab;

    [HideInInspector] public TileCoords coords;
    [HideInInspector] public bool chosenTileChange;

    private Dictionary<int, CreaturesParams> player_params;

    private List<List<TileCoords>> tiles_coords;
    private List<List<bool>> busy_tiles;

    Dictionary<int, List<Creature>> players_creatures;

    [SerializeField] private HeroHuman human;

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        tiles_coords = new List<List<TileCoords>>();
        busy_tiles = new List<List<bool>>();
        photonView = GetComponent<PhotonView>();
        player_params = new Dictionary<int, CreaturesParams>();
        player_params[PhotonNetwork.PlayerList[0].ActorNumber] = new CreaturesParams(2, 0f);
        player_params[PhotonNetwork.PlayerList[1].ActorNumber] = new CreaturesParams(12, 180f);
        // PhotonNetwork.ConnectUsingSettings();
        if (photonView == null) {
            Debug.LogError("PhotonView component not found on game object!");
        }
        GenerateGrid();
        chosenTileChange = false;
        Spawn("HeroHuman", player_params[PhotonNetwork.LocalPlayer.ActorNumber].StartX, 3,
        player_params[PhotonNetwork.LocalPlayer.ActorNumber].RotateY);
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
        int width = 15;
        int height = 7;
        for (int x = 0; x < width; ++x) {
            tiles_coords.Add(new List<TileCoords>());
            busy_tiles.Add(new List<bool>());
            for (int y = 0; y < height; ++y) {
                tiles_coords[x].Add(new TileCoords{x = x * 1.2f + transform.position.x, y = y * 0.6f + transform.position.y});
                busy_tiles[x].Add(false);
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

    void Spawn(string creatureName, int tile_x, int tile_y, float rotateY)
    {
        human = PhotonNetwork.Instantiate(creatureName,
            new Vector3(tiles_coords[tile_x][tile_y].x, tiles_coords[tile_x][tile_y].y + 0.8f, 0),
            Quaternion.Euler(0f, rotateY, 0f)).GetComponent<HeroHuman>();
    }

    public void Go(int x, int y)
    {
        // Animator animator = human.GetComponent<Animator>();
        // animator.SetTrigger("Move");
        photonView.RPC("GoRPC", RpcTarget.All, x, y);
    }

    [PunRPC]
    public void GoRPC(int x, int y)
    {
        Animator animator = human.GetComponent<Animator>();
        animator.SetTrigger("Move");
    }
}
