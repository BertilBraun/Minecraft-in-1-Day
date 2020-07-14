using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Guid id;
    public string username;


    public void Initialize(Guid _id, string _username)
    {
        id = _id;
        username = _username;
    }

}
