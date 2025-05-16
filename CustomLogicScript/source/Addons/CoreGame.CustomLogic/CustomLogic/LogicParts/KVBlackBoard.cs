using System.Collections.Generic;
using UnityEngine;

namespace CoreGame.Custom
{
    public class KVBlackBoard
{
    protected Dictionary<string, int> _intValues = new Dictionary<string, int>();
    protected Dictionary<string, float> _floatValues = new Dictionary<string, float>();
    protected Dictionary<string, string> _stringValues = new Dictionary<string, string>();
    protected Dictionary<string, bool> _boolValues = new Dictionary<string, bool>();
    protected Dictionary<string, object> _objectValues = new Dictionary<string, object>();
    protected Dictionary<string, Vector3> _vectorValues = new Dictionary<string, Vector3>();

    public bool IsInPool = false;

    public KVBlackBoard()
    {
    }
    
    public void Clear()
    {
        _intValues.Clear();
        _floatValues.Clear();
        _stringValues.Clear();
        _boolValues.Clear();
        _objectValues.Clear();
        _vectorValues.Clear();
    }

    public void SetInt(string key, int value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        _intValues[key] = value;

    }

    public int GetInt(string key, int defaultValue = 0)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_intValues.TryGetValue(key, out var value))
        {
            return value;
        }
        value = defaultValue;
        return value;
    }

    public void AddInt(string key, int value, int defaultValue = 0)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        var oldValue = GetInt(key, defaultValue);
        _intValues[key] = oldValue + value;
    }
    
    public void MinusInt(string key, int value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        
        if(_intValues.TryGetValue(key, out var oldValue))
        {
            _intValues[key] = oldValue - value;
        }
    }

    public void SetFloat(string key, float value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        _floatValues[key] = value;

    }

    public float GetFloat(string key, float defaultValue = 0)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_floatValues.TryGetValue(key, out var value))
        {
            return value;
        }
        value = defaultValue;
        return value;
    }

    public bool GetFloat(string key, out float value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        return _floatValues.TryGetValue(key, out value);
    }

    public void AddFloat(string key, float value, float defaultValue = 0)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        var oldValue = GetFloat(key, defaultValue);
        _floatValues[key] = oldValue + value;
    }
    
    public void MinusFloat(string key, float value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if(_floatValues.TryGetValue(key, out var oldValue))
        {
            _floatValues[key] = oldValue - value;
        }
    }

    public string SetString(string key, string value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        _stringValues[key] = value;
        return value;
    }

    public string GetString(string key, string defaultValue = "")
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_stringValues.TryGetValue(key, out var value))
        {
            return value;
        }
        value = defaultValue;
        return value;
    }

    public void SetBool(string key, bool value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        _boolValues[key] = value;

    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_boolValues.TryGetValue(key, out var value))
        {
            return value;
        }
        value = defaultValue;
        return value;
    }

    public void SetObject(string key, object value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        _objectValues[key] = value;

    }

    public object GetObject(string key, object defaultValue = null)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_objectValues.TryGetValue(key, out var value))
        {
            return value;
        }
        value = defaultValue;
        return value;
    }
    public T GetObject<T>(string key) where T : class
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_objectValues.TryGetValue(key, out var value))
        {
            return value as T;
        }
        return null;
    }


    public T GetObjectSafe<T>(string key, bool autoAdd = false) where T : new()
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_objectValues.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        value = new T();
        if (autoAdd)
        {
            _objectValues.Add(key, value);
        }
        return (T)value;
    }

    public void SetVector(string key, Vector3 value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        _vectorValues[key] = value;

    }

    public Vector3 GetVector(string key)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_vectorValues.TryGetValue(key, out var value))
        {
            return value;
        }
        value = Vector3.zero;
        return value;
    }

    public Vector3 GetVector(string key, Vector3 defaultValue)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_vectorValues.TryGetValue(key, out var value))
        {
            return value;
        }
        value = defaultValue;
        return value;
    }

    public bool GetVector(string key, out Vector3 value)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        return _vectorValues.TryGetValue(key, out value);
    }
    
    public bool HasKey(string key)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_intValues.ContainsKey(key))
            return true;
        if (_floatValues.ContainsKey(key))
            return true;
        if (_stringValues.ContainsKey(key))
            return true;
        if (_boolValues.ContainsKey(key))
            return true;
        if (_objectValues.ContainsKey(key))
            return true;
        if (_vectorValues.ContainsKey(key))
            return true;
        return false;
    }

    public void RemoveKey(string key)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (_intValues.ContainsKey(key))
        {
            _intValues.Remove(key);
        }
        if (_floatValues.ContainsKey(key))
        {
            _floatValues.Remove(key);
        }
        if (_stringValues.ContainsKey(key))
        {
            _stringValues.Remove(key);
        }
        if (_boolValues.ContainsKey(key))
        {
            _boolValues.Remove(key);
        }
        if (_objectValues.ContainsKey(key))
        {
            _objectValues.Remove(key);
        }
        if (_vectorValues.ContainsKey(key))
        {
            _vectorValues.Remove(key);
        }
    }

    public void Copy(KVBlackBoard target, bool overlap = false)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        if (target == null)
        {
            return;
        }
        foreach (var item in target._intValues)
        {
            if (overlap)
                _intValues[item.Key] = item.Value;
            else
                _intValues.Add(item.Key, item.Value);
        }
        foreach (var item in target._floatValues)
        {
            if (overlap)
                _floatValues[item.Key] = item.Value;
            else
                _floatValues.Add(item.Key, item.Value);
        }
        foreach (var item in target._stringValues)
        {
            if (overlap)
                _stringValues[item.Key] = item.Value;
            else
                _stringValues.Add(item.Key, item.Value);
        }
        foreach (var item in target._boolValues)
        {
            if (overlap)
                _boolValues[item.Key] = item.Value;
            else
                _boolValues.Add(item.Key, item.Value);
        }
        foreach (var item in target._objectValues)
        {
            if (overlap)
                _objectValues[item.Key] = item.Value;
            else
                _objectValues.Add(item.Key, item.Value);
        }
        foreach (var item in target._vectorValues)
        {
            if (overlap)
                _vectorValues[item.Key] = item.Value;
            else
                _vectorValues.Add(item.Key, item.Value);
        }
    }


    public void CopyFloat(KVBlackBoard target, string key)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        SetFloat(key, target.GetFloat(key));
    }

    public void CopyBool(KVBlackBoard target, string key)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        SetBool(key, target.GetBool(key));
    }
    public void CopyInt(KVBlackBoard target, string key)
    {
        if (IsInPool)
        {
            LogWrapper.LogError("错误的使用了池子中的资源");
        }
        SetInt(key, target.GetInt(key));
    }
}
}
