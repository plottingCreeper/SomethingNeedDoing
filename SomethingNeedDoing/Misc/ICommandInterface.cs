namespace SomethingNeedDoing.Misc;

/// <summary>
/// Miscellaneous functions that commands/scripts can use.
/// </summary>
public interface ICommandInterface
{
    /// <summary>
    /// Get a value indicating whether the player is crafting.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsCrafting();

    /// <summary>
    /// Get a value indicating whether the player is not crafting.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsNotCrafting();

    /// <summary>
    /// Get a value indicating whether the current craft is collectable.
    /// </summary>
    /// <returns>A value indicating whether the current craft is collectable.</returns>
    public bool IsCollectable();

    /// <summary>
    /// Get the current synthesis condition.
    /// </summary>
    /// <param name="lower">A value indicating whether the result should be lowercased.</param>
    /// <returns>The current synthesis condition.</returns>
    public string GetCondition(bool lower = true);

    /// <summary>
    /// Get a value indicating whether the current condition is active.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="lower">A value indicating whether the result should be lowercased.</param>
    /// <returns>A value indcating whether the current condition is active.</returns>
    public bool HasCondition(string condition, bool lower = true);

    /// <summary>
    /// Get the current progress value.
    /// </summary>
    /// <returns>The current progress value.</returns>
    public int GetProgress();

    /// <summary>
    /// Get the max progress value.
    /// </summary>
    /// <returns>The max progress value.</returns>
    public int GetMaxProgress();

    /// <summary>
    /// Get a value indicating whether max progress has been achieved.
    /// This is useful when a crafting animation is finishing.
    /// </summary>
    /// <returns>A value indicating whether max progress has been achieved.</returns>
    public bool HasMaxProgress();

    /// <summary>
    /// Get the current quality value.
    /// </summary>
    /// <returns>The current quality value.</returns>
    public int GetQuality();

    /// <summary>
    /// Get the max quality value.
    /// </summary>
    /// <returns>The max quality value.</returns>
    public int GetMaxQuality();

    /// <summary>
    /// Get a value indicating whether max quality has been achieved.
    /// </summary>
    /// <returns>A value indicating whether max quality has been achieved.</returns>
    public bool HasMaxQuality();

    /// <summary>
    /// Get the current durability value.
    /// </summary>
    /// <returns>The current durability value.</returns>
    public int GetDurability();

    /// <summary>
    /// Get the max durability value.
    /// </summary>
    /// <returns>The max durability value.</returns>
    public int GetMaxDurability();

    /// <summary>
    /// Get the current CP amount.
    /// </summary>
    /// <returns>The current CP amount.</returns>
    public int GetCp();

    /// <summary>
    /// Get the max CP amount.
    /// </summary>
    /// <returns>The max CP amount.</returns>
    public int GetMaxCp();

    /// <summary>
    /// Get the current GP amount.
    /// </summary>
    /// <returns>The current GP amount.</returns>
    public int GetGp();

    /// <summary>
    /// Get the max GP amount.
    /// </summary>
    /// <returns>The max GP amount.</returns>
    public int GetMaxGp();

    /// <summary>
    /// Get the current step value.
    /// </summary>
    /// <returns>The current step value.</returns>
    public int GetStep();

    /// <summary>
    /// Get the current percent HQ (and collectable) value.
    /// </summary>
    /// <returns>The current percent HQ value.</returns>
    public int GetPercentHQ();

    /// <summary>
    /// Gets a value indicating whether any of the player's worn equipment is broken.
    /// </summary>
    /// <param name="below">Return true if the gear durability is less than or eqal to this number.</param>
    /// <returns>A value indicating whether any of the player's worn equipment is broken.</returns>
    public bool NeedsRepair(float below = 0);

    /// <summary>
    /// Gets a value indicating whether any of the player's worn equipment can have materia extracted.
    /// </summary>
    /// <param name="within">Return false if the next highest spiritbond is >= this value.</param>
    /// <returns>A value indicating whether any of the player's worn equipment can have materia extracted.</returns>
    public bool CanExtractMateria(float within = 100);

    /// <summary>
    /// Gets a value indicating whether the required crafting stats have been met.
    /// </summary>
    /// <param name="craftsmanship">Craftsmanship.</param>
    /// <param name="control">Control.</param>
    /// <param name="cp">Crafting points.</param>
    /// <returns>A value indcating whether the required crafting stats bave been met.</returns>
    public bool HasStats(uint craftsmanship, uint control, uint cp);

    /// <summary>
    /// Gets a value indicating whether the given status is present on the player.
    /// </summary>
    /// <param name="statusName">Status name.</param>
    /// <returns>A value indicating whether the given status is present on the player.</returns>
    public bool HasStatus(string statusName);

    /// <summary>
    /// Gets a value indicating whether the given status is present on the player.
    /// </summary>
    /// <param name="statusIDs">Status IDs.</param>
    /// <returns>A value indicating whether the given status is present on the player.</returns>
    public bool HasStatusId(params uint[] statusIDs);

    /// <summary>
    /// Gets a value indicating whether an addon is visible.
    /// </summary>
    /// <param name="addonName">Addon name.</param>
    /// <returns>A value indicating whether an addon is visible.</returns>
    public bool IsAddonVisible(string addonName);

    /// <summary>
    /// Gets a value indicating whether an addon is ready to be used. It may not be visible.
    /// </summary>
    /// <param name="addonName">Addon name.</param>
    /// <returns>A value indicating whether an addon is ready to be used.</returns>
    public bool IsAddonReady(string addonName);

    /// <summary>
    /// Get the text of a TextNode by its index number. You can find this by using the addon inspector.
    /// In general, these numbers do not change.
    /// </summary>
    /// <param name="addonName">Addon name.</param>
    /// <param name="nodeNumbers">Node numbers, can fetch nested nodes.</param>
    /// <returns>The node text.</returns>
    public string GetNodeText(string addonName, params int[] nodeNumbers);

    /// <summary>
    /// Get the text of a 0-indexed SelectIconString entry.
    /// </summary>
    /// <param name="index">Item number, 0 indexed.</param>
    /// <returns>The item text, or an empty string.</returns>
    public string GetSelectStringText(int index);

    /// <summary>
    /// Get the text of a 0-indexed SelectIconString entry.
    /// </summary>
    /// <param name="index">Item number, 0 indexed.</param>
    /// <returns>The item text, or an empty string.</returns>
    public string GetSelectIconStringText(int index);

    /// <summary>
    /// Get the status of a given character condition.
    /// </summary>
    /// <param name="flagID">Flag ID.</param>
    /// <param name="hasCondition">Bool flag to invert the condition check.</param>
    /// <returns>Returns true if the player has the condition, false otherwise. Condition inverted if provided with hasCondition=false.</returns>
    public bool GetCharacterCondition(int flagID, bool hasCondition = true);

    /// <summary>
    /// Get the status of the player being in a given zone.
    /// </summary>
    /// <param name="zoneID">Zone ID/Territory ID.</param>
    /// <returns>Returns true if the player is in given zone, false otherwise.</returns>
    public bool IsInZone(int zoneID);

    /// <summary>
    /// Gets the current zone's ID.
    /// </summary>
    /// <returns>Returns the current zone's ID, int.</returns>
    public int GetZoneID();

    /// <summary>
    /// Gets the character's name.
    /// </summary>
    /// <param name="includeWorld">Bool to include the world in the string.</param>
    /// <returns>Returns the current's name, string.</returns>
    public string GetCharacterName(bool includeWorld = false);

    /// <summary>
    /// Gets an item count from your inventory.
    /// </summary>
    /// <param name="itemID">Item ID.</param>
    /// <param name="includeHQ">Include HQ in count.</param>
    /// <returns>Returns the count of a given item in your inventory, int.</returns>
    public int GetItemCount(int itemID, bool includeHQ = true);

    /// <summary>
    /// Sends an IPC request to Deliveroo to check if the turn in function is running.
    /// </summary>
    /// <returns>Returns value of turn in function running, bool.</returns>
    public bool DeliverooIsTurnInRunning();

    /// <summary>
    /// Get the amount of progress a crafting action will give.
    /// </summary>
    /// <param name="actionID">Action ID.</param>
    /// <returns>Returns amount of increase in progress a given action will make in a craft, uint.</returns>
    public uint GetProgressIncrease(uint actionID);

    /// <summary>
    /// Get the amount of quality a crafting action will give.
    /// </summary>
    /// <param name="actionID">Action ID.</param>
    /// <returns>Returns amount of increase in quality a given action will make in a craft, uint.</returns>
    public uint GetQualityIncrease(uint actionID);

    /// <summary>
    /// Leaves an instance.
    /// </summary>
    public void LeaveDuty();
}
