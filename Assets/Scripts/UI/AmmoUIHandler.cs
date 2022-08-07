using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoUIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI magazineText;



    public void UpdateAmmoCountText(int value)
    {
        ammoText.text = value.ToString();
    }

    public void UpdateMagazineCountText(int value)
    {
        magazineText.text = value.ToString();
    }
}
