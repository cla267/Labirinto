using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public static class Player
{
    public static Color color;
    public static int playerNumber;

    public static void SetColor(Color inputColor)
    {
        color = inputColor;
    }

    public static void SetPlayerNumber(int number)
    {
        playerNumber = number;
    }
}
