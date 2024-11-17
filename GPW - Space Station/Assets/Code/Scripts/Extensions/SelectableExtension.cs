using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class SelectableExtension
{
    public static void SetupNavigation(this Selectable selectable, Selectable selectOnUp = null, Selectable selectOnDown = null, Selectable selectOnLeft = null, Selectable selectOnRight = null)
    {
        // Create the custom navigation.
        Navigation customNavigation = new Navigation();
        customNavigation.mode = Navigation.Mode.Explicit;

        // Set the selection targets.
        customNavigation.selectOnUp = selectOnUp;
        customNavigation.selectOnDown = selectOnDown;
        customNavigation.selectOnLeft = selectOnLeft;
        customNavigation.selectOnRight = selectOnRight;

        // Assign the custom navigation.
        selectable.navigation = customNavigation;

    }
}
