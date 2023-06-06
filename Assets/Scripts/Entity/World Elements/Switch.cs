﻿using UnityEngine;
using UnityEngine.Tilemaps;

using Photon.Pun;
using NSMB.Utils;

public class Switch : MonoBehaviour
{
    public Vector2 spawnother;
        private double bumpTime;
        public void OnTriggerEnter2D(Collider2D collision)
        {
            Vector3Int tileLocation = Utils.WorldToTilemapPosition(transform.position);

            if (PhotonNetwork.Time - bumpTime < 0)
                return;

            if (Utils.GetTileAtTileLocation(tileLocation) != null)
                return;

            if (collision.gameObject.GetComponent<PlayerController>() is not PlayerController player)
                return;

            if (!player.photonView.IsMine)
                return;

            Rigidbody2D body = collision.attachedRigidbody;
            if (player.previousFrameVelocity.y <= 0)
                return;

            BoxCollider2D bc = collision as BoxCollider2D;
            if (bc == null)
                return;
            if (body.position.y + (bc.size.y * body.transform.lossyScale.y) - (player.previousFrameVelocity.y * Time.fixedDeltaTime) > transform.position.y)
                return;

            DoBump(tileLocation, collision.gameObject.GetPhotonView());
            bumpTime = PhotonNetwork.Time + 0.25d;
            collision.attachedRigidbody.velocity = new(body.velocity.x, 0);
        }

        public void DoBump(Vector3Int tileLocation, PhotonView player)
        {
            player.RPC(nameof(PlayerController.AttemptCollectCoin), RpcTarget.All, -1, (Vector2)Utils.TilemapToWorldPosition(tileLocation) + Vector2.one / 4f);

            object[] parametersBump = new object[] { spawnother.x, spawnother.y, false, "SpecialTiles/EmptyYellowQuestion", "" };
            GameManager.Instance.SendAndExecuteEvent(Enums.NetEventIds.SetThenBumpTile, parametersBump, ExitGames.Client.Photon.SendOptions.SendReliable);
        }
    }
