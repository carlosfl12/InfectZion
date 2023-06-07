using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class WeaponHandler : NetworkBehaviour
{
    public GameObject[] weapons;
    public int selectedWeapon = 0;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        weapons = GameObject.FindGameObjectsWithTag("Weapon");
        SelectMainWeapon();
        player = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        int previousWeapon = selectedWeapon;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {

            if (selectedWeapon >= transform.childCount - 1) {
                selectedWeapon = 0;
            } else {
                selectedWeapon++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {

            if (selectedWeapon <= 0) {
                selectedWeapon = transform.childCount -1 ;
            } else {
                selectedWeapon--;
            }
        }

        if (previousWeapon != selectedWeapon) {
            SelectMainWeapon();
        }

        if (weapons[selectedWeapon].name == "Axe(Clone)" && Input.GetMouseButtonDown(0)) {
            player._currentState.Value = Player.PlayerState.MeleeWeapon;
        } else {
            if (Input.GetMouseButton(1) && weapons[selectedWeapon].name == "AK(Clone)") {
                // player._currentState.Value = Player.PlayerState.DistanceWeapon;
                if (IsOwner) {
                    player.ChangePlayerStateServerRpc(Player.PlayerState.DistanceWeapon);
                }
            } else {
                return;
            }
        }

        player.currentWeapon = weapons[selectedWeapon];

    }

    // 
    



    void SelectMainWeapon() {
        int i = 0;

        foreach (Transform weapon in transform) {
            if (i == selectedWeapon) {
                weapon.gameObject.SetActive(true);
            }
            else {
                weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }
}
