/// Level of cooking of an enemy
/// We won't be changing those levels much, so an enum is fine, we don't need a full data-driven system.
public enum CookLevel
{
    /// Not cooked at all, or too little to make a difference
    Rare,
    /// Cooked beyond the first threshold
    Medium,
    /// Cooked beyond the second threshold
    WellDone,
    /// Cooked beyond the third threshold
    Carbonized
}
