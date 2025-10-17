# 📋 **GUÍA DE CONFIGURACIÓN - DIMENSION SWITCHER**

## 🎯 **Resumen del Sistema**
**DimensionSwitcher** es un videojuego 2D completo donde el jugador debe encontrar 3 componentes para reparar un estabilizador cuántico mientras sobrevive a enemigos y mantiene sus necesidades básicas. Incluye un sistema de cambio de dimensiones, inventario funcional, y supervivencia básica.

---

## 🔧 **CONFIGURACIÓN EN INSPECTOR**

### **1. GameManager Setup**
**Ubicación**: Objeto vacío en la escena llamado "GameManager"  
**Script**: `GameManager.cs`

#### **Referencias Principales**
- **Cientifico**: Drag del prefab del jugador desde la escena
- **Estabilizador**: Drag del objeto EstabilizadorCuantico desde la escena  
- **UIManager**: Drag del objeto Canvas que contiene el UIManager
- **QuestManager**: Drag del objeto con QuestManager (puede ser el mismo GameManager)
- **DimensionSwitcher**: Drag del objeto con DimensionSwitcher (puede ser el mismo GameManager)

#### **Configuración de Enemigos**
- **Enemy Prefab**: Array con los prefabs de enemigos (desde carpeta Prefabs/)
- **Posiciones Enemigos Iniciales**: Se llena automáticamente al ejecutar
- **Componentes Drop Enemigos**: Array con los ScriptableObjects de componentes que dropean

---

### **2. Cientifico (Jugador) Setup**
**Ubicación**: Prefab del jugador en la escena  
**Script**: `Cientifico.cs`

#### **Estadísticas del Jugador**
- **Salud**: 100 (valor inicial)
- **Stamina**: 100 (valor inicial)
- **Hambre**: 100 (valor inicial)  
- **Sed**: 100 (valor inicial)

#### **Sistema de Inventario**
- **Inventory**: Debe tener el componente `Inventory` en el mismo GameObject
- **Arma**: ScriptableObject de tipo Weapon (opcional)

#### **Configuración de Movimiento**
- **Velocidad**: 5 (velocidad de movimiento)
- **Rango Ataque**: 2 (rango para atacar enemigos)

#### **Referencias Visuales**
- **Arma Renderer**: SpriteRenderer hijo que mostrará el arma

#### **Sistema de Necesidades**
- **Tiempo Reduccion Necesidades**: 30 (segundos entre reducciones)
- **Cantidad Reduccion Hambre**: 5
- **Cantidad Reduccion Sed**: 8
- **Cantidad Reduccion Stamina**: 3

---

### **3. Enemy Setup**
**Ubicación**: Prefabs de enemigos  
**Script**: `Enemy.cs`

#### **Estadísticas del Enemigo**
- **Salud Enemy**: 100
- **Daño**: 10
- **Rango Deteccion**: 10f
- **Velocidad**: 5f

#### **Sistema de Drops**
- **Drop Type**: Food, Water, Components, o Weapon
- **Drop Chance**: 0.0 a 1.0 (probabilidad de dropear)
- **Drop Min**: 1 (cantidad mínima)
- **Drop Max**: 1 (cantidad máxima)
- **Posibles Drops**: Array con ScriptableObjects que puede dropear

#### **Control de Ataques**
- **Tiempo Entre Ataques**: 1f (segundos)

---

### **4. UIManager Setup**
**Ubicación**: Canvas principal de la escena  
**Script**: `UIManager.cs`

#### **HUD Elements**
- **Salud**: Slider para mostrar vida (valor 0-100)
- **Sed**: Slider para mostrar sed (valor 0-100)  
- **Hambre**: Slider para mostrar hambre (valor 0-100)
- **Stamina**: Slider para mostrar stamina (valor 0-100)
- **Dimension Text**: TextMeshPro para mostrar dimensión actual
- **Icon1**: Image para mostrar componente 1 encontrado
- **Icon2**: Image para mostrar componente 2 encontrado  
- **Icon3**: Image para mostrar componente 3 encontrado
- **HUD Container**: Panel padre que contiene todos los elementos HUD

#### **Menu Principal**
- **Menu Principal**: Panel del menú inicial
- **Boton Iniciar**: Botón para comenzar el juego

#### **Pantallas de Final**
- **Pantalla Victoria**: Panel que se muestra al ganar
- **Boton Menu Victoria**: Botón para volver al menú desde victoria
- **Pantalla Derrota**: Panel que se muestra al perder
- **Boton Reiniciar Derrota**: Botón para reiniciar desde derrota

#### **Referencias**
- **Game Manager**: Drag del objeto GameManager

---

### **5. InventoryUI Setup**  
**Ubicación**: Canvas de UI, panel de inventario  
**Script**: `InventoryUI.cs`

#### **Referencias del UI**
- **Inventory Panel**: Panel principal del inventario
- **Slots Parent**: Transform donde se crearán los slots (ej: Grid Layout Group)
- **Slot Prefab**: Prefab de un slot individual

#### **Información del Item**
- **Item Name Text**: TextMeshPro para nombre del item
- **Item Description Text**: TextMeshPro para descripción
- **Item Quantity Text**: TextMeshPro para cantidad
- **Item Preview Image**: Image para ícono del item

#### **Navegación**
- **Slots Visibles**: 9 (número de slots en pantalla)
- **Color Slot Seleccionado**: Color.yellow
- **Color Slot Normal**: Color.white

#### **Referencias de Items**
- **Todos Los Items Del Juego**: Array con TODOS los ScriptableObjects de items

---

### **6. DimensionSwitcher Setup**
**Ubicación**: GameManager o objeto independiente  
**Script**: `DimensionSwitcher.cs`

#### **Configuración de Dimensiones**
- **Normal Mask**: "Dim_Normal" (nombre de capa)
- **Altered Mask**: "Dim_Altered" (nombre de capa)

#### **Estado del Sistema**  
- **Desbloqueado**: false (se activa al encontrar primer componente)
- **Dimension Actual**: false (false = Normal, true = Alterada)

#### **Configuración de Cooldown**
- **Cooldown**: 5f (segundos de espera entre cambios)
- **Toggle Key**: KeyCode.Tab (tecla para cambiar)

#### **Referencias**
- **Quest Manager**: Drag del objeto con QuestManager

---

### **7. QuestManager Setup**
**Ubicación**: GameManager o objeto independiente  
**Script**: `QuestManager.cs`

#### **Estado de la Quest**
- **Componentes Recolectados**: Lista que se llena automáticamente

#### **Referencias**  
- **Inventory**: Referencia al Inventory del jugador (se busca automáticamente)

---

### **8. EstabilizadorCuantico Setup**
**Script**: `EstabilizadorCuantico.cs` (sin cambios mayores)

#### **Configuración Existente**
- **Componentes Necesarios**: {1, 2, 3}
- **Componentes Insertados**: Lista que se llena durante el juego
- **Reparado**: bool que determina la victoria

---

## 🎮 **CONTROLES DEL JUEGO**

### **Movimiento**
- **WASD / Flechas**: Mover al científico
- **Espacio**: Atacar (consume 10 stamina)

### **Inventario**
- **I**: Abrir/cerrar inventario
- **Flechas**: Navegar entre items
- **Enter/E**: Usar item seleccionado
- **Delete/X**: Tirar item seleccionado
- **Escape**: Cerrar inventario
- **1-9**: Usar items rápidamente

### **Dimensiones**
- **Tab**: Cambiar dimensión (después de encontrar primer componente)

### **Estabilizador**
- **E**: Insertar componente (cuando estás cerca)

### **General**
- **R**: Reiniciar nivel (en pantallas de fin)

---

## 📦 **CREACIÓN DE SCRIPTABLEOBJECTS**

### **Items**
1. Clic derecho en Project → Create → Items → Item
2. Configurar:
   - **ID**: Número único (1-3 componentes, 10-15 comida, 16-20 agua, etc.)
   - **Type**: Food, Water, Components, o Weapon
   - **Icon**: Sprite del item
   - **Valor**: Efectos (nutrición/hidratación/daño)
   - **Nombre**: Nombre descriptivo
   - **Descripción**: Texto explicativo

### **Ejemplos de IDs Sugeridos**
- **Componentes**: 1, 2, 3
- **Comida**: 10, 11, 12, 13, 14, 15
- **Agua**: 16, 17, 18, 19, 20  
- **Armas**: 30, 31, 32, 33
- **Medicina**: 21, 22, 23, 24, 25

---

## 🏗️ **CONFIGURACIÓN DE CAPAS (LAYERS)**

### **Capas Necesarias**
1. Ve a Edit → Project Settings → Tags and Layers
2. Agrega las siguientes capas:
   - **Dim_Normal**: Para objetos en dimensión normal
   - **Dim_Altered**: Para objetos en dimensión alterada

### **Configuración de Objetos**
- **Estabilizador**: Tag "Estabilizador", visible solo en Dim_Normal
- **Enemigos**: Tag "Enemy", visibles solo en Dim_Altered  
- **Items sueltos**: Tag "Item", visibles en Dim_Normal

---

## 🚀 **ORDEN DE CONFIGURACIÓN RECOMENDADO**

### **Paso 1: Configuración Básica**
1. Crear GameManager vacío
2. Configurar Cientifico con componente Inventory
3. Configurar UIManager en Canvas
4. Asignar referencias básicas

### **Paso 2: Sistema de Items** 
1. Crear ScriptableObjects de items
2. Configurar array en InventoryUI
3. Configurar drops en enemigos

### **Paso 3: Sistemas Avanzados**
1. Agregar QuestManager y DimensionSwitcher al GameManager
2. Configurar capas de dimensiones
3. Configurar tags de objetos
4. Asignar todas las referencias

### **Paso 4: Testing**
1. Probar movimiento y ataque
2. Probar inventario y consumo
3. Probar cambio de dimensiones
4. Probar victoria (insertar componentes)

---

## ⚠️ **ERRORES COMUNES**

### **Referencias Faltantes**
- Siempre drag objetos desde la **escena**, no desde Project
- GameManager debe tener **todas** las referencias asignadas

### **Items No Aparecen**
- Verificar que ScriptableObjects están en el array de InventoryUI
- Verificar IDs únicos en cada item

### **Dimensiones No Cambian**
- Verificar que las capas existen en Project Settings
- Verificar que objetos tienen los tags correctos

### **UI No Funciona**
- Verificar que Canvas tiene GraphicRaycaster
- Verificar que EventSystem existe en la escena

---

Este sistema está completamente funcional y preparado para expandirse. Cada componente está diseñado para ser modular y fácil de configurar.
