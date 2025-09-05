# UIManager - Sistema de UI Completo para DimensionSwitcher

## Resumen
He creado un sistema completo de UI que maneja todo el flujo del juego: menú principal, HUD en partida, pantallas de victoria y derrota. El sistema respeta todos los flujos existentes y permite un loop completo de juego.

## Archivos Modificados/Creados

### 1. `UIManager.cs` (Actualizado)
- **Ubicación**: `Assets/Scripts/UI/UIManager.cs`
- **Funciones Principales**:
  - Manejo de menú principal
  - Control del HUD durante el juego
  - Pantallas de victoria y derrota
  - Botones para navegación entre estados

### 2. `GameManager.cs` (Actualizado)
- **Ubicación**: `Assets/Scripts/Core/GameManager.cs`
- **Nuevos Métodos**:
  - `IniciarJuego()`: Inicia el juego desde el menú principal
  - `ResetearJuegoCompleto()`: Resetea todo para volver al menú
  - **Variables Nuevas**:
    - `juegoIniciado`: Controla si el juego está activo
    - `uiManager`: Referencia al UIManager (antes era `hud`)

### 3. `UISetupHelper.cs` (Nuevo)
- **Ubicación**: `Assets/Scripts/UI/UISetupHelper.cs`
- **Propósito**: Script auxiliar para crear automáticamente la UI
- **Características**:
  - Crea toda la estructura de UI automáticamente
  - Configura las referencias del UIManager
  - Incluye instrucciones detalladas

## Flujo del Juego

### 1. **Inicio del Juego**
- Comienza en el menú principal
- El tiempo está pausado (`Time.timeScale = 0`)
- El jugador no puede moverse
- Solo se muestra el menú principal

### 2. **Al Presionar "Iniciar Juego"**
- Se oculta el menú principal
- Se muestra el HUD
- Se resetea el estado del juego
- Se habilitan los controles del jugador
- Se reanuda el tiempo (`Time.timeScale = 1`)

### 3. **Durante el Juego**
- El HUD se actualiza constantemente
- Se comprueban las condiciones de victoria/derrota
- El jugador puede interactuar normalmente

### 4. **En Victoria**
- Se pausa el tiempo
- Se deshabilitan los controles
- Se muestra la pantalla de victoria
- **Botón "Volver al Menú"**: Resetea todo y vuelve al menú principal

### 5. **En Derrota**
- Se pausa el tiempo
- Se deshabilitan los controles
- Se muestra la pantalla de derrota
- **Botón "Reintentar"**: Reinicia el nivel manteniendo el HUD activo

## Configuración en Unity

### Opción 1: Configuración Automática (Recomendada)
1. Agrega el script `UISetupHelper` a un GameObject vacío en tu escena
2. En el Inspector, marca `crearUIAutomaticamente = true`
3. Arrastra tu `GameManager` al campo `gameManager`
4. Ejecuta la escena o haz clic derecho en el componente y selecciona "Crear UI Completa"
5. El script creará toda la estructura automáticamente
6. Elimina el GameObject con `UISetupHelper` cuando termines

### Opción 2: Configuración Manual
1. Crea un Canvas en tu escena
2. Dentro del Canvas, crea estos paneles:
   ```
   Canvas
   ├── MenuPrincipal
   │   ├── Titulo (TextMeshProUGUI)
   │   └── BotonIniciar (Button)
   ├── HUDContainer
   │   ├── BarraSalud (Slider)
   │   ├── BarraSed (Slider)
   │   ├── BarraHambre (Slider)
   │   ├── BarraStamina (Slider)
   │   ├── IconoComponente1 (Image)
   │   ├── IconoComponente2 (Image)
   │   └── IconoComponente3 (Image)
   ├── PantallaVictoria
   │   ├── TextoVictoria (TextMeshProUGUI)
   │   └── BotonMenu (Button)
   └── PantallaDerrota
       ├── TextoDerrota (TextMeshProUGUI)
       └── BotonReiniciar (Button)
   ```

3. Configura las referencias en el UIManager:
   - **Menu Principal**: Arrastra el panel MenuPrincipal
   - **HUD Container**: Arrastra el panel HUDContainer
   - **Pantalla Victoria**: Arrastra el panel PantallaVictoria
   - **Pantalla Derrota**: Arrastra el panel PantallaDerrota
   - **Botones**: Arrastra cada botón correspondiente
   - **Sliders**: Arrastra cada slider del HUD
   - **Iconos**: Arrastra cada imagen de componente
   - **Game Manager**: Arrastra tu GameManager

4. En el GameManager:
   - Cambia la referencia de `hud` a `uiManager`
   - Arrastra tu UIManager al campo `uiManager`

## Características Importantes

### 1. **Compatibilidad Hacia Atrás**
- Las referencias `pantallaVictoria` y `pantallaDerrota` en GameManager se mantienen
- Si no hay UIManager asignado, usa el sistema anterior
- El método `ReiniciarNivel()` sigue funcionando como antes

### 2. **Control de Estados**
- `juegoIniciado`: Indica si el juego está activo
- `juegoTerminado`: Indica si el juego terminó
- Solo actualiza UI cuando el juego está activo

### 3. **Manejo de Tiempo**
- Se pausa automáticamente en menús y pantallas de fin
- Se reanuda cuando se inicia o reinicia el juego

### 4. **Botones Inteligentes**
- **Iniciar**: Resetea todo y comienza el juego
- **Volver al Menú**: Resetea completamente y vuelve al menú
- **Reintentar**: Solo reinicia el nivel actual

## Métodos Públicos Disponibles

### UIManager
- `MostrarMenuPrincipal()`: Muestra el menú principal
- `IniciarJuego()`: Inicia el juego desde el menú
- `MostrarPantallaVictoria()`: Muestra pantalla de victoria
- `MostrarPantallaDerrota()`: Muestra pantalla de derrota
- `VolverAlMenu()`: Vuelve al menú principal
- `ReiniciarJuego()`: Reinicia el nivel actual
- `Actualizar(cientifico, estabilizador)`: Actualiza el HUD

### GameManager
- `IniciarJuego()`: Nuevo - Inicia el juego completamente
- `ResetearJuegoCompleto()`: Nuevo - Resetea todo al estado inicial
- `ReiniciarNivel()`: Reinicia solo el nivel actual (sin cambiar UI)

## Testing del Sistema

1. **Flujo Completo**:
   - Inicio → Menú Principal
   - Clic "Iniciar" → Juego activo
   - Morir → Pantalla derrota → Clic "Reintentar" → Juego activo
   - Ganar → Pantalla victoria → Clic "Volver al Menú" → Menú Principal

2. **Controles**:
   - Los controles solo funcionan cuando `juegoIniciado = true`
   - Se deshabilitan automáticamente en menús y pantallas de fin

3. **Actualización de UI**:
   - El HUD se actualiza solo durante el juego
   - Los iconos de componentes se muestran correctamente
   - Las barras reflejan el estado del jugador

## Troubleshooting

### Si los botones no funcionan:
- Verifica que las referencias estén asignadas en el Inspector
- Asegúrate de que `gameManager` esté asignado en UIManager

### Si el HUD no se actualiza:
- Verifica que `uiManager` esté asignado en GameManager
- Asegúrate de que los sliders e imágenes estén referenciados

### Si el juego no inicia:
- Verifica que `UIManager.gameManager` tenga la referencia correcta
- Asegúrate de que el método `IniciarJuego()` se esté llamando

Este sistema proporciona un flujo completo y profesional para tu juego, manteniendo la compatibilidad con el código existente y permitiendo una experiencia de usuario fluida.
