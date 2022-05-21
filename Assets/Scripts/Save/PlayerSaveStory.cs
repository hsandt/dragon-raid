/// Player Save for Story mode
/// It is a different struct from PlayerSaveArcade because when we add cinematics, Story save will contain data slightly
/// different from Arcade, such as the next cinematic to play (this allows precise save resume, and removes the need
/// to play the first cinematic following the previous level for safety, as done in Freedom Planet)
public struct PlayerSaveStory
{
    /// Index of the level to load when the player loads this Save
    /// As soon as the player finishes a level, this is incremented in the current save
    public int nextLevelIndex;
}
