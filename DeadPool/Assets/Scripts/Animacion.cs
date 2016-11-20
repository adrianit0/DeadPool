using UnityEngine;
using System.Collections.Generic;

public enum CONDICIONAL { IgualQue, MayorQue, MayorIgualQue, MenorQue, MenorIgualQue, DistintoQue}
public enum TERMINAR { Repetir, NoSeguir, EmpezarOtra, ActualizarSolo };

public class Animacion : ScriptableObject {
    public AnimacionVariable variables = new AnimacionVariable();
    public AnimacionClip[] animaciones = new AnimacionClip[0];


    //AHORA ESTO SE HA VACIADO
}



[System.Serializable]
public class AnimacionClip {
    public string nombre = "";
    public AnimacionCondicion[] condiciones = new AnimacionCondicion[0];
    public bool predeterminada = false;
    public TERMINAR terminar = TERMINAR.Repetir;
    public int otra = 0;

    //Ajustes avanzados
    public bool repetir = false;

    public Sprite[] sprites = new Sprite[0];
    public int fps = 2;
}

[System.Serializable]
public class AnimacionCondicion {
    public string nombreCondicion = "";
    public float valor = 0;

    public CONDICIONAL condicional = CONDICIONAL.IgualQue;
}
