# FlappyBird (Unity)

Implementació base de Flappy Bird amb els requisits de l'enunciat:

- **Estat inicial** amb missatge: *"Tap screen or press X to fly"*.
- En començar, el personatge vola i es generen **pipes** contínuament.
- Es mostra sempre el **current score**.
- Quan l'ocell toca un pipe, el terra o el sostre, es mostra el panell de **Game Over** amb:
  - current score
  - best score
- El **best score** es desa amb persistència (`PlayerPrefs`).
- **Animació de l'ocell** amb sprites.
- Suport per afegir **sprites de pipes i fons** des de l'Inspector.

## Scripts inclosos

- `Assets/Scripts/GameManager.cs`
  - Control d'estats (`WaitingToStart`, `Playing`, `GameOver`)
  - Gestió de puntuació i persistència del rècord
  - Mostra/amaga panells UI
- `Assets/Scripts/BirdController.cs`
  - Salt amb tap/clic o tecla `X`
  - Física de vol i rotació
  - Animació per sprites
  - Detecció de col·lisions i trigger de puntuació
- `Assets/Scripts/PipeSpawner.cs`
  - Generació periòdica de pipes amb alçada aleatòria
- `Assets/Scripts/PipeMover.cs`
  - Desplaçament dels pipes cap a l'esquerra i destrucció fora de pantalla

## Configuració ràpida de l'escena

1. **Bird**
   - Afegeix `Rigidbody2D`, `Collider2D`, `SpriteRenderer`.
   - Afegeix `BirdController` i assigna:
     - `GameManager`
     - Array de sprites de vol (`flyingSprites`).

2. **Pipes**
   - Crea un prefab `PipePair` amb:
     - Pipe superior + `Collider2D`
     - Pipe inferior + `Collider2D`
     - Objecte fill `ScoreZone` amb `isTrigger=true` i tag `ScoreZone`
     - Script `PipeMover`
   - Assigna els sprites dels pipes a cada `SpriteRenderer`.

3. **Spawner**
   - Crea un GameObject `PipeSpawner` fora de pantalla a la dreta.
   - Afegeix script `PipeSpawner` i referencia el prefab `PipePair`.

4. **Límits (terra/sostre)**
   - Crea col·liders per terra i sostre perquè facin `GameOver` en col·lisionar amb l'ocell.

5. **UI**
   - Canvas amb:
     - `startPanel` (missatge inicial)
     - `gameOverPanel` (current + best)
     - `currentScoreText`
   - Connecta tots els camps al `GameManager`.

6. **Fons**
   - Afegeix un `SpriteRenderer` o `Image` amb el sprite de fons.

## Notes

- El best score es desa amb clau `BestScore`.
- Si vols resetar rècord: `PlayerPrefs.DeleteKey("BestScore")`.
