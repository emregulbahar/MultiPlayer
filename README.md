# MultiPlayer

Unity 6 ile geliştirilmiş, Netcode for GameObjects tabanlı çok oyunculu kaynak toplama oyunu. Oyuncular birlikte ağaç ve taş toplayıp paletlere teslim ederek hedefi tamamlar.

## Gereksinimler

| Bileşen | Sürüm |
|---------|-------|
| Unity Editor | `6000.5.2f1` |
| Render Pipeline | URP (`com.unity.render-pipelines.universal`) |
| Ağ | Netcode for GameObjects `2.13.0` |
| Girdi | Input System `1.19.0` |

## Hızlı Başlangıç

1. Projeyi Unity Hub üzerinden açın (Unity `6000.5.2f1` gerekli).
2. `Assets/Scenes/GameScene.unity` sahnesini açın.
3. **Play** tuşuna basın.
4. Açılan arayüzden **Host** ile sunucuyu başlatın veya başka bir editör/oyuncu örneğinde **Client** ile bağlanın.

Varsayılan bağlantı adresi: `127.0.0.1:7777`

### Çoklu editör ile test

Unity Multiplayer Play Mode paketi kurulu olduğundan aynı makinede birden fazla oyuncu test edilebilir:

1. **Window → Multiplayer → Multiplayer Play Mode** menüsünü açın.
2. Bir örnek **Host**, diğerini **Client** olarak yapılandırın.
3. Her iki örnekte de `GameScene` sahnesini çalıştırın.

## Oynanış

Oyuncular birlikte kaynak toplayıp paletlere teslim eder. Tüm paletler dolduğunda sahne yeniden yüklenir.

### Kontroller

| Tuş / Girdi | İşlev |
|-------------|-------|
| Hareket (WASD / gamepad) | Karakteri hareket ettir |
| `E` | En yakın etkileşimli nesneyi al / palete teslim et |
| Sol tık | Elinde balta veya kazma varken kaynak düğümünü kes / kır |

### Oyun döngüsü

1. **Araç al** — Balta (`Axe`) ağaç, kazma (`PickAxe`) taş düğümleri için gereklidir.
2. **Kaynak topla** — Uygun araçla kaynak düğümüne saldır; düğüm yok olunca tahta veya taş düşer.
3. **Taşı** — Düşen kaynakları `E` ile al.
4. **Teslim et** — Doğru türdeki kaynağı ilgili palete `E` ile bırak.
5. **Kazan** — Tüm paletler dolduğunda oyun yeniden başlar.

### Nesne türleri

```csharp
None, Axe, PickAxe, Wood, Stone
```

## Proje Yapısı

```
Assets/
├── _Scripts/           # Oyun mantığı (ağ, etkileşim, oyuncu)
├── _NetworkPrefabs/    # Ağ üzerinden spawn edilen prefab'lar
├── _UI Documents/      # UI Toolkit bağlantı arayüzü
├── _Assets/            # KayKit 3D asset paketleri
├── Scenes/
│   ├── GameScene.unity # Ana oyun sahnesi
│   └── SampleScene.unity
└── Settings/           # URP render ayarları
```

### Önemli scriptler

| Script | Görev |
|--------|-------|
| `GameManager.cs` | Host/Client başlatma, oyuncu spawn, kazanma koşulu |
| `PlayerController.cs` | Hareket, etkileşim, envanter (NetworkVariable) |
| `InteractionDetector.cs` | Yakındaki etkileşimli nesneleri algılar |
| `ResourceNode.cs` | Araçla hasat edilen kaynak düğümleri |
| `ResourcePallet.cs` | Kaynak teslim noktaları |
| `ResourceSpawner.cs` | Sunucu tarafında tahta/taş spawn |
| `MultiplayerUI.cs` | Host / Client / Disconnect butonları |

## Ağ Yapılandırması

Oyun **Unity Transport (UTP)** kullanır. Bağlantı ayarları `GameScene` içindeki `NetworkManager` nesnesinde tanımlıdır:

## Geliştirme

```bash
# Unity Editor içinden test
# Edit → Play → GameScene

# Build (Unity Editor)
# File → Build Settings → GameScene ekli → Build
```

Build Settings'te kayıtlı sahneler:

- `Assets/Scenes/SampleScene.unity`
- `Assets/Scenes/GameScene.unity`

## Üçüncü Taraf Assetler

Proje aşağıdaki ücretsiz KayKit paketlerini içerir (lisans dosyaları ilgili klasörlerde):

- KayKit Character Animations
- KayKit Forest Nature Pack
- KayKit Resource Bits
- KayKit Block Bits
- KayKit RPG Tools Bits

## Lisans

Bu depodaki özgün oyun kodu için lisans belirtilmemiştir. KayKit asset paketlerinin kullanım koşulları için `Assets/_Assets/` altındaki `License.txt` dosyalarına bakın.
