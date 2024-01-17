using UnityEngine;

public static class MouseData
{
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoveredOver;
    public static bool IsMouseBusy => interfaceMouseIsOver != null || tempItemBeingDragged != null || slotHoveredOver != null;
}