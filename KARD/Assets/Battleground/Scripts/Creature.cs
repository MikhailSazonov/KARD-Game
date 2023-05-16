using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.IO;
using System;

public class Creature : MonoBehaviour
{
    readonly int[] dx = {0, 1, 0, -1};
    readonly int[] dy = {1, 0, -1, 0};

    const int WIDTH = 15;
    const int HEIGHT = 7;

    public bool big;

    [HideInInspector]
    public int x;

    [HideInInspector]
    public int y;

    public CreatureGameParams cr_params;

    public float speed = 0.5f;

    [HideInInspector]
    public int ownerPlayer;

    [HideInInspector]
    public PhotonView photonView;

    GridManager manager;

    List<Tuple<int, int>> way;

    public Creature(GridManager man)
    {
        manager = man;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Move(Tuple<int, int> next)
    {
        int xn = next.Item1;
        int yn = next.Item2;
        Vector3 position = transform.position;
        if (xn > x)
        {
            position.x += speed;
        }
        else if (xn < x)
        {
            position.x -= speed;
        }
        else if (yn > y)
        {
            position.y += speed;
        }
        else if (yn < y)
        {
            position.y -= speed;
        }
        transform.position = position;
    }

    bool SamePosition(Tuple<int, int> pos)
    {
        int xp = pos.Item1;
        int yp = pos.Item2;
        float x_real = manager.tiles_coords[xp][yp].x;
        float y_real = manager.tiles_coords[xp][yp].x;
        return x_real == transform.position.x && y_real == transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {   
        if (way != null)
        {
            var next_cell = way[0];
            Move(next_cell);
            if (SamePosition(next_cell))
            {
                way.RemoveAt(0);
                if (way.Count == 0)
                {
                    way = null;
                    manager.FinishTurn();
                    return;
                }
            }
        }
    }

    List<Tuple<int, int>> bfsFoundAvailable(int x, int y, Dictionary<int, Dictionary<int, bool>> free_tiles, int xto, int yto)
    {
        Dictionary<int, Dictionary<int, Tuple<int, int>>> where_from = new Dictionary<int, Dictionary<int, Tuple<int, int>>>();
        for (int i = 0; i < WIDTH; ++i)
        {
            where_from.Add(i, new Dictionary<int, Tuple<int, int>>());
            for (int j = 0; j < HEIGHT; ++j)
            {
                where_from[i].Add(j, new Tuple<int, int>(-1, -1));
            }
        }

        Queue<Tuple<int, int, int>> q = new Queue<Tuple<int, int, int>>();
        q.Enqueue(new Tuple<int, int, int>(x, y, 0));
        while (q.Count > 0)
        {
            Tuple<int, int, int> next = q.Dequeue();
            int xc = next.Item1;
            int yc = next.Item2;
            if (xc == xto && yc == yto)
            {
                break;
            }
            int d = next.Item3;
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    if (d + 1 <= speed && free_tiles[xc + dx[i]][yc + dy[j]] == true && where_from[xc + dx[i]][yc + dy[j]] == Tuple.Create(-1, -1))
                    {
                        q.Enqueue(new Tuple<int, int, int>(xc + dx[i], yc + dy[j], d + 1));
                        where_from[i][j] = Tuple.Create(xc, yc);
                    }
                }
            }
        }

        if (where_from[xto][yto] == Tuple.Create(-1, -1))
        {
            return null;
        }
        List<Tuple<int, int>> path = new List<Tuple<int, int>>();
        Tuple<int, int> idx = new Tuple<int, int>(xto, yto);
        while (idx.Item1 != x && idx.Item2 != y)
        {
            path.Add(idx);
            int xi = where_from[idx.Item1][idx.Item2].Item1;
            int yi = where_from[idx.Item1][idx.Item2].Item2;
            idx = Tuple.Create(xi, yi);
        }
        return path;
    }

    [PunRPC]
    public void Go(int xto, int yto)
    {
        var path = bfsFoundAvailable(x, y, manager.free_tiles, xto, yto);
        if (path == null)
        {
            return;
        }
        var animator = GetComponent<Animator>();
        animator.SetTrigger("Move");
        way = path;
    }
}


public class Hero : Creature {
    public Hero(GridManager man) : base(man)
    {}
}
