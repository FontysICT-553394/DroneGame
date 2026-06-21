## Implementatiedocument structuur

> Status: deels ingevuld op basis van bestaande Unity-scripts. Alles wat ik niet met zekerheid uit het project kan afleiden staat als **[VUL IN]** of **TODO**.

### 1. Introductie

Korte uitleg:

- **Welk prototype heb je gemaakt?**
  - Een speelbaar **drone-racing prototype** met:
    - oneindig doorlopende track (segmenten)
    - ground + air obstacles op nodes
    - batteries/currency + score
    - finish line + eindscherm
    - eerste moving obstacle (vliegtuig) prototype

- **Wat is het doel van het prototype?**
  - [VUL IN] (bijv. aantonen dat core gameplay “drone racing + obstacles + score + finish” werkt)

- **Voor welk project/game is het?**
  - [VUL IN] (naam project/game + opdracht/context)

---

### 2. Link met design

Beschrijf waar je implementatie vandaan komt:

- **Gebruikte bron(nen)**
  - [VUL IN] (designdocument/Figma/GDD/prototype + link)

- **Gameplay requirements die je wilde realiseren**
  - [VUL IN] (bulletlijst met requirements)

- **Feedback/designkeuze vooraf**
  - [VUL IN] (welke feedback leidde tot welke keuze)

Voorbeeld (invulbaar):

> De drone movement is gebaseerd op de designkeuze dat de game race-achtig blijft, maar wel het gevoel moet geven van het besturen van een drone.

---

### 3. Gebruikte technologie

Concreet gebruikt in dit prototype:

- **Unity versie**
  - `6000.4.0f1` (uit `ProjectSettings/ProjectVersion.txt`)

- **Programmeertaal**
  - C# (Unity MonoBehaviours)

- **Physics**
  - Rigidbody (o.a. track movement, speler movement)
  - Colliders + triggers (currency, finish, obstacles)

- **Prefabs**
  - Track segment prefab (`trackSegmentPrefab`)
  - Finish line prefab (`finishLinePrefab`)
  - Battery prefab (`batteryPrefab`)
  - Ground obstacle prefabs (`ObstaclePrefabs`)
  - Air obstacle prefabs (`AirObstaclePrefabs`)
  - Plane prefab (`planePrefab`)
  - Banner prefab (`bannerPrefab`)

- **Input**
  - Unity Input System (`UnityEngine.InputSystem`)

- **UI / Text**
  - TextMeshPro (`TMPro.TextMeshProUGUI`)

- **Scripts (belangrijkste)**
  - `RacingTrackGenerator` (track spawning + finish plaatsen)
  - `DestoryRacingTrack` (oude segmenten verwijderen)
  - `GenerateObstacles` (ground/air obstacles + batteries + plane nodes)
  - `DroneMovementRacingDrone` (speler movement)
  - `HandleCollisionDroneRacing` (triggers: currency/finish)
  - `DroneRacing_ScoreSystem` (score bijhouden)
  - `DroneRacing_ShowFinishUI` (eindpaneel)
  - `DroneRacing_PlaneMovement` (moving obstacle movement)
  - `DroneRacing_RotateBattery` (battery visual feedback)
  - `ShowUIWhenOffScreen` (UI tonen als speler buiten beeld is)

---

### 4. Implementatie per systeem

#### 4.1 Track generation

**Wat doet het systeem?**
- Zorgt dat er altijd een vaste hoeveelheid track-segmenten aanwezig is (standaard 5).
- Plaatst een finish line na een instelbaar aantal geplaatste segmenten.
- Geeft elk tracksegment een constante “scroll” snelheid.

**Welke scripts?**
- `RacingTrackGenerator`
- `DestoryRacingTrack`

**Hoe werkt het technisch?**
- In `Start()` wordt het eerste tracksegment op (0,0,0) gespawned en toegevoegd aan een lijst.
- In `Update()`:
  - elk tracksegment krijgt een `Rigidbody.linearVelocity = Vector3.right * 8f` zodat de wereld langs de speler beweegt.
  - zolang er minder dan `trackSegmentAmount` segmenten zijn, worden er segmenten bijgemaakt.
  - zodra `tracksPlaced >= tracksToPlaceForFinish` wordt de finish geplaatst en wordt track-placing gestopt.

**Codefragment (kern)**
```csharp
// Track blijft bewegen
trackSegmentRb.linearVelocity = Vector3.right * 8f;

// Nieuwe track segments bijmaken tot we aan het gewenste aantal zitten
for (trackSegmentCounter = trackSegments.Count; trackSegmentCounter < trackSegmentAmount; trackSegmentCounter++)
{
    AddTrackSegment();
}
```

**Verwijderen oude segments**
- `DestoryRacingTrack` checkt of de bounds van een “destroy trigger” overlappen met de collider van het eerste tracksegment.
- Als dat zo is: segment uit de lijst halen + `Destroy()`.

---

#### 4.2 Obstacle generation

**Wat doet het systeem?**
- Spawnt per tracksegment:
  - ground obstacles op `ObstacleNode` nodes
  - air obstacles op `AirObstacleNode` nodes
  - banners op `BannerNode` nodes (kans-gebaseerd)
  - plane nodes (`PlaneObstacleNode`) worden queued en pas gespawned als de speler dichtbij is
- Spawnt daarna batteries op alle nog lege nodes.

**Welke scripts?**
- `GenerateObstacles`

**Hoe werkt het technisch? (nodes/rows)**
- Onder elk tracksegment worden “rows” gezocht via tags:
  - ground: tag `ObstacleRow`
  - air: tag `AirObstacleRow`
- Binnen rows worden nodes gezocht via tags:
  - ground: `ObstacleNode`
  - air: `AirObstacleNode`
  - plane: `PlaneObstacleNode`
  - banner: `BannerNode`
- `nodeStatus` houdt per node bij of die `Empty` of `Occupied` is.
- Per row wordt een random aantal nodes gekozen (max 2) en gevuld.

**Random spawning (unieke nodes)**
- `ChooseRandomNodes(...)` gebruikt een `HashSet<Transform>` zodat gekozen nodes uniek zijn.

**Batteries op lege nodes**
- Na obstacles worden alle nodes die nog `Empty` zijn gevuld met `batteryPrefab` (met hoogte-offset).

---

#### 4.3 Player movement

**Hoe beweegt de drone?**
- De drone gebruikt het Input System om een `Vector2` input te lezen.
- De beweging is “drone-ish” gemaakt door:
  - input vertraging (smoothing)
  - accelereren richting target velocity (geen directe snap)
  - extra afremmen als er geen input is

**Welke parameters zijn getweakt?**
- `moveSpeed`
- `inputLatency` (lager = meer vertraging)
- `acceleration`
- `dragWhenNoInput`

**Waarom voelt dit als drone/racing?**
- De input volgt niet instant, waardoor het gewicht/inertia simuleert.
- De drone accelereert naar snelheid, wat meer “floaty” aanvoelt dan direct verplaatsen.

**Codefragment (kern)**
```csharp
// input smoothing


delayedInput = Vector2.Lerp(delayedInput, movementInput, inputLatency * Time.fixedDeltaTime);

// velocity naar target toe accelereren
rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
```

---

#### 4.4 Score / batteries

**Waarom bestaan batterijen?**
- Batterijen zijn “currency” die de speler motiveert om risico’s te nemen en actief te sturen.
- [VUL IN] (bijv. later te gebruiken voor upgrades/energie?)

**Hoe wordt score verhoogd?**
- Speler raakt een trigger met tag `Currency`.
- `HandleCollisionDroneRacing` verwijdert de battery en roept `scoreSystem.AddScore(scorePerCurrency)` aan.

**Hoe spawnen ze?**
- In `GenerateObstacles.PlaceBatteryOnEmptyNodes()`:
  - na obstacle placement
  - op alle nodes die nog `Empty` zijn
  - behalve nodes met tag `PlaneObstacleNode` (gereserveerd)

---

#### 4.5 Finish logic

**Wanneer verschijnt finish?**
- Na `tracksToPlaceForFinish` geplaatste segments (default: 10).
- `RacingTrackGenerator` zet `stopPlacingTracks = true` zodra de finish is geplaatst.

**Eindscherm**
- `DroneRacing_ShowFinishUI` heeft een `finishLinePanel` dat standaard uit staat.
- Bij finish collision wordt het panel geactiveerd en wordt score getoond.

**Hoe wordt game afgerond?**
- In `HandleCollisionDroneRacing`:
  - bij tag `FinishLine` → `showFinishUI.ShowFinishLinePanel(scoreSystem.CurrentScore)`
- TODO: [VUL IN] (pauzeren? input lock? restart button?)

---

#### 4.6 Moving obstacle prototype

**Vliegtuig-obstakel**
- Plane nodes (`PlaneObstacleNode`) worden verzameld tijdens obstacle generation.
- Planes worden niet direct gespawned: ze worden queued (`pendingPlaneNodes`).
- Pas als de speler binnen `planeSpawnDistance` komt, wordt `planePrefab` geïnstantieerd.

**Wat werkt al?**
- Spawning op basis van afstand tot speler.
- Beweging via `DroneRacing_PlaneMovement`: plane beweegt “links” relatief aan de camera.

**Wat moet nog verbeterd worden?**
- [VUL IN] (bijv. collision/damage, timing, spawn fairness, pooling/performance, visuals/animatie)

---

### 5. Assets

Laat zien wat jij zelf hebt gemaakt of aangepast:

- racing track model: [VUL IN]
- battery model: [VUL IN]
- traffic cone: [VUL IN]
- banner: [VUL IN]
- drone/player: [VUL IN]
- vliegtuig obstacle: [VUL IN]

Zet erbij:

> Deze assets zijn low-poly gemaakt zodat ze snel te produceren zijn en passen bij de stijl van het prototype.

---

### 6. Validatie / testen

Heel belangrijk voor Realization.

- **Hoe je getest hebt**
  - [VUL IN]

- **Wat je hebt gecontroleerd**
  - [VUL IN]

- **Wat werkte**
  - [VUL IN]

- **Wat niet werkte**
  - [VUL IN]

- **Wat je hebt aangepast na feedback**
  - [VUL IN]

Voorbeeld (invulbaar):

> Ik heb getest of er altijd vijf trackdelen aanwezig blijven, of oude trackdelen worden verwijderd en of batterijen alleen op lege nodes spawnen.

---

### 7. Problemen en oplossingen

Voorbeelden uit dit project (vul aan waar nodig):

- **Unity crash door for-loop**
  - Probleem: track generator crashte door een fout in de for-loop/plaats-logica.
  - Oplossing: for-loop/logica aangepast zodat het aantal tracksegmenten correct begrensd wordt.

- **Pivot point problemen bij animated models**
  - Probleem: modellen met losse onderdelen verschoven pivot/alignement waardoor placement op nodes niet klopte.
  - Oplossing: modellen vereenvoudigd en animatie uitgesteld (nice-to-have).

- **Vliegtuig obstacle werkt nog niet volledig**
  - Status: prototype aanwezig (spawning + movement), verdere afwerking volgt.

---

### 8. Resultaat

Huidige staat van het prototype:

- Speelbaar prototype aanwezig.
- Player kan bewegen (Input System + Rigidbody).
- Obstacles werken (ground + air + banners).
- Score werkt (currency → score).
- Finish werkt (finish line → eindscherm met score).
- Eerste moving obstacle prototype aanwezig (vliegtuig).

---

### 9. Bewijs

Voeg screenshots toe van:

- Unity scene: TODO
- scripts (relevante snippets): TODO
- prefabs: TODO
- inspector instellingen: TODO
- gameplay: TODO
- score/finish screen: TODO
- assets in Blender/Unity: TODO
- Git commits: TODO

---

### 10. Conclusie

- [VUL IN] (wat is er bewezen met dit prototype, wat is de volgende stap, welke risico’s/lessons learned)
