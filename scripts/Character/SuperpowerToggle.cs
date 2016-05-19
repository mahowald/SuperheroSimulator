using UnityEngine;
using System.Collections;

public class SuperpowerToggle : MonoBehaviour {

    public SuperPower power;

    private bool hasPower = false;

    public bool HasPower
    {
        get { return hasPower; }
        set
        {
            hasPower = value;
            SetPower(value);

        }
    }

    private void SetPower(bool value)
    {
        if(value == true)
        {
            GlobalData.playerData.powers.Add(power);
        }
        if (value == false)
        {
            GlobalData.playerData.powers.Remove(power);
        }
    }

}
