using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlayerPool 
{
    private static PlayerPool _instance;

    private Random random;

    private Dictionary<int, Player> _playerPool = new Dictionary<int, Player>();

    public static PlayerPool GetInstance()
    {
        if (_instance == null)
        {
            _instance = new PlayerPool();
            _instance.random = new System.Random(1000); 
        }
            
        return _instance;
    }

    public void UpsertData(int id, Player obj)
    {
        GetInstance()._playerPool[id] = obj;
    }

    public void ResetDataExcept(int id)
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

    public void ResetData(int id)
    {
        _playerPool[id].MoveController.takeOver = false;
    }

    public int GetSelectedId()
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
            int pos = random.Next(0, keys.Length);
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
