using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public static UpdateManager updateManager;
    public List<IUpdate> Updates = new List<IUpdate>();

    private void Awake()
    {
        updateManager = this;
        Updates.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Updates.Count; i++)
        {
            Updates[i].FastUpdate();
        }
    }
}
