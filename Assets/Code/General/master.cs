using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Container for the most important values.
/// </summary>
/// <remarks>
/// This is not used for simplifying tasks. See <see cref="helper"/> or <see cref="helperMono"/> instead
/// </remarks>
public static class master
{
    /// <summary> Internal representation of unit multiplier </summary>
    private static float _unitMultiplier = 1;
    /// <summary>
    /// Public representation of unit multiplier
    /// </summary>
    /// <remarks>
    ///     <para> Used to scale the in-game representations of planets. See the Assets/README.txt design philosphy section for more info on best practices </para>
    ///     <para> onUnitMultiplierChanged event is called everytime this value is changed- not whenever it is called (that said, you should still not make unnecessary calls) </para>
    /// </remarks>
    public static float unitMultiplier 
    {
        set 
        {
            if (_unitMultiplier != value) // only call event if its a new value
            {
                _unitMultiplier = value;
                onUnitMultiplierChanged(null, EventArgs.Empty);
            }
        }
        get {return _unitMultiplier;}
    }

    /// <summary> Event that is called whenever unitMultiplier changes </summary>
    /// <remarks> Expect all arguments to be null/default value </remarks>
    public static event EventHandler onUnitMultiplierChanged = delegate {};

    /// <summary> Internal representation of global time </summary>
    private static float _globalTime = 0;
    /// <summary>
    /// Public representation of unit multiplier
    /// </summary>
    /// <remarks>
    ///     <para> Used to sync/update the positions of objects. Positions should be calculated via its specific value, not by its change </para>
    ///     <para> onGlobalTimeChanged event is called everytime this value is changed- not whenever it is called (that said, you should still not make unnecessary calls) </para>
    /// </remarks>
    public static float globalTime
    {
        set
        {
            if (_globalTime != value) // only call event if its a new value
            {
                _globalTime = value;
                onGlobalTimeChanged(null, EventArgs.Empty);
            }
        }
        get {return _globalTime;}
    }
    /// <summary> Event that is called whenever globalTime changes </summary>
    /// <remarks> Expect all arguments to be null/default value </remarks>
    public static event EventHandler onGlobalTimeChanged = delegate {};

    /// <summary> Force calls time and unitMulti events </summary>
    /// <remarks> Used mainly to sync up values of a newly created object </remarks>
    public static void forceUpdateGlobalVars()
    {
        onUnitMultiplierChanged(null, EventArgs.Empty);
        onGlobalTimeChanged(null, EventArgs.Empty);
    }

    public static LoudDictionary<string, PLANETCLASS> planets = new LoudDictionary<string, PLANETCLASS>();

    public static LoudDictionary<string, FACILITYCLASS> buildings = new LoudDictionary<string, FACILITYCLASS>();

    public static GameObject referenceFrame = null;
}
