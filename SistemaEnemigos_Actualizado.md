# Sistema de Enemigos y Dimensiones - CORRECCIÓN COMPLETA

## ⚠️ PROBLEMAS CORREGIDOS

### 1. **Enemigos visibles en dimensión incorrecta - ARREGLADO**
**Problema:** Los enemigos con layer `Dim_Altered` se veían en la dimensión normal.

**Causa:** El `DimensionSwitcher` cambiaba la visibilidad de TODOS los enemigos sin importar su layer.

**Solución:** Modificado `SetearDimension()` para usar el nuevo método `CambiarVisibilidadPorLayer()` que solo afecta enemigos del layer específico:
```csharp
// Ahora verifica el layer antes de cambiar visibilidad
if (obj.layer != layer) continue;
```

### 2. **Prefabs duplicados al arrastrar - ARREGLADO**
**Problema:** Al arrastrar prefabs a la escena, se creaban duplicados al iniciar.

**Causa:** El GameManager llamaba `RespawnearEnemigos()` incluso con enemigos pre-colocados.

**Solución:** 
- `Start()` ya NO llama `RespawnearEnemigos()` si `usarEnemigosPreColocados = true`
- `IniciarJuego()` ya NO respawnea, solo resetea enemigos existentes
- `RespawnearEnemigosPreColocados()` ahora inteligente: solo resetea posiciones sin destruir/recrear

---

## ✅ CÓMO USAR (DEFINITIVO)

### Configuración en GameManager (Inspector):
1. ✅ **Marcar:** `Usar Enemigos Pre Colocados`
2. **Asignar:** `enemigoPrefab` (el prefab base - solo se usa si un enemigo muere y necesita recrearse)
3. **Asignar:** Los 3 `Tipos Enemigos` (Slime_Data, Orc_Data, Predator_Data)

### Colocación de Enemigos en la Escena:

#### IMPORTANTE: Usa VARIANTES de prefabs, no instancias directas

**Por cada tipo de enemigo:**

1. **Crear variante de prefab:**
   - Click derecho en `Enemy.prefab` → Create → Prefab Variant
   - Nombrar: `Enemy_Slime.prefab`, `Enemy_Orc.prefab`, `Enemy_Predator.prefab`

2. **Configurar cada variante:**
   - Abrir la variante en el editor de prefabs
   - En el componente `Enemy`:
     - Asignar `Enemy Type Data` correspondiente (Slime_Data, Orc_Data, Predator_Data)
   - Guardar la variante

3. **Colocar en la escena:**
   - Arrastra las **VARIANTES** a la escena (NO el prefab base)
   - Colócalas donde quieras que aparezcan
   - Para cada enemigo en la escena, configura:
     - **Layer:** `Dim_Normal` o `Dim_Altered` (según donde quieres que aparezca)
     - Verifica que tenga **Tag:** "Enemy"

4. **Al iniciar el juego:**
   - El sistema guarda automáticamente todas las posiciones y configuraciones
   - Los enemigos aparecen SOLO en la dimensión correcta
   - Al morir/ganar, vuelven exactamente a sus posiciones originales

---

## 🎯 RESUMEN DE LA LÓGICA

### Sistema de Dimensiones (DimensionSwitcher.cs):
- **Dimensión Normal (false):**
  - ✅ Muestra: Enemigos en layer `Dim_Normal`, Estabilizador, Items
  - ❌ Oculta: Enemigos en layer `Dim_Altered`

- **Dimensión Alterada (true):**
  - ✅ Muestra: Enemigos en layer `Dim_Altered`
  - ❌ Oculta: Enemigos en layer `Dim_Normal`, Estabilizador, Items

### Sistema de Respawn (GameManager.cs):

**Si `usarEnemigosPreColocados = true`:**
1. `Start()` → Guarda estado inicial, NO crea enemigos
2. `IniciarJuego()` → Solo resetea enemigos existentes
3. `RespawnearEnemigos()` → Detecta si enemigos existen:
   - Si existen: Solo resetea posiciones sin destruir
   - Si no existen: Recrea desde configuración guardada

**Si `usarEnemigosPreColocados = false`:**
- Modo spawn automático en posiciones hardcodeadas (5 posiciones fijas)

---

## 📋 CHECKLIST PARA VERIFICAR

Antes de testear, verifica:

- [ ] GameManager tiene marcado `Usar Enemigos Pre Colocados`
- [ ] Creaste 3 variantes del prefab Enemy (Slime, Orc, Predator)
- [ ] Cada variante tiene su EnemyTypeData asignado
- [ ] Arrastraste las VARIANTES a la escena (no el prefab base)
- [ ] Cada enemigo en la escena tiene:
  - [ ] Tag: "Enemy"
  - [ ] Layer: `Dim_Normal` O `Dim_Altered`
  - [ ] EnemyTypeData asignado (visible en el inspector)

---

## 🐛 SI ALGO NO FUNCIONA

### Enemigos siguen viéndose en dimensión incorrecta:
1. Verifica en el Inspector que el layer sea correcto (`Dim_Normal` o `Dim_Altered`)
2. Verifica en Edit → Project Settings → Tags and Layers que:
   - Layer 8 = `Dim_Normal`
   - Layer 9 = `Dim_Altered`
3. Revisa la consola al cambiar dimensiones - debe decir cuántos enemigos se configuran

### Se siguen duplicando enemigos:
1. Asegúrate que `Usar Enemigos Pre Colocados` esté marcado en GameManager
2. Verifica en la consola al inicio - debe decir "Sistema configurado para usar enemigos pre-colocados"
3. NO debe aparecer el mensaje "=== CREANDO X ENEMIGOS ==="

### Enemigos no respawnean después de morir:
1. El GameManager debe tener asignado el `enemigoPrefab`
2. Los enemigos guardados inicialmente deben tener toda su configuración
3. Revisa la consola para ver si hay errores al guardar el estado inicial

---

## 🎮 EJEMPLO DE SETUP TÍPICO

```
Escena:
  - Enemy_Slime_1 (Layer: Dim_Altered, Posición: 5, 5)
  - Enemy_Slime_2 (Layer: Dim_Altered, Posición: -5, 5)
  - Enemy_Orc_1 (Layer: Dim_Altered, Posición: 5, -5)
  - Enemy_Predator_1 (Layer: Dim_Normal, Posición: 0, 8)  ← Este aparece en Normal

GameManager:
  ✅ Usar Enemigos Pre Colocados
  enemigoPrefab: Enemy (prefab base)
  Tipos Enemigos[0]: Slime_Data
  Tipos Enemigos[1]: Orc_Data
  Tipos Enemigos[2]: Predator_Data
```

**Comportamiento esperado:**
- Dimensión Normal: Solo ves Enemy_Predator_1
- Dimensión Alterada: Ves los 3 Slimes y el Orc, NO ves el Predator
- Al morir/reiniciar: Todos vuelven a sus posiciones originales
