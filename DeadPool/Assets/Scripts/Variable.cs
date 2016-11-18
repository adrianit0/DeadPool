using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class AnimacionVariable {
    public float this [string param] {
        set {
            for(int i = 0; i < Count; i++) {
                if(keys[i].Equals(param)) {
                    values[i] = value;
                }
            }
        }
        get {
            for (int i = 0; i < Count; i++) {
                if (keys[i].Equals(param)) {
                    return values[i];
                }
            }
            return 0;
        }
    }

    public string this [int param] {
        set {
            keys[param] = value;
        }
        get {
            return keys[param];
        }
    }

    public List<string> keys;
    public List<float> values;
    public int Count {
        set { }
        get { return keys.Count; }
    }

    public AnimacionVariable () {
        keys = new List<string>();
        values = new List<float>();
    }

    public void Add (string _key, float _value) {
        keys.Add(_key);
        values.Add(_value);
    }

    public void Remove (string _key) {
        for (int i = 0; i < Count; i++) {
            if (keys[i].Equals(_key)) {
                RemoveAt(i);
                return;
            }
        }
    }

    public void RemoveAt (int index) {
        keys.RemoveAt(index);
        values.RemoveAt(index);
    }

    public bool ContainsKey (string key) {
        for(int i = 0; i < Count; i++) {
            if(keys[i].Equals(key)) {
                return true;
            }
        }
        return false;
    }

    public string[] GetList () {
        string[] newList = new string[keys.Count];

        for (int i = 0; i < newList.Length; i++) {
            newList[i] = keys[i];
        }

        return newList;
    }

    public string[] GetListWithEmptyCase() {
        string[] newList = new string[keys.Count+1];

        newList[0] = "Null";
        for(int i = 1; i < newList.Length; i++) {
            newList[i] = keys[i-1];
        }

        return newList;
    }

    public float GetValue (string _key) {
        for (int i = 0; i<keys.Count; i++) {
            if (keys[i].Equals(_key)) {
                return values[i];
            }
        }
        Debug.LogWarning("Key no encontrada.");
        return 0;
    }
}
