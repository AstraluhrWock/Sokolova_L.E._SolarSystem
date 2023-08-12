using UnityEngine;
using UnityEngine.Networking;

internal class StarCollector : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnStarCollectedChanged))]
    private int _countOfStar = 0;

    [Command]
    public void CmdCollectStar()
    {
        _countOfStar++;
        Debug.Log($"{ _countOfStar}");
    }

    private void OnStarCollectedChanged(int value)
    {
        _countOfStar = value;
    }

    public int GetCountOfStar()
    {
        return _countOfStar;
    }
}

