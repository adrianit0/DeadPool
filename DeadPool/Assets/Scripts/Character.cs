using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum DIRECCION { Izquierda, Derecha};

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Character : MonoBehaviour {

    public int ID;

    public int bando = 0;
    
    float vidaMaxima = 100, vidaActual = 100;
    float manaMaximo = 100, manaActual = 100, regMana = 20;
    public float velocidad = 3.6f, salto = 12/*, velocidadAtaque = 1f*/;
    public bool vivo = true;

    //CONFIGURACION
    public LayerMask layer;
    public bool interactable = true;

    public Transform posArrI;
    public Transform posAbaD;
    public Transform posicionArma;

    [HideInInspector]
    public int habilidad = 0;

    GameObject balaPrefab;

    //VARIABLES JUEGO
    float mov = 0;
    DIRECCION dir;

    public bool enSuelo = true;
    bool enSueloInternal = false;
    
    float lastTimeShot = 0, timeNeededShot = 0;
    float lastJump = 0, timeJump = 0.25f;
    
    float fixedDelta;

    Rigidbody2D rigid;
    BoxCollider2D coll;
    SpriteRenderer render;
    Animator anim;

    void Awake () {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();

        fixedDelta = Time.fixedDeltaTime;
    }

    void Start () {
        MirarHacia(-transform.localScale.x);
    }

	void FixedUpdate () {
        if (posArrI!=null&&posAbaD!=null) {
            enSuelo = Physics2D.OverlapArea(posArrI.position, posAbaD.position, layer);
            if (enSuelo!=enSueloInternal) {
                //anim.SetBool("EnSuelo", enSuelo);
                enSueloInternal = enSuelo;
            }
        }

        rigid.velocity = new Vector2 (velocidad * mov, rigid.velocity.y);

        anim.SetFloat("HorSpeed", Mathf.Abs(rigid.velocity.x));
        anim.SetFloat("VerSpeed", Mathf.Abs(rigid.velocity.y));

        lastTimeShot += fixedDelta;
        lastJump += fixedDelta;

        if(transform.position.y < -20 && vivo)
            Morir();
    }

    ///-1 izquierda
    /// 0 quieto
    /// 1 derecha
    public void Moverse (float direccion) {
        if(!interactable)
            return;

        mov = Mathf.Clamp(direccion, -1, 1);

        //if(defendiendo && mov != 0)
        //    Defender(false, true);

        MirarHacia(mov);
        
        //anim.SetBool("Movement", mov != 0);
    }

    public void MirarHacia (float direccion) {
        if(!interactable)
            return;

        if(direccion < 0)
            dir = DIRECCION.Izquierda;
        else if(direccion > 0)
            dir = DIRECCION.Derecha;

        transform.localScale = new Vector3((dir == DIRECCION.Izquierda) ? -1 : 1, transform.localScale.y, transform.localScale.z);
    }

    public void Saltar () {
        if(!interactable || lastJump<timeJump)
            return;

        lastJump = 0;

        if(enSuelo) {
            rigid.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
        }
            
    }

    
    public void Disparar() {
        if(!interactable)
            return;


        if (lastTimeShot>timeNeededShot) {
            //Weapon _balaScript = balaPrefab.GetComponent<Weapon>();

            /*if (_balaScript.costeMana>manaActual) {
                return;
            }*/

            //GameObject _balaObj = (GameObject)Instantiate(balaPrefab, posicionArma.transform.position, Quaternion.identity);
            // _balaScript = _balaObj.GetComponent<Weapon>();
            

            lastTimeShot = 0;

            /*
            _balaScript.bando = bando;
            _balaScript.SetVelocidad(_balaScript.velocidad * ((dir == DIRECCION.Derecha) ? 1 : -1));
            _balaScript.transform.localScale = new Vector2((dir == DIRECCION.Derecha) ? -1 : 1, 1);
            _balaScript.origenBala = this.gameObject;
            _balaScript.daño *= dañoBase;
            _balaScript.retroceso *= retrocesoBase;

            timeNeededShot = _balaScript.delay*delayHechizo;

            ConsumirMana(_balaScript.costeMana);
            */
        }
    }
    
    public void InfligirDaño (int cantidad) {
        StartCoroutine(CambiarColor());

        if(cantidad < 0)
            cantidad = 0;

        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0, vidaMaxima);
        
        if(vidaActual == 0) {
            Morir();
        }  
    }

    IEnumerator CambiarColor () {
        StopCoroutine("CambiarColor");
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        Color _color = new Color(0, 0, 0, 1);

        float lerp = 0;
        while (lerp < 1) {
            lerp += Time.deltaTime;

            render.color = Color.Lerp (render.color, _color, lerp);
            yield return wait;
        }
    }

    public void CurarVida (float cantidad) {
        vidaActual = Mathf.Clamp(vidaActual + cantidad, 0, vidaMaxima);
    }

    void Morir() {
        Moverse(0);

        vivo = false;
        interactable = false;
        //anim.SetTrigger("Morir");
        coll.enabled = true;
    }

    public void ConsumirMana (float cantidad) {
        manaActual = Mathf.Clamp (manaActual-cantidad, 0, manaMaximo);
    }

    void RegMana () {
        if(manaActual >= manaMaximo)
            return;
        
        manaActual = Mathf.Clamp(manaActual + regMana, 0, manaMaximo);
    }
}