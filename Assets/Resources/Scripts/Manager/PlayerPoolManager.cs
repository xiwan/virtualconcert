using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class PlayerPoolManager : Single<PlayerPoolManager>
{

    private Dictionary<int, Player> _playerPool = new Dictionary<int, Player>();

    public void UpsertData(int id, Player obj)
    {
        this._playerPool[id] = obj;
    }

    public void ResetDataExcept(int id)
    {
        foreach (KeyValuePair<int, Player> keyVal in _playerPool)
        {
            if (keyVal.Key != id)
            {
                ResetData(keyVal.Key);
            }
        }
    }

    public void ResetData(int id)
    {
        //_playerPool[id].moveController.takeOver = false;
        _playerPool[id].takeOver = false;
    }

    public int GetSelectedId()
    {
        foreach (KeyValuePair<int, Player> keyVal in _playerPool)
        {
            if (_playerPool[keyVal.Key] != null)
            {
                if (_playerPool[keyVal.Key].takeOver)
                {
                    return keyVal.Key;
                }
            }
        }
        return 0;
    }

    public Player GetPlayer(int playerId)
    {
        return _playerPool[playerId];
    }

    public Player GetAnyPlayer()
    {
        if (_playerPool.Count > 0)
        {
            int[] keys = _playerPool.Keys.ToArray();
            int pos = MathHelper.GetRandom(0, keys.Length);
            //Debug.Log("return pos: " + pos);
            return _playerPool[keys[pos]];
        }
        return null;
    }

    public int CountPlayer()
    {
        return _playerPool.Count;
    }
}