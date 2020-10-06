using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InteractOnGridObject : InteractiveObject {   
    [Header ("Events")]
    public UnityEvent m_OnEnterTile, m_OnExitTile;

    protected override void OnEnterTileMethod(GridObject gridObject, GridTile gridTile) {
        m_OnEnterTile.Invoke();
    }

    private IEnumerator Exit()
    {
        yield return new WaitForSeconds(.1f);
        m_OnExitTile.Invoke();
    }

    protected override void OnExitTileMethod(GridObject gridObject, GridTile gridTile)
    {
        StartCoroutine(Exit());
    }
}
