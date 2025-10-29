# GUÍA DE CONFIGURACIÓN COMPLETA - CORRECCIONES Y MEJORAS

## ✅ PROBLEMAS SOLUCIONADOS

### 1. **Enemigos aparecen en dimensión incorrecta al reiniciar**
**ARREGLADO** en `GameManager.cs` línea 271
- Ahora llama correctamente a `dimensionSwitcher.Resetear()` que fuerza dimensión NORMAL
- Los enemigos siempre aparecen visibles al reiniciar

### 2. **Menú de pausa vacío al abrir**
**ARREGLADO** en `UIManager.cs` línea 104
- Por defecto abre en pestaña INVENTARIO
- Siempre muestra contenido al abrir

### 3. **Controles no caben en opciones**
**ARREGLADO** - Creado panel dedicado `PanelControles.cs`
- Ahora hay 4 pestañas: Inventario, Opciones, Controles, Estadísticas
- E/Q ciclan entre todas las pestañas

### 4. **Panel de estadísticas del jugador**
**NUEVO** - Creado `PanelEstadisticas.cs`
- Muestra barras de Salud, Sed, Hambre, Stamina
- Actualización en tiempo real con interpolación suave
- Colores que cambian según el valor

---

## 📦 INSTALACIÓN DE CINEMACHINE

### **PASO 1: Instalar el paquete**
1. En Unity → **Window → Package Manager**
2. En la esquina superior izquierda, cambiar a **"Unity Registry"**
3. Buscar **"Cinemachine"**
4. Click en **Install**
5. Esperar a que termine la instalación

### **PASO 2: Crear la Virtual Camera**
1. En Hierarchy → Click derecho → **Cinemachine → Virtual Camera**
2. Se crea automáticamente "CM vcam1"
3. Renombrar a **"PlayerCamera"**

### **PASO 3: Configurar la cámara**
Con **PlayerCamera** seleccionado en Inspector:

```
CinemachineVirtualCamera:
├─ Priority: 10 (mayor prioridad)
├─ Follow → Arrastra tu GameObject "Cientifico"
├─ Look At → Arrastra tu GameObject "Cientifico"

Body:
├─ Framing Transposer (recomendado para 2D)
│  ├─ Camera Distance: 10
│  ├─ Screen X: 0.5
│  ├─ Screen Y: 0.5
│  ├─ Dead Zone Width: 0.1
│  ├─ Dead Zone Height: 0.1
│  ├─ Soft Zone Width: 0.8
│  ├─ Soft Zone Height: 0.8
│  └─ Damping: X=1, Y=1, Z=1

Aim:
└─ Do Nothing (para 2D top-down)

Noise:
└─ None (o Basic Multi Channel si querés shake)
```

### **PASO 4: Ajustar la Main Camera**
Con **Main Camera** seleccionado:
- **NO toques el script CinemachineBrain** (se agregó automáticamente)
- Ajustar **Size** (Orthographic): 5-10 según tu preferencia
- Position Z: -10 (importante para 2D)

---

## 🎮 CONFIGURACIÓN EN INSPECTOR

### **A) UIManager (en GameObject MenuPausa):**
```
Menu de Pausa:
├─ Menu Pausa → MenuPausa (GameObject)
├─ Panel Inventario → PanelInventario
├─ Panel Opciones → PanelOpciones
├─ Panel Controles → PanelControles (NUEVO)
└─ Panel Estadisticas → PanelEstadisticas (NUEVO)
```

---

### **B) CREAR PanelControles:**

**Jerarquía:**
```
MenuPausa
└─ PanelControles (Panel)
   ├─ Titulo (TextMeshPro - "═══ CONTROLES ═══")
   └─ TextoControles (TextMeshPro con Scroll Rect)
```

**Configurar PanelControles:**
1. GameObject Panel
2. **Add Component** → Script **PanelControles**
3. **RectTransform**: Anchors Center, Width 700, Height 500
4. **Desactivar** inicialmente

**Configurar TextoControles:**
- **Width**: 650, **Height**: auto
- **Font Size**: 16
- **Alignment**: Left + Top
- **Overflow**: Overflow (para que no se corte)
- **Asignar** en el script PanelControles

**En Inspector de PanelControles:**
```
Texto Controles → TextoControles
```

---

### **C) CREAR PanelEstadisticas:**

**Jerarquía:**
```
MenuPausa
└─ PanelEstadisticas (Panel)
   ├─ Titulo (TextMeshPro - "═══ ESTADÍSTICAS ═══")
   ├─ SeccionSalud (Panel)
   │  ├─ LabelSalud (TextMeshPro - "SALUD")
   │  ├─ BarraSalud (UI Slider)
   │  └─ ValorSalud (TextMeshPro - "100/100")
   ├─ SeccionSed (igual estructura)
   ├─ SeccionHambre (igual estructura)
   └─ SeccionStamina (igual estructura)
```

**Configurar PanelEstadisticas:**
1. GameObject Panel
2. **Add Component** → Script **PanelEstadisticas**
3. **RectTransform**: Anchors Center, Width 700, Height 500
4. **Desactivar** inicialmente

**Configurar cada Slider:**
- **Min Value**: 0
- **Max Value**: 100
- **Whole Numbers**: Activar
- **Interactable**: Desactivar (solo visual)
- **Direction**: Left to Right
- **Fill**: Color según tipo (Verde/Cyan/etc)

**En Inspector de PanelEstadisticas:**
```
Referencias del Jugador:
└─ Cientifico → (Se busca automáticamente, pero puedes asignar)

Barras de Estadísticas:
├─ Barra Salud → BarraSalud
├─ Barra Sed → BarraSed
├─ Barra Hambre → BarraHambre
└─ Barra Stamina → BarraStamina

Textos de Valores:
├─ Texto Salud → ValorSalud
├─ Texto Sed → ValorSed
├─ Texto Hambre → ValorHambre
└─ Texto Stamina → ValorStamina

Actualización:
└─ Velocidad Actualizacion → 0.1
```

---

## 🎯 CONTROLES FINALES

### **Navegación de pestañas:**
```
ESC o P → Abrir/Cerrar menú
E → Siguiente pestaña (Inventario → Opciones → Controles → Stats → Inventario...)
Q → Pestaña anterior (...Stats → Controles → Opciones → Inventario)
C → Cerrar menú
```

### **Orden de pestañas:**
1. **Inventario** (por defecto al abrir)
2. **Opciones** (Resolución, Calidad, Brillo)
3. **Controles** (Lista completa de controles)
4. **Estadísticas** (Salud, Sed, Hambre, Stamina)

---

## 🔍 VERIFICACIÓN

### **Checklist después de configurar:**
- [ ] Cinemachine instalado desde Package Manager
- [ ] PlayerCamera creando y siguiendo al jugador
- [ ] MenuPausa tiene 4 paneles asignados en UIManager
- [ ] PanelControles muestra lista de controles
- [ ] PanelEstadisticas muestra 4 barras funcionando
- [ ] Al reiniciar, enemigos aparecen visibles
- [ ] Al abrir menú (ESC/P), muestra Inventario por defecto
- [ ] E/Q cambian entre las 4 pestañas

---

## ⚠️ NOTAS IMPORTANTES

1. **Cinemachine**: Si querés un seguimiento más suave, aumenta el Damping (2-3)
2. **Estadísticas**: Las barras se actualizan cada 0.1 segundos mientras el panel está abierto
3. **Performance**: PanelEstadisticas solo actualiza cuando está activo
4. **Pestañas**: Ahora ciclan en orden con E (siguiente) y Q (anterior)

---

**¿Todo claro? Prueba el juego y decime si algo no funciona como esperabas.**
