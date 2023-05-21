using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.IO;
using System;

public enum Type {
    ORDINARY,
    HERO
}

public class Creature : MonoBehaviour, IComparable<Creature>
{
    readonly int[] dx = {0, 1, 0, -1};
    readonly int[] dy = {1, 0, -1, 0};

    const int WIDTH = 15;
    const int HEIGHT = 7;

    public bool big;

    public string creatureName;
    public Type type = Type.ORDINARY;

    [HideInInspector]
    public int x;

    [HideInInspector]
    public int y;

    public float speed_x;
    public float speed_y;

    [HideInInspector]
    public int ownerPlayer;

    [HideInInspector]
    public PhotonView photonView;

    [HideInInspector]
    public GridManager manager;

    List<Tuple<int, int>> way;
    [HideInInspector] public Creature creature_to_hit;

    // CREATURE PARAMS
    public int hp;
    public int damage;
    public int attack;
    public int defense;
    public int distance;
    public int shots;
    public int initiative;
    public int manacost;
    public List<Effect> effects;

    public int id;

    public float bonus_y;

    [HideInInspector]
    public int currentHp;

    [HideInInspector]
    public int currentDamage;

    [HideInInspector]
    public int currentAttack;

    [HideInInspector]
    public int currentDefense;

    [HideInInspector]
    public int currentDistance;

    [HideInInspector]
    public int currentShots;

    [HideInInspector]
    public int currentInitiative;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = hp;
        currentDamage = damage;
        currentAttack = attack;
        currentDefense = defense;
        currentDistance = distance;
        currentShots = shots;
        currentInitiative = initiative;
        effects = new List<Effect>();
        animator = GetComponent<Animator>();
        way = null;
    }

    // Comparison is based on:
    // 1. Initiative
    // 2. If equal - on the id
    public int CompareTo(Creature other)
    {
        if (currentInitiative != other.currentInitiative) {
            return currentInitiative.CompareTo(other.currentInitiative);
        }
        return id.CompareTo(other.id);
    }

    void Turn(int xto, int x)
    {
        int pl = ownerPlayer;
        float current_rotation_y = GridManager.gridManagerSingletone.data[pl].player_params.RotateY;
        Quaternion rotation = transform.rotation;
        if (current_rotation_y == 0.0f && xto < x)
        {
            rotation.y = 180.0f;
        }
        else if (current_rotation_y == 180.0f && xto > x)
        {
            rotation.y = 0.0f;
        }
        transform.rotation = rotation;
    }

    void TurnBack()
    {
        int pl  = ownerPlayer;
        float base_rotation_y = GridManager.gridManagerSingletone.data[pl].player_params.RotateY;
        Quaternion rotation = transform.rotation;
        rotation.y = base_rotation_y;
        transform.rotation = rotation;
    }

    void Move(Tuple<int, int> next)
    {
        float s_x = speed_x;
        float s_y = speed_y;

        if (ownerPlayer == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            s_x *= 2.0f;
            s_y *= 2.0f;
        }

        int xn = next.Item1;
        int yn = next.Item2;
        Vector3 position = transform.position;
        float x_real_next = GridManager.gridManagerSingletone.tiles_info[xn][yn].x;
        float y_real_next = GridManager.gridManagerSingletone.tiles_info[xn][yn].y + bonus_y;

        float x_shift = Math.Min(Math.Abs(x_real_next - position.x), speed_x);
        float y_shift = Math.Min(Math.Abs(y_real_next - position.y), speed_y);

        if (position.x < x_real_next)
        {
            position.x += x_shift;
        }
        else if (position.x > x_real_next)
        {
            position.x -= x_shift;
        }
        else if (position.y < y_real_next)
        {
            position.y += y_shift;
        }
        else if (position.y > y_real_next)
        {
            position.y -= y_shift;
        }
        position.z = position.y + 6.0f;
        transform.position = position;
    }

    bool SamePosition(Tuple<int, int> pos)
    {
        int xp = pos.Item1;
        int yp = pos.Item2;
        float x_real = GridManager.gridManagerSingletone.tiles_info[xp][yp].x;
        float y_real = GridManager.gridManagerSingletone.tiles_info[xp][yp].y + bonus_y;
        return Math.Abs(x_real - transform.position.x) < 1e-9 && Math.Abs(y_real - transform.position.y) < 1e-9;
    }

    // Update is called once per frame
    void Update()
    {
        bool stayed = false;
        if (way != null)
        {
            var next_cell = way[0];
            Turn(next_cell.Item1, x);
            Move(next_cell);
            if (SamePosition(next_cell))
            {
                x = next_cell.Item1;
                y = next_cell.Item2;
                way.RemoveAt(0);
                if (way.Count == 0)
                {
                    way = null;
                    animator.SetTrigger("Stay");
                    stayed = true;
                    if (creature_to_hit == null)
                    {
                        TurnBack();
                        if (ownerPlayer != PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            GridManager.gridManagerSingletone.FinishTurn();
                        }
                        return;
                    }
                }
            }
        }
        if (stayed && creature_to_hit != null)
        {
            // Turn(creature_to_hit.x, x);
            Attack(creature_to_hit);
            creature_to_hit = null;
            TurnBack();
            if (ownerPlayer != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                GridManager.gridManagerSingletone.FinishTurn();
            }
        }
    }

    List<Tuple<int, int>> bfsFoundAvailable(int xto, int yto)
    {
        var tiles_info = GridManager.gridManagerSingletone.tiles_info;
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
                if (xc + dx[i] < 0 || xc + dx[i] >= WIDTH || yc + dy[i] < 0 || yc + dy[i] >= HEIGHT)
                {
                    continue;
                }
                if (d + 1 <= distance && tiles_info[xc + dx[i]][yc + dy[i]].standing == null && where_from[xc + dx[i]][yc + dy[i]].Item1 == -1)
                {
                    q.Enqueue(new Tuple<int, int, int>(xc + dx[i], yc + dy[i], d + 1));
                    where_from[xc + dx[i]][yc + dy[i]] = Tuple.Create(xc, yc);
                }
            }
        }

        if (where_from[xto][yto].Item1 == -1)
        {
            return null;
        }
        List<Tuple<int, int>> path = new List<Tuple<int, int>>();
        Tuple<int, int> idx = new Tuple<int, int>(xto, yto);
        while (!(idx.Item1 == x && idx.Item2 == y))
        {
            path.Add(idx);
            int xi = where_from[idx.Item1][idx.Item2].Item1;
            int yi = where_from[idx.Item1][idx.Item2].Item2;
            idx = Tuple.Create(xi, yi);
        }
        path.Reverse();
        return path;
    }

    Dictionary<int, Dictionary<int, Tuple<int, int>>> bfsFoundIfCanGo()
    {
        var tiles_info = GridManager.gridManagerSingletone.tiles_info;
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
            int d = next.Item3;
            for (int i = 0; i < 4; ++i)
            {
                if (xc + dx[i] < 0 || xc + dx[i] >= WIDTH || yc + dy[i] < 0 || yc + dy[i] >= HEIGHT)
                {
                    continue;
                }
                if (d + 1 <= distance && tiles_info[xc + dx[i]][yc + dy[i]].standing == null && where_from[xc + dx[i]][yc + dy[i]].Item1 == -1)
                {
                    q.Enqueue(new Tuple<int, int, int>(xc + dx[i], yc + dy[i], d + 1));
                    where_from[xc + dx[i]][yc + dy[i]] = Tuple.Create(xc, yc);
                }
            }
        }

        return where_from;
    }

    
    public bool Go(int xto, int yto)
    {
        var path = bfsFoundAvailable(xto, yto);
        if (path == null)
        {
            return false;
        }

        GridManager.TileInfo info1 = new GridManager.TileInfo();
        info1.x = GridManager.gridManagerSingletone.tiles_info[x][y].x;
        info1.y = GridManager.gridManagerSingletone.tiles_info[x][y].y;
        info1.standing = null;
        info1.dead = GridManager.gridManagerSingletone.tiles_info[x][y].dead;

        GridManager.TileInfo info2 = new GridManager.TileInfo();
        info2.x = GridManager.gridManagerSingletone.tiles_info[xto][yto].x;
        info2.y = GridManager.gridManagerSingletone.tiles_info[xto][yto].y;
        info2.standing = this;
        info2.dead = GridManager.gridManagerSingletone.tiles_info[x][y].dead;

        GridManager.gridManagerSingletone.tiles_info[x][y] = info1;
        GridManager.gridManagerSingletone.tiles_info[xto][yto] = info2;

        // Turn(xto, x);

        // x = xto;
        // y = yto;
        animator.SetTrigger("Move");
        way = path;

        return true;
    }

    public virtual void Attack(Creature other)
    {
        double add = (currentAttack - other.currentDefense) * 1.2;
        double dmg = Math.Max(currentDamage * 0.2, currentDamage + add);
        animator.SetTrigger("Hit");
        other.TakeDamage(dmg);
    }

    public virtual void TakeDamage(double dmg)
    {
        currentHp -= (int)dmg;
        Debug.Log(string.Format("Hit taken! current hp: {0}", currentHp));
        if (currentHp <= 0)
        {
            currentHp = 0;
            animator.SetTrigger("Die");
            GridManager.gridManagerSingletone.DieCallback(this);
        }
        else
        {
            animator.SetTrigger("TakeHit");
        }
    }

    public static void Recolor(int x_, int y_)
    {
        var color = GridManager.gridManagerSingletone.tiles[x_][y_].GetComponent<Renderer>().material.color;
        color.r *= 0.7f;
        color.g *= 0.7f;
        color.b *= 0.7f;
        GridManager.gridManagerSingletone.tiles[x_][y_].GetComponent<Renderer>().material.color = color;
    }

    void SetYellow(int x_, int y_)
    {
        var color = new Color(1.0f, 1.0f, 0.5f, GridManager.gridManagerSingletone.tiles[x_][y_].GetComponent<Renderer>().material.color.a);
        GridManager.gridManagerSingletone.tiles[x_][y_].GetComponent<Renderer>().material.color = color;
    }

    public void LightenTheCells()
    {
        var where_can_go = bfsFoundIfCanGo();
        for (int i_x = 0; i_x < WIDTH; ++i_x)
        {
            for (int j_y = 0; j_y < HEIGHT; ++j_y)
            {
                if (where_can_go[i_x][j_y].Item1 != -1)
                {
                    Recolor(i_x, j_y);
                }
            }
        }
        SetYellow(x, y);
    }
}
