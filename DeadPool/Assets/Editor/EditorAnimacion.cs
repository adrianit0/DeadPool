using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class EditorAnimacion : EditorWindow {
    
    Vector2 scrollPositionBarra;
    Vector2 scrollPositionVariables;
    Vector2 scrollPositionContenido;

    GameObject objetoActual;

    AnimacionController controller;
    Animacion animacion;

    Sprite lastSprite;
    SpriteRenderer render;
    bool playing = false;
    int tile = 0;
    float tiempoActual = 0;

    int idAnimacion = 0;

    //VARIABLES DE CONTROL
    string nuevoNombreArchivo = "";
    int ajustes;
    Animacion animacionACopiar = null;
    List<Animacion> animaciones = new List<Animacion>();

    //CREAR UNA NUEVA VARIABLE
    string nuevaVariable = "";
    float nuevoValor = 0;

    float heightButton = 15;
    float buttonWidth = 20;

    [MenuItem("DeadPool/Editor de animación.", false, 140)]
    static void Init() {
        EditorAnimacion window = (EditorAnimacion)EditorWindow.GetWindow(typeof(EditorAnimacion));
        
        window.Show();
    }

    [MenuItem("DeadPool/Crear nueva animación.", false, 150)]
    public static void CreateAsset() {
        Animacion asset = ScriptableObject.CreateInstance<Animacion>();
        AssetDatabase.CreateAsset(asset, "Assets/Animaciones/NuevaAnimacion.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    Animacion CreateAsset (string nombre) {
        if(string.IsNullOrEmpty(nombre))
            return null;

        Animacion asset = ScriptableObject.CreateInstance<Animacion>();
        AssetDatabase.CreateAsset(asset, "Assets/Animaciones/"+ nombre + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        return asset;
    }

    void OnInspectorUpdate() {
        Repaint();
    }

    void Update () {
        if (playing) {
            if(render == null)
                return;

            tiempoActual += 0.01f;

            if(tiempoActual > 1/(float) animacion.animaciones[idAnimacion].fps) {
                tiempoActual = 0;

                render.sprite = animacion.animaciones[idAnimacion].sprites[tile];
                tile++;

                if(tile == animacion.animaciones[idAnimacion].sprites.Length) {
                    tile = 0;
                }
            }
        }
    }
    
    void OnGUI() {
        if (Selection.activeGameObject!=objetoActual) {
            GetObjeto(Selection.activeGameObject);
        }

        if (objetoActual==null) {
            EditorGUILayout.HelpBox("No has seleccionado ningún objeto", MessageType.Info);
            return;
        }
        if (controller==null) {
            controller = objetoActual.GetComponent<AnimacionController>();
            if (controller== null) {
                EditorGUILayout.HelpBox("No contiene ningun script de animación", MessageType.Warning);
                return;
            }
        }

        if (animacion == null) {
            animacion = controller.animacion;
            if (animacion==null) {
                GetAnimacion();
                return;
            }
        }

        if (animacion != controller.animacion) {
            animacion = controller.animacion;
        }

        MostrarGUI();

        //SI HAY CAMBIOS -> GUARDAR EN 30s
    }

    void GetAnimacion () {
        EditorGUILayout.HelpBox("No contiene ningún archivo de animación.", MessageType.Warning);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Puedes añadir tu una animación:");
        controller.animacion = (Animacion) EditorGUILayout.ObjectField(controller.animacion, typeof(Animacion), false);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Crear un nuevo archivo de animación:");
        EditorGUILayout.BeginHorizontal();
        nuevoNombreArchivo = EditorGUILayout.TextField("Nombre archivo:", nuevoNombreArchivo);
        if(GUILayout.Button ("Crear")) {
            if (string.IsNullOrEmpty(nuevoNombreArchivo)) {
                Debug.LogWarning("No se ha creado porque el archivo debe tener un nombre");
            } else {
                controller.animacion = CreateAsset(nuevoNombreArchivo);
                nuevoNombreArchivo = "";
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("O seleccionar una de esta lista de animaciones ya creadas:");

        if (GUILayout.Button ("Buscar")) {
            BuscarAnimaciones();
        }

        EditorGUILayout.Space();

        for (int i = 0; i < animaciones.Count; i++) {
            if (GUILayout.Button (animaciones[i].ToString())) {
                if (animaciones[i]!=null) {
                    controller.animacion = animaciones[i];
                } else {
                    Debug.Log("Animacion no encontrada");
                }
            }
        }
    }

    void BuscarAnimaciones () {
        animaciones = new List<Animacion>();
        string[] guids = AssetDatabase.FindAssets("*", new string[] { "Assets/Animaciones" });

        //Debug.Log("Encontrado: " + guids.Length);
        for (int i = 0; i < guids.Length; i++) {
            guids[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            Animacion anim = (Animacion) AssetDatabase.LoadAssetAtPath(guids[i], typeof (Animacion));
            if (anim!=null) {
                animaciones.Add(anim);
            } else {
                Debug.LogWarning(guids[i] + " no compatible con Animacion");
            }
        }
    }
    

    void GetObjeto (GameObject obj) {
        objetoActual = obj;
        if (objetoActual == null) {
            controller = null;
            return;
        }

        if (controller!=null&&playing) {
            Stop();
        }

        controller = objetoActual.GetComponent<AnimacionController>();
        animacion = (controller!=null) ? controller.animacion : null;
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
            string _nombre = animacion.animaciones[i].nombre == "" ? "Animacion #" + (i) : animacion.animaciones[i].nombre + " (#"+i+")";

            EditorGUILayout.BeginHorizontal();
            if(idAnimacion == i) {
                GUILayout.Label(_nombre, GUILayout.Width(200-(buttonWidth*3)));
            } else {
                if(GUILayout.Button(_nombre, GUILayout.Width(200 - (buttonWidth * 3)))) {
                    GUIUtility.keyboardControl = 0;
                    idAnimacion = i;
                    if(playing)
                        Stop();
                }
            }
            if (animacion.animaciones.Length>1) {
                if(GUILayout.Button("-", GUILayout.Width(buttonWidth))) {
                    if(EditorUtility.DisplayDialog("Confirmar", "¿Deseas eliminar la animación " + animacion.animaciones[i].nombre + "?", "Sí", "No")) {
                        GUIUtility.keyboardControl = 0;
                        animacion.animaciones = BorrarValor<AnimacionClip>(animacion.animaciones, i);
                        idAnimacion = Mathf.Clamp(idAnimacion - 1, 0, animacion.animaciones.Length - 1);
                    }
                }
            }
            
            if(i == animacion.animaciones.Length - 1) {
                GUILayout.Space(buttonWidth + 4);
            } else {
                if(GUILayout.Button("↓", GUILayout.Width(buttonWidth))) {
                    animacion.animaciones = CambiarValor<AnimacionClip>(animacion.animaciones, i, i + 1);
                }
            }
            if(i == 0) {
                GUILayout.Space(buttonWidth);
            } else {
                if(GUILayout.Button("↑", GUILayout.Width(buttonWidth))) {
                    animacion.animaciones = CambiarValor<AnimacionClip>(animacion.animaciones, i, i - 1);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.Space(5);
        if(GUILayout.Button("Crear nueva animación", GUILayout.Width(205))||animacion.animaciones.Length==0) {
            GUIUtility.keyboardControl = 0;
            animacion.animaciones = NuevoValor<AnimacionClip>(animacion.animaciones);
            idAnimacion = animacion.animaciones.Length - 1;
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box", GUILayout.Width(230));
        GUILayout.Label("Variables"+((EditorApplication.isPlaying) ? " (Modo juego)" : ""), EditorStyles.centeredGreyMiniLabel);
        scrollPositionVariables = GUILayout.BeginScrollView(scrollPositionVariables, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        
        if (animacion.variables != null && animacion.variables.Count>0) {
            for (int i = 0; i < animacion.variables.Count; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("[" + i + "]", GUILayout.Width (20));
                string oldText = animacion.variables[i];
                animacion.variables[i] = EditorGUILayout.TextField(animacion.variables[i]);
                if (oldText != animacion.variables[i]) {
                    CambiarTextos(oldText, animacion.variables[i]);
                }
                animacion.variables[animacion.variables[i]] =  EditorGUILayout.FloatField((!EditorApplication.isPlaying) ? animacion.variables[animacion.variables[i]] : controller.variables[controller.variables[i]]);
                EditorGUILayout.EndHorizontal();
            }
        }
        GUILayout.Space(3);

        EditorGUILayout.BeginHorizontal();
        nuevaVariable = EditorGUILayout.TextField(nuevaVariable, GUILayout.Width(70));
        nuevoValor = EditorGUILayout.FloatField(nuevoValor, GUILayout.Width(70));
        if(GUILayout.Button("Añadir", GUILayout.Width(70))) {
            if (string.IsNullOrEmpty (nuevaVariable)) {
                EditorUtility.DisplayDialog("Error", "No se puede crear una variable sin nombre", "Ok...");
            } else {
                animacion.variables.Add(nuevaVariable, nuevoValor);
                nuevaVariable = "";
                nuevoValor = 0;
            }
        }
        EditorGUILayout.EndHorizontal();

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
        
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical("box");
        GUILayout.Label("Condiciones", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(5);

        if (animacion.animaciones[idAnimacion].condiciones.Length==0) {
            EditorGUILayout.HelpBox("No se ha creado ninguna condición, por lo que siempre será TRUE", MessageType.Warning);
        }

        string[] list = animacion.variables.GetListWithEmptyCase();
        for (int i = 0; i < animacion.animaciones[idAnimacion].condiciones.Length; i++) {
            EditorGUILayout.BeginHorizontal();
            animacion.animaciones[idAnimacion].condiciones[i].nombreCondicion = Popup(animacion.animaciones[idAnimacion].condiciones[i].nombreCondicion, list);
                //EditorGUILayout.TextField("Variable: ", animacion.animaciones[idAnimacion].condiciones[i].nombreCondicion);
            animacion.animaciones[idAnimacion].condiciones[i].condicional = (CONDICIONAL) EditorGUILayout.EnumPopup(animacion.animaciones[idAnimacion].condiciones[i].condicional);
            animacion.animaciones[idAnimacion].condiciones[i].valor = EditorGUILayout.FloatField(animacion.animaciones[idAnimacion].condiciones[i].valor);
            if(GUILayout.Button("-", GUILayout.Width(20))) {
                if (EditorUtility.DisplayDialog("Confirmar", "¿Deseas eliminar la condición "+ animacion.animaciones[idAnimacion].condiciones[i].nombreCondicion+"?", "Sí", "No")) {
                    GUIUtility.keyboardControl = 0;
                    animacion.animaciones[idAnimacion].condiciones = BorrarValor<AnimacionCondicion>(animacion.animaciones[idAnimacion].condiciones, i);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button ("Nueva condición")) {
            animacion.animaciones[idAnimacion].condiciones = NuevoValor<AnimacionCondicion>(animacion.animaciones[idAnimacion].condiciones);
        }
        GUILayout.EndVertical();

        //VALORES DE LA ANIMACION
        GUILayout.BeginVertical("box");
        GUILayout.Label("Ajustes de la animación", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button ("Básicas")) {
            ajustes = 0;
        }
        if(GUILayout.Button("Avanzadas")) {
            ajustes = 1;
        }
        EditorGUILayout.EndHorizontal();

        if (ajustes==0) {
            bool predeterminada = EditorGUILayout.Toggle("Predeterminado: ", animacion.animaciones[idAnimacion].predeterminada);
            if(predeterminada && predeterminada != animacion.animaciones[idAnimacion].predeterminada) {
                for(int i = 0; i < animacion.animaciones.Length; i++) {
                    animacion.animaciones[i].predeterminada = i == idAnimacion;
                }
            }
            animacion.animaciones[idAnimacion].nombre = EditorGUILayout.TextField("Nombre: ", animacion.animaciones[idAnimacion].nombre);
            animacion.animaciones[idAnimacion].fps = Mathf.Clamp(EditorGUILayout.IntField("FPS: ", animacion.animaciones[idAnimacion].fps), 1, 30);
            animacion.animaciones[idAnimacion].repetir = EditorGUILayout.Toggle("Reiniciable: ", animacion.animaciones[idAnimacion].repetir);

            EditorGUILayout.BeginHorizontal();
            animacion.animaciones[idAnimacion].terminar = (TERMINAR)EditorGUILayout.EnumPopup("Al terminar: ", animacion.animaciones[idAnimacion].terminar);
            if(animacion.animaciones[idAnimacion].terminar == TERMINAR.EmpezarOtra)
                animacion.animaciones[idAnimacion].otra = EditorGUILayout.IntField("ID animacion: ", animacion.animaciones[idAnimacion].otra);
            EditorGUILayout.EndHorizontal();
        } else if (ajustes==1) {
            EditorGUILayout.BeginHorizontal();
            animacionACopiar = (Animacion)EditorGUILayout.ObjectField(animacionACopiar, typeof(Animacion), false);
            if (GUILayout.Button ("Copiar")) {
                if (animacionACopiar!=null) {
                    animacion.animaciones = animacionACopiar.animaciones;
                    animacion.variables = animacionACopiar.variables;
                    animacionACopiar = null;
                }else {
                    Debug.LogWarning("No hay animación que copiar");
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button ("Coger lista")) {
                BuscarAnimaciones();
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();


        GUILayout.BeginVertical("box");
        GUILayout.Label("Animación", EditorStyles.centeredGreyMiniLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (!playing) {
            if (GUILayout.Button ("Play", GUILayout.Width (100))) {
                Play();
            }
        } else {
            if (GUILayout.Button ("Stop", GUILayout.Width(100))) {
                Stop();
            }
        }

        if (GUILayout.Button ("Guardar", GUILayout.Width(100))) {
            Guardar();
        }
        EditorGUILayout.LabelField("El autoguardado está desactivado en esta versión.");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);

        scrollPositionContenido = GUILayout.BeginScrollView(scrollPositionContenido, GUILayout.ExpandWidth(true)/*, GUILayout.ExpandHeight(true)*/);
        EditorGUILayout.BeginHorizontal("box");
        
        for (int i = 0; i< animacion.animaciones[idAnimacion].sprites.Length; i++) {
            EditorGUILayout.BeginVertical(GUILayout.Width(50));

            animacion.animaciones[idAnimacion].sprites[i] = (Sprite) EditorGUILayout.ObjectField(animacion.animaciones[idAnimacion].sprites[i], typeof(Sprite), false, GUILayout.Width (50), GUILayout.Height (50));
            if (GUILayout.Button("-",GUILayout.Width(50), GUILayout.Height(heightButton))) {
                animacion.animaciones[idAnimacion].sprites = BorrarValor<Sprite>(animacion.animaciones[idAnimacion].sprites, i);
            }
            if (i== animacion.animaciones[idAnimacion].sprites.Length-1) {
                GUILayout.Space(heightButton);
            } else {
                if(GUILayout.Button("→", GUILayout.Width(50), GUILayout.Height(heightButton))) {
                    animacion.animaciones[idAnimacion].sprites = CambiarValor<Sprite>(animacion.animaciones[idAnimacion].sprites, i, i+1);
                }
            }
            if(i == 0) {
                GUILayout.Space(heightButton);
            } else {
                if(GUILayout.Button("←", GUILayout.Width(50), GUILayout.Height(heightButton))) {
                    animacion.animaciones[idAnimacion].sprites = CambiarValor<Sprite>(animacion.animaciones[idAnimacion].sprites, i, i - 1);
                }
            }
            EditorGUILayout.EndVertical();
        }
        if (GUILayout.Button ("+", GUILayout.Width(50), GUILayout.Height(50))) {
            animacion.animaciones[idAnimacion].sprites = NuevoValor<Sprite> (animacion.animaciones[idAnimacion].sprites);
        }
        
        EditorGUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        GUILayout.EndVertical();
    }

    string Popup (string texto, string[] lista, params GUILayoutOption[] options) {
        for (int i = 0; i < lista.Length; i++) {
            if (lista[i]==texto) {
                int dev = EditorGUILayout.Popup(i, lista);
                return lista[dev];
            }
        }
        EditorGUILayout.Popup(0, lista);
        return lista[0];
    }

    void CambiarTextos (string oldText, string newText) {
        if(oldText == newText)
            return;

        for (int x = 0; x < animacion.animaciones.Length; x++) {
            for (int y = 0; y < animacion.animaciones[x].condiciones.Length; y++) {
                if (animacion.animaciones[x].condiciones[y].nombreCondicion== oldText) {
                    animacion.animaciones[x].condiciones[y].nombreCondicion = newText;
                }
            }
        }
    }

    void Play () {
        playing = true;
        render = controller.GetComponent<SpriteRenderer>();
        tile = 0;
        lastSprite = render.sprite;
    }

    void Stop () {
        playing = false;
        render.sprite = lastSprite;
        render = null;
        lastSprite = null;
    }
    
    /// <summary>
    /// Guarda todas las animaciones que hayan dentro de la carpeta "Animaciones".
    /// </summary>
    void Guardar () {
        if(animacion == null)
            return;

        BuscarAnimaciones();

        for (int i = 0; i< animaciones.Count; i++) {
            EditorUtility.SetDirty(animaciones[i]);
        }
    }

    T[] NuevoValor <T>(T[] array) where T : new() {
        T[] nuevoArray = new T[array.Length+1];
        for (int i = 0; i < array.Length; i++) {
            nuevoArray[i] = array[i];
        }
        nuevoArray[array.Length] = new T();
        return nuevoArray;
    }

    T[] BorrarValor <T> (T[] array, int value) {
        if(value < 0 || value >= array.Length)
            return array;
        T[] nuevoArray = new T[array.Length - 1];

        int x = 0;
        for (int i = 0; i < nuevoArray.Length; i++) {
            if(x == value)
                x++;
            nuevoArray[i] = array[x];
            x++;
        }

        return nuevoArray;
    }

    T[] CambiarValor <T> (T[] array, int value1, int value2) {
        if(value1 < 0 || value1 >= array.Length || value2 < 0 || value2 >= array.Length)
            return array;

        T value = array[value1];
        array[value1] = array[value2];
        array[value2] = value;
        return array;
    }

    /*
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