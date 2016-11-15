using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class EditorAnimacion : EditorWindow {
    
    Vector2 scrollPositionBarra;
    Vector2 scrollPositionContenido;

    GameObject objetoActual;
    Animacion animacion;

    int idAnimacion = 0;

    string nuevaVariable = "";
    float nuevoValor = 0;

    [MenuItem("DeadPool/Editor de animación", false, 140)]
    static void Init() {
        EditorAnimacion window = (EditorAnimacion)EditorWindow.GetWindow(typeof(EditorAnimacion));
        
        window.Show();
    }

    /*[MenuItem("GameEditor/Crear nuevo archivo", false, 150)]
    public static void CreateAsset() {
        BaseDatos asset = ScriptableObject.CreateInstance<BaseDatos>();
        AssetDatabase.CreateAsset(asset, "Assets/NuevoDocumentoDeCartas.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }*/

    void OnInspectorUpdate() {
        Repaint();
    }
    
    void OnGUI() {
        if (Selection.activeGameObject!=objetoActual) {
            GetObjeto(Selection.activeGameObject);
        }

        if (objetoActual==null) {
            EditorGUILayout.HelpBox("No has seleccionado ningún objeto", MessageType.Info);
            return;
        }
        if (animacion== null) {
            EditorGUILayout.HelpBox("No contiene ningun script de animación", MessageType.Warning);
            return;
        }

        MostrarGUI();
    }

    void GetObjeto (GameObject obj) {
        objetoActual = obj;
        if (objetoActual == null) {
            animacion = null;
            return;
        }

        animacion = objetoActual.GetComponent<Animacion>();
    }
    
    void MostrarGUI() {
        if(idAnimacion >= animacion.animaciones.Length)
            idAnimacion = animacion.animaciones.Length;

        GUILayout.BeginHorizontal();

        SelectorUnidad();

        GUILayout.BeginVertical();
        
        EdicionUnidad();
        
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    void SelectorUnidad() {
        GUILayout.BeginVertical();
        GUILayout.BeginVertical("box", GUILayout.Width(230));
        GUILayout.Label("Animaciones", EditorStyles.centeredGreyMiniLabel);
        scrollPositionBarra = GUILayout.BeginScrollView(scrollPositionBarra, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        for(int i = 0; i < animacion.animaciones.Length; i++) {
            string _nombre = "Animacion #" + (i + 1);

            if(idAnimacion == i) {
                GUILayout.Label(_nombre, GUILayout.Width(200));
            } else {
                /*if(GUILayout.Button(_nombre, GUILayout.Width(200))) {
                    GUIUtility.keyboardControl = 0;
                    idUnidad = i;
                    minValue = 0;
                    maxValue = 0;
                    if(cambiarPosicion) {
                        posicionFinal = idUnidad;
                        MoverUnidad(posicionInicial, posicionFinal);
                        cambiarPosicion = false;
                        posicionInicial = 0;
                        posicionFinal = 0;
                    }
                }*/
            }
        }
        if(GUILayout.Button("+", GUILayout.Width(200))) {
            GUIUtility.keyboardControl = 0;
            //CrearNuevaCarta();
            idAnimacion = animacion.animaciones.Length - 1;
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndVertical();
    }

    void EdicionUnidad() {
        /*if(!cambiarPosicion) {
            if(GUILayout.Button("Cambiar posición carta.", GUILayout.MinWidth(60), GUILayout.MaxWidth(300))) {
                posicionInicial = idUnidad;
                cambiarPosicion = true;
            }
        } else {
            if(GUILayout.Button("Cancelar cambio de posición.", GUILayout.MinWidth(60), GUILayout.MaxWidth(300))) {
                posicionInicial = 0;
                cambiarPosicion = false;
            }
            EditorGUILayout.HelpBox("Selecciona donde quieres cambiar de posición esta unidad usando el menú de la izquierda", MessageType.Info);
        }*/
        
        
        GUILayout.BeginVertical("box");
        GUILayout.Label("Variables. (De todas las animaciones)", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        foreach (KeyValuePair<string, float> valor in animacion.variables) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Variable: ", valor.Key);
            animacion.variables[valor.Key] = EditorGUILayout.FloatField("Valor: ", valor.Value);
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.Space(3);
        EditorGUILayout.BeginHorizontal();
        nuevaVariable = EditorGUILayout.TextField("Variable: ", nuevaVariable);
        nuevoValor = EditorGUILayout.FloatField("Valor: ", nuevoValor);
        if (GUILayout.Button ("AñadirVariable")) {
            animacion.variables.Add(nuevaVariable, nuevoValor);
            nuevaVariable = "";
            nuevoValor = 0;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        GUILayout.Label("Condiciones. (Tienen que darse todo en true para cambiarse)", EditorStyles.boldLabel);
        GUILayout.Space(5);

        for (int i = 0; i < animacion.animaciones[idAnimacion].condiciones.Length; i++) {
            EditorGUILayout.BeginHorizontal();
            animacion.animaciones[idAnimacion].condiciones[i].nombreCondicion = EditorGUILayout.TextField("Variable: ", animacion.animaciones[idAnimacion].condiciones[i].nombreCondicion);
            animacion.animaciones[idAnimacion].condiciones[i].condicional = (CONDICIONAL) EditorGUILayout.EnumPopup(animacion.animaciones[idAnimacion].condiciones[i].condicional);
            animacion.animaciones[idAnimacion].condiciones[i].valor = EditorGUILayout.FloatField(animacion.animaciones[idAnimacion].condiciones[i].valor);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button ("Nueva condición")) {
            Debug.LogWarning("Función no creada aún");
        }
        GUILayout.EndVertical();

        //VALORES DE LA ANIMACION
        GUILayout.BeginVertical("box");
        GUILayout.Label("Valores de la animación.", EditorStyles.boldLabel);
        GUILayout.Space(5);
        animacion.animaciones[idAnimacion].fps = EditorGUILayout.IntField("FPS: ", animacion.animaciones[idAnimacion].fps);

        EditorGUILayout.BeginHorizontal();
        animacion.animaciones[idAnimacion].terminar = (TERMINAR) EditorGUILayout.EnumPopup("Al terminar: ", animacion.animaciones[idAnimacion].terminar);
        if(animacion.animaciones[idAnimacion].terminar == TERMINAR.EmpezarOtra)
            animacion.animaciones[idAnimacion].otra = EditorGUILayout.IntField("ID animacion: ", animacion.animaciones[idAnimacion].otra);
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        

        GUILayout.BeginVertical("box");
        GUILayout.Label("Sprites.", EditorStyles.boldLabel);
        scrollPositionContenido = GUILayout.BeginScrollView(scrollPositionContenido, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        EditorGUILayout.BeginHorizontal("box");
        for (int i = 0; i< animacion.animaciones[idAnimacion].sprites.Length; i++) {
            animacion.animaciones[idAnimacion].sprites[i] = (Sprite) EditorGUILayout.ObjectField(animacion.animaciones[idAnimacion].sprites[i], typeof(Sprite), false, GUILayout.Width (50), GUILayout.Height (50));
        }
        if (GUILayout.Button ("+", GUILayout.Width(50), GUILayout.Height(50))) {

        }
        EditorGUILayout.EndHorizontal();

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    /*
    void Convertir (int index, int decenas) {
        float diferencia = maxValue - minValue;
        float unidad = diferencia / (baseDatos.campeones[idUnidad].habilidades[index].valores.Length-1);
        for (int i = 0; i < baseDatos.campeones[idUnidad].habilidades[index].valores.Length; i++) {
            baseDatos.campeones[idUnidad].habilidades[index].valores[i] = Mathf.Round((minValue + (unidad * i))*Mathf.Pow (10, decenas))/ Mathf.Pow(10, decenas);
        }
        minValue = 0;
        maxValue = 0;
        GUIUtility.keyboardControl = 0;
    }

    void MoverUnidad(int posInicial, int posFinal) {
        Campeon _antiguaCarta = baseDatos.campeones[posInicial];
        baseDatos.campeones[posInicial] = baseDatos.campeones[posFinal];
        baseDatos.campeones[posFinal] = _antiguaCarta;
    }
    
    void CrearNuevaCarta() {
        Campeon[] _copiaCartas = baseDatos.campeones;
        baseDatos.campeones = new Campeon[baseDatos.campeones.Length + 1];

        for(int i = 0; i < _copiaCartas.Length; i++) {
            baseDatos.campeones[i] = _copiaCartas[i];
        }
        baseDatos.campeones[baseDatos.campeones.Length - 1] = new Campeon();
        baseDatos.campeones[baseDatos.campeones.Length - 1].nombreCampeon = "";
    }

    void BorrarUltimaCarta() {
        Campeon[] _copiaCartas = baseDatos.campeones;
        baseDatos.campeones = new Campeon[baseDatos.campeones.Length - 1];

        for(int i = 0; i < _copiaCartas.Length; i++) {
            baseDatos.campeones[i] = _copiaCartas[i];
        }
    }

    void CrearHabilidad () {
        if (baseDatos.campeones[idUnidad].habilidades.Length >= 5) {
            Debug.LogError("Número máximo de habilidades permitidas, aumenta manualmente este límite");
            return;
        }
        Habilidad[] _copiaHabilidad = baseDatos.campeones[idUnidad].habilidades;
        baseDatos.campeones[idUnidad].habilidades = new Habilidad[baseDatos.campeones[idUnidad].habilidades.Length + 1];

        for(int i = 0; i < _copiaHabilidad.Length; i++) {
            baseDatos.campeones[idUnidad].habilidades[i] = _copiaHabilidad[i];
        }
        baseDatos.campeones[idUnidad].habilidades[baseDatos.campeones[idUnidad].habilidades.Length - 1] = new Habilidad();
        baseDatos.campeones[idUnidad].habilidades[baseDatos.campeones[idUnidad].habilidades.Length - 1].nombreHabilidad = "";
    }*/
}