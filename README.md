# AR Block Shooter

Mobile AR game built with Unity and AR Foundation. Place a block structure on a real-world surface and shoot balls to knock it down before running out of ammo.

## How to Play

1. Point your phone at a flat surface until a plane is detected.
2. Tap to place the structure.
3. Tap again to shoot balls at the blocks.
4. Knock down all the blocks before running out of ammo to win.
5. Tap anywhere on the game over screen to play again.

## Tech Stack

- Unity
- AR Foundation

## Project Structure

- `Assets/Scripts/StructureShooter.cs`: core gameplay loop (placement, shooting, win/lose)
- `Assets/Scripts/KnockableBlock.cs`: detects when a block has toppled
- `Assets/Scripts/StructureShooterUI.cs`: updates UI from game events
- `Assets/Scripts/AboutCardController.cs`: About panel (show/hide, website link)
