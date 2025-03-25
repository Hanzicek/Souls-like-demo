using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Transform weaponSocket;  // Assign this in the Inspector
    public GameObject swordPrefab;  // Drag your sword FBX prefab here

    private GameObject equippedSword;

    void Start()
    {
        EquipWeapon();
    }

    void EquipWeapon()
    {
        if (swordPrefab != null && weaponSocket != null)
        {
            equippedSword = Instantiate(swordPrefab, weaponSocket.position, weaponSocket.rotation);
            equippedSword.transform.SetParent(weaponSocket);
        }
    }
}
