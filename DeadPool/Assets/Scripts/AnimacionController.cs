using UnityEngine;
using System.Collections;

public class AnimacionController : MonoBehaviour {
    public Animacion animacion;
    public AnimacionVariable variables = new AnimacionVariable();

    public bool isPlaying = true;
    SpriteRenderer render;

    int actual = 0;
    int step = 0, stepMaximo = 2;
    float tiempoActual = 0, tiempoTotal = 0, velocidad = 1f;
    
    void Awake () {
        render = GetComponent<SpriteRenderer>();
        if(render == null) {
            Debug.LogError("Renderizador en el objeto " + name + " no encontrado.");
        }
    }

    void Start() {
        if(animacion == null || animacion.animaciones == null || animacion.animaciones.Length == 0) {
            isPlaying = false;
            return;
        }

        variables.CopyOf(animacion.variables);
        for(int i = 0; i < animacion.animaciones.Length; i++) {
            if(animacion.animaciones[i].predeterminada) {
                SetAnimation(i);
                return;
            }
        }

        SetAnimation(0);
    }

    void Update() {
        if(render == null || !isPlaying)
            return;

        tiempoActual += Time.deltaTime * velocidad;

        if(tiempoActual > tiempoTotal) {
            tiempoActual = 0;

            render.sprite = animacion.animaciones[actual].sprites[step];
            step++;

            if(step == stepMaximo) {
                switch(animacion.animaciones[actual].terminar) {
                    case TERMINAR.Repetir:
                        step = 0;
                        break;
                    case TERMINAR.EmpezarOtra:
                        SetAnimation(animacion.animaciones[actual].otra);
                        break;
                    case TERMINAR.NoSeguir:
                        isPlaying = false;
                        break;
                    case TERMINAR.ActualizarSolo:
                        ActualizarCondiciones();
                        break;
                    default:
                        Debug.LogWarning("Opción no configurada");
                        break;
                }
            }
        }
    }

    public void SetFloat(string texto, float cantidad) {
        if(variables.ContainsKey(texto)) {
            cantidad = Mathf.Round(cantidad * 1000) / 1000;
            if(cantidad != variables[texto]) {
                variables[texto] = cantidad;
                ActualizarCondiciones();
            }
        } else {
            Debug.LogWarning("Variable " + texto + " no existe.");
        }
    }

    //Cambia de una animación a otra
    public void SetAnimation(int id) {
        actual = id;
        step = 0;
        stepMaximo = animacion.animaciones[id].sprites.Length;

        if(animacion.animaciones[id].fps > 0) {
            tiempoTotal = 1 / (float)animacion.animaciones[id].fps;
        } else {
            tiempoTotal = 0;
            isPlaying = false;
        }

        if(animacion.animaciones[id].sprites.Length > 0)
            render.sprite = animacion.animaciones[id].sprites[0];
        isPlaying = true;
    }

    public void NuevaAnimacion (Animacion _animacion) {
        if(_animacion == null || _animacion.animaciones == null || _animacion.animaciones.Length == 0) {
            isPlaying = false;
            return;
        }

        animacion = _animacion;
        variables.CopyOf(animacion.variables);
        for(int i = 0; i < animacion.animaciones.Length; i++) {
            if(animacion.animaciones[i].predeterminada) {
                SetAnimation(i);
                return;
            }
        }

        SetAnimation(0);
    }

    public void CambiarVelocidad(float nuevaVelocidad) {
        velocidad = nuevaVelocidad;
    }

    public void ActualizarCondiciones() {
        for(int anim = 0; anim < animacion.animaciones.Length; anim++) {
            bool apto = true;
            for(int i = 0; i < animacion.animaciones[anim].condiciones.Length; i++) {
                if(variables.ContainsKey(animacion.animaciones[anim].condiciones[i].nombreCondicion)) {
                    apto = Condicional(variables[animacion.animaciones[anim].condiciones[i].nombreCondicion], animacion.animaciones[anim].condiciones[i].valor, animacion.animaciones[anim].condiciones[i].condicional);
                    if(!apto)
                        break;
                }
            }
            if(apto) {
                if(anim == actual && !animacion.animaciones[actual].repetir)
                    return;

                SetAnimation(anim);
                break;
            }
        }
    }

    bool Condicional(float valorA, float valorB, CONDICIONAL condicion) {
        switch(condicion) {
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
