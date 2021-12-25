using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPool 
{
    private static PlayerPool _instance;

    private Dictionary<int, Player> _playerPool = new Dictionary<int, Player>();

    public static PlayerPool getInstance()
    {
        if (_instance == null)
            _instance = new PlayerPool();
        return _instance;
    }

    public void upsertData(int id, Player obj)
    {
        getInstance()._playerPool[id] = obj;
    }

    public void resetDataExcept(int id)
    {
        foreach(KeyValuePair<int, Player> keyVal in _playerPool)
        {            
            if (keyVal.Key != id)
            {
                _playerPool[keyVal.Key].MoveController.takeOver = false;
                //_playerPool[keyVal.Key].TakeOver = false;
            }
        }
    }

    public void resetData(int id)
    {
        _playerPool[id].MoveController.takeOver = false;
    }

    public int getSelectedId()
    {
        foreach(KeyValuePair<int, Player> keyVal in _playerPool)
        {
            if (_playerPool[keyVal.Key] != null)
            {
                if (_playerPool[keyVal.Key].TakeOver)
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
            int pos = UnityEngine.Random.Range(0, keys.Length-1);
            Debug.Log("return pos: " + pos);
            return _playerPool[keys[pos]];
        }
        return null;
    }

}
