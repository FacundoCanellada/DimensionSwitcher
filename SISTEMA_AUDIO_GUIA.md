# 🎵 SISTEMA DE AUDIO - GUÍA COMPLETA

## ✅ **LO QUE YA ESTÁ HECHO:**

1. **AudioManager.cs** creado en `Assets/Scripts/Core/`
2. **UIButtonSound.cs** creado en `Assets/Scripts/UI/`
3. **UIManager.cs** actualizado con música para menús
4. **Integración automática de música** en victoria/derrota/menú/juego

---

## 📋 **CONFIGURACIÓN PASO A PASO**

### **1. CREAR EL AUDIOMANAGER EN LA ESCENA**

1. Hierarchy → Right-click → **Create Empty**
2. Renombrar a: **AudioManager**
3. **Add Component → Audio Manager**
4. El script creará automáticamente 2 AudioSources

---

### **2. CONSEGUIR AUDIOS (OPCIONES GRATUITAS)**

#### **🔹 Sitios recomendados:**
- **Freesound.org** - Efectos de sonido gratis
- **Incompetech.com** - Música gratis (Kevin MacLeod)
- **OpenGameArt.org** - Audio para juegos
- **Zapsplat.com** - SFX gratis
- **Pixabay.com/music** - Música libre de derechos

#### **🔹 Lo que necesitas:**
```
MÚSICA (4 pistas):
- Música de Menú (loop, ambient, menú)
- Música de Juego (loop, acción/aventura)
- Música de Victoria (celebración, corta)
- Música de Derrota (triste, game over)

SFX UI (4 sonidos):
- Click de botón
- Hover de botón
- Menú abierto
- Menú cerrado

SFX JUGADOR (6 sonidos):
- Paso (footstep)
- Ataque (swish, slash)
- Daño recibido (grunt, hit)
- Muerte (death)
- Recoger item (pickup, coin)
- Usar item (consume, use)

SFX ENEMIGOS (3 sonidos):
- Ataque enemigo
- Daño enemigo
- Muerte enemigo

SFX ESTABILIZADOR (2 sonidos):
- Componente colocado (insert, place)
- Estabilizador completo (success, victory)
```

---

### **3. IMPORTAR AUDIOS A UNITY**

1. Crea carpetas:
   ```
   Assets/Audio/
   ├─ Music/
   ├─ SFX/
   │  ├─ UI/
   │  ├─ Player/
   │  ├─ Enemy/
   │  └─ Estabilizador/
   ```

2. Arrastra tus archivos de audio (.wav, .mp3, .ogg) a las carpetas correspondientes

3. **Para MÚSICA:** Selecciona el audio → Inspector:
   - **Load Type:** `Streaming` (para archivos grandes)
   - **Compression Format:** `Vorbis`
   - **Quality:** `70-100`

4. **Para SFX:** Selecciona el audio → Inspector:
   - **Load Type:** `Decompress On Load` (para sonidos cortos)
   - **Compression Format:** `ADPCM`

---

### **4. ASIGNAR AUDIOS EN EL AUDIOMANAGER**

Selecciona **AudioManager** en Hierarchy:

```
Inspector → Audio Manager:

MÚSICA:
├─ Musica Menu: (arrastra tu audio de menú)
├─ Musica Juego: (arrastra tu audio de juego)
├─ Musica Victoria: (arrastra tu audio de victoria)
└─ Musica Derrota: (arrastra tu audio de derrota)

SFX - UI:
├─ Sonido Boton: (click)
├─ Sonido Boton Hover: (hover)
├─ Sonido Menu Abrir: (open menu)
└─ Sonido Menu Cerrar: (close menu)

SFX - JUGADOR:
├─ Sonido Paso: (footstep)
├─ Sonido Ataque: (attack swish)
├─ Sonido Daño Recibido: (player hurt)
├─ Sonido Muerte: (player death)
├─ Sonido Recoger Item: (pickup)
└─ Sonido Usar Item: (use item)

SFX - ENEMIGOS:
├─ Sonido Enemigo Ataque: (enemy attack)
├─ Sonido Enemigo Daño: (enemy hurt)
└─ Sonido Enemigo Muerte: (enemy death)

SFX - ESTABILIZADOR:
├─ Sonido Componente Colocado: (place component)
└─ Sonido Estabilizador Completo: (repair complete)

CONFIGURACIÓN:
├─ Volumen Musica: 0.5 (50%)
└─ Volumen SFX: 0.7 (70%)
```

---

### **5. AÑADIR SONIDOS A LOS BOTONES (AUTOMÁTICO)**

Para **TODOS los botones** de tu UI:

1. Selecciona el botón en el Canvas
2. **Add Component → UI Button Sound**
3. Configuración automática:
   - ✅ Reproducir Hover
   - ✅ Reproducir Click

**OPCIONAL:** Crea un prefab de botón con el componente ya añadido

---

## 🎮 **INTEGRACIÓN EN TU CÓDIGO**

### **YA INTEGRADO AUTOMÁTICAMENTE:**
✅ Música de menú al iniciar  
✅ Música de juego al empezar partida  
✅ Música de victoria/derrota  
✅ Sonidos de menú pausa  

### **FALTA INTEGRAR MANUALMENTE:**

#### **A) En Cientifico.cs - Sonidos del jugador**

Busca el método `Atacar()` y añade:
```csharp
void Atacar()
{
    // Tu código de ataque...
    
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoAtaque();
}
```

Busca el método `RecibirDaño()` y añade:
```csharp
public void RecibirDaño(int cantidad)
{
    // Tu código de daño...
    
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoDañoRecibido();
}
```

Busca donde el jugador muere y añade:
```csharp
if (salud <= 0)
{
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoMuerte();
    
    // Tu código de muerte...
}
```

#### **B) En Enemy.cs - Sonidos de enemigos**

Busca el método `AtacarJugador()` y añade:
```csharp
void AtacarJugador()
{
    // Tu código de ataque...
    
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoEnemigoAtaque();
}
```

Busca el método `RecibirDaño()` y añade:
```csharp
public void RecibirDaño(int cantidad)
{
    // Tu código de daño...
    
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoEnemigoDaño();
}
```

Busca donde el enemigo muere y añade:
```csharp
void Morir()
{
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoEnemigoMuerte();
    
    // Tu código de muerte...
}
```

#### **C) En EstabilizadorCuantico.cs**

Busca donde se coloca un componente y añade:
```csharp
void ColocarComponente()
{
    // Tu código...
    
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoComponenteColocado();
}
```

Busca donde se completa el estabilizador y añade:
```csharp
void Reparado()
{
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoEstabilizadorCompleto();
    
    // Tu código...
}
```

#### **D) En Item.cs o InventoryUI.cs**

Busca donde se recoge un item y añade:
```csharp
void RecogerItem()
{
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoRecogerItem();
    
    // Tu código...
}
```

Busca donde se usa un item y añade:
```csharp
void UsarItem()
{
    // AÑADIR SONIDO:
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoUsarItem();
    
    // Tu código...
}
```

---

## 🎵 **PASOS DE JUGADOR (ANIMACIÓN)**

Para sonidos de pasos, tienes 2 opciones:

### **OPCIÓN 1: Por código (simple)**
En `PlayerAnimationControllerSimple.cs`, en `Update()`:
```csharp
void Update()
{
    // Si el jugador se está moviendo
    if (estaMoviendo)
    {
        tiempoPasos += Time.deltaTime;
        if (tiempoPasos >= 0.3f) // Cada 0.3 segundos
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SonidoPaso();
            tiempoPasos = 0f;
        }
    }
}
```

### **OPCIÓN 2: Por Animation Events (preciso)**
1. Window → Animation → Animation
2. Selecciona animación de caminar
3. Añade **Animation Event** en el frame del paso
4. Función: `PlayFootstepSound`
5. En tu script de animación añade:
```csharp
void PlayFootstepSound()
{
    if (AudioManager.Instance != null)
        AudioManager.Instance.SonidoPaso();
}
```

---

## 🔊 **AJUSTAR VOLUMEN EN TIEMPO REAL**

En `MenuOpciones.cs` (si tienes sliders de volumen):

```csharp
void OnVolumenMusicaChanged(float valor)
{
    if (AudioManager.Instance != null)
        AudioManager.Instance.CambiarVolumenMusica(valor);
}

void OnVolumenSFXChanged(float valor)
{
    if (AudioManager.Instance != null)
        AudioManager.Instance.CambiarVolumenSFX(valor);
}
```

---

## 🎬 **EJEMPLO COMPLETO DE USO**

```csharp
// En cualquier parte de tu código:

// Reproducir música
AudioManager.Instance.ReproducirMusicaJuego();

// Reproducir efecto de sonido
AudioManager.Instance.SonidoAtaque();

// Cambiar volumen
AudioManager.Instance.CambiarVolumenMusica(0.5f); // 50%

// Pausar música
AudioManager.Instance.PausarMusica();

// Reanudar música
AudioManager.Instance.ReanudarMusica();
```

---

## ✅ **CHECKLIST FINAL**

- [ ] AudioManager creado en la escena
- [ ] Audios descargados e importados
- [ ] Audios asignados en Inspector del AudioManager
- [ ] UIButtonSound añadido a todos los botones
- [ ] Sonidos integrados en Cientifico.cs
- [ ] Sonidos integrados en Enemy.cs
- [ ] Sonidos integrados en EstabilizadorCuantico.cs
- [ ] Sonidos integrados en Item/InventoryUI
- [ ] Pasos del jugador configurados
- [ ] Probado en Play mode

---

## 🎯 **¿NECESITAS AYUDA?**

- **Para añadir un nuevo sonido:** Añade el AudioClip en AudioManager y crea un método público
- **Para probar sonidos:** `AudioManager.Instance.ReproducirSFX(tuClip)`
- **Para música custom:** Usa `AudioManager.Instance.musicSource.clip = tuClip`

---

**¡Sistema de audio completo y listo para usar!** 🎵
