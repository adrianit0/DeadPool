using UnityEngine;
using System.Collections.Generic;

public enum CONDICIONAL { IgualQue, MayorQue, MayorIgualQue, MenorQue, MenorIgualQue, DistintoQue}
public enum TERMINAR { Repetir, NoSeguir, EmpezarOtra };

public class Animacion : MonoBehaviour {
    public Dictionary<string, float> variables = new Dictionary<string, float>();
    public AnimacionClip[] animaciones = new AnimacionClip[0];

    SpriteRenderer render;

    int actual = 0;
    int step = 0, stepMaximo = 2;
    bool parar = false;
    float tiempoActual = 0, tiempoTotal = 0;

    void Awake () {
        render = GetComponent<SpriteRenderer>();
        if (render == null) {
            Debug.LogError("Renderizador en el objeto "+name+" no encontrado");
        }
	}

    void Start () {
        SetAnimation(0);
    }
	
	void Update () {
        if(render == null || parar)
            return;

        tiempoActual += Time.deltaTime;

        if (tiempoActual>tiempoTotal) {
            tiempoActual -= tiempoTotal;

            render.sprite = animaciones[actual].sprites[step];
            step++;

            if(step == stepMaximo) {
                switch(animaciones[actual].terminar) {
                    case TERMINAR.Repetir:
                        step = 0;
                        break;
                    case TERMINAR.EmpezarOtra:
                        SetAnimation(animaciones[actual].otra);
                        break;
                    case TERMINAR.NoSeguir:
                        parar = true;
                        break;
                }
            }
        }
	}

    public void SetFloat (string texto, float cantidad) {
        if (variables.ContainsKey (texto)) {
            variables[texto] = cantidad;
        } else {
            Debug.LogWarning("Variable " + texto + " no existe.");
        }
    }

    //Cambia de una animación a otra
    public void SetAnimation (int id) {
        actual = id;
        step = 0;
        stepMaximo = animaciones[id].sprites.Length;
        tiempoTotal = 1 / (float) animaciones[id].fps;
        parar = false;
    }

    public void ActualizarCondiciones () {
        for (int anim = 0; anim < animaciones.Length; anim++) {
            bool apto = true;
            for (int i = 0; i < animaciones[anim].condiciones.Length; i++) {
                if (variables.ContainsKey(animaciones[anim].condiciones[i].nombreCondicion)) {
                    apto = Condicional(variables[animaciones[anim].condiciones[i].nombreCondicion], animaciones[anim].condiciones[i].valor, animaciones[anim].condiciones[i].condicional);
                    if(!apto)
                        break;
                }
            }
            if (apto) {
                SetAnimation(anim);
                break;
            }
        }
    }

    bool Condicional (float valorA, float valorB, CONDICIONAL condicion) {
        switch (condicion) {
            case CONDICIONAL.IgualQue:
                return valorA == valorB;
            case CONDICIONAL.DistintoQue:
                return valorA != valorB;
            case CONDICIONAL.MayorQue:
                return valorA > valorB;
            case CONDICIONAL.MayorIgualQue:
                return valorA >= valorB;
            case CONDICIONAL.MenorQue:
                return valorA < valorB;
            case CONDICIONAL.MenorIgualQue:
                return valorA <= valorB;
            default:
                return false;
        }
    }
}

[System.Serializable]
public class AnimacionClip {
    public AnimacionCondicion[] condiciones = new AnimacionCondicion[0];
    public TERMINAR terminar = TERMINAR.Repetir;
    public int otra = 0;

    public Sprite[] sprites = new Sprite[0];
    public int fps = 2;
}

[System.Serializable]
public class AnimacionCondicion {
    public string nombreCondicion = "";
    public float valor = 0;

    public CONDICIONAL condicional = CONDICIONAL.IgualQue;
}
