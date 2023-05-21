using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tile : MonoBehaviour
{
    public GridManager gridManager;
    
    [HideInInspector] public int x;
    [HideInInspector] public int y;

    int cntr = 0;
    bool toCnt = false;

    private bool isMouseOver = false;

    private void OnMouseOver()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (toCnt)
        {
            ++cntr;
            var gm = GridManager.gridManagerSingletone;
            Creature cur_creature = gm.init_manager.getCreature();
            if (cntr == 130)
            {
                var pl = PhotonNetwork.LocalPlayer.ActorNumber;
                var crName = "Races/Undead/Skeleton/Skeleton";
                gm.SpawnLocal(crName,
                x, y, pl);
                cur_creature.animator.SetTrigger("Cast");
                int cnt = gm.tiles_info[x][y].dead.Count;
                var res_creature = gm.tiles_info[x][y].dead[cnt - 1];
                Destroy(res_creature.gameObject);
                gm.tiles_info[x][y].dead.RemoveAt(cnt - 1);
                var photonView = gm.GetComponent<PhotonView>();
                photonView.RPC("Spawn", RpcTarget.Others, crName, x, y, pl);
                toCnt = false;
            }
            return;
        }
        if (isMouseOver && Input.GetMouseButtonUp(1))
        {
            var gm = GridManager.gridManagerSingletone;
            Creature cur_creature = gm.init_manager.getCreature();
            if (GridManager.gridManagerSingletone.MyTurn() && cur_creature.creatureName == "UndeadHero" &&
                    gm.tiles_info[x][y].dead.Count > 0)
            {
                toCnt = true;
            }
        }
    }

    void OnMouseDown()
    {
        if (!GridManager.gridManagerSingletone.MyTurn())
        {
            return;
        }
        if (DeckManager.deckManagerSingletone.numClicked >= 0 &&
            !DeckManager.deckManagerSingletone.played)
        {
            if (!DeckManager.cardClicked.Play(x, y))
            {
                return;
            }
            DeckManager.deckManagerSingletone.played = true;
        }
        else
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gridManager.Go(x, y, mousePosition.x, mousePosition.y);
        }
    }

}
