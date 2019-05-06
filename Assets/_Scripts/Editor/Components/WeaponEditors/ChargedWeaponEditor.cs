using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChargedWeapon))]
public class ChargedWeaponEditor : BaseWeaponEditor
{
    private double changeSprite = 0;
    private int index = 0;
    private int chargeIndex = 0;
    private int collisionIndex = 0;
    private int loopChargedAnim = 0;
    private bool startAnimation = false;
    private bool playChargedAnimation = false;
    private bool playChargeAnimation = true;
    private bool playFireAnimation = false;
    private double animationStartTime;

    public override void OnInspectorGUI()
    {
		string tooltip;
        var chargedWeapon = serializedObject.targetObject as ChargedWeapon;
        EditorGUILayout.Space();
        if (!startAnimation)
        {
            animationStartTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += Animate;
            startAnimation = true;
        }

        // ----- PROPERTIES SECTION ----- //
        DisplayItemProperties(false);
		chargedWeapon.itemType = ItemType.Turret;
		chargedWeapon.visible = true;

		// ----- WEAPON STATS SECTION ----- //
		EditorGUILayout.LabelField("Weapon Stats", EditorStyles.boldLabel);
		DisplayProjectile();
		DisplayShotspread();

		EditorGUILayout.BeginHorizontal();
        tooltip = "The amount of time it takes for the weapon to cooldown before being able to fire again. The speed of the cooldown animation is determined by this value.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeToCooldown"), new GUIContent("    Cooldown Time", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tooltip = "The amount of time it takes for the weapon to decharge. Decharging occurs when the user releases the fire button before the weapon is charged." +
                  "The speed of the de-charge animation is determined by this value.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeToDecharge"), new GUIContent("    De-charge Time", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

		// ----- AUDIO/VISUAL SECTION ----- //
		EditorGUILayout.LabelField("Audio/Visual", EditorStyles.boldLabel);
		DisplayAnimation();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargingAnimation"), new GUIContent("    Charging Animation"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargedAnimationLoop"), new GUIContent("    Charged Animation"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldownAnimation"), new GUIContent("    Cooldown Animation"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeSound"), new GUIContent("    Charging Sound"), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        tooltip = "This sound is looped so long as the weapon is charged.";
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("chargedSound"), new GUIContent("    Charged Sound", tooltip), true, GUILayout.MaxWidth(500f));
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.EndHorizontal();

    }

    /// <summary>
	/// Plays the fire animation (if it exists), then plays the travel animation 3 times, then plays the collision effect.
	/// If the fire and collision animations don't exist, then the travel animation just loops.
	/// </summary>
	private void Animate()
    {
        var weapon = serializedObject.targetObject as ChargedWeapon;
        if (weapon == null)
            return;
        var rend = weapon.GetComponentInChildren<SpriteRenderer>();
        if (playChargeAnimation)
        {
            if (weapon.chargingAnimation != null)
            {
                if (EditorApplication.timeSinceStartup > changeSprite + animationStartTime)
                {
                    rend.sprite = weapon.chargingAnimation.GetFrame(chargeIndex);
                    chargeIndex++;
                    if (chargeIndex == weapon.chargingAnimation.frames.Length)
                    {
                        chargeIndex = 0;
                        playChargeAnimation = false;
                        playChargedAnimation = true;
                    }
                    else
                        changeSprite += weapon.chargeSound.length / weapon.chargingAnimation.frames.Length;
                }
            }
            else
            {
                playChargeAnimation = false;
                playChargedAnimation = true;
            }
        }
        else if (playChargedAnimation)
        {
            if (weapon.chargedAnimationLoop != null)
            {
                if (EditorApplication.timeSinceStartup > changeSprite + animationStartTime)
                {
                    rend.sprite = weapon.chargedAnimationLoop.GetFrame(index);
                    index++;
                    if (index == weapon.chargedAnimationLoop.frames.Length)
                    {
                        index = 0;
                        if (loopChargedAnim == 3)
                        {
                            loopChargedAnim = 0;
                            playChargedAnimation = false;
                            playFireAnimation = true;
                        }
                        else
                            loopChargedAnim++;
                    }
                    else
                        changeSprite += weapon.chargedAnimationLoop.playSpeed;
                }
            }
            else if (!weapon.animated && weapon.itemSprite != null)
            {
                rend.sprite = weapon.itemSprite;
                if (EditorApplication.timeSinceStartup > changeSprite + 2.0f + animationStartTime)
                {
                    playChargedAnimation = false;
                    playFireAnimation = true;
                    changeSprite += 2.0f;
                }
            }
            else
            {
                playChargedAnimation = false;
                playFireAnimation = true;
            }
        }
        else if (playFireAnimation)
        {
            if (weapon.cooldownAnimation != null)
            {
                if (EditorApplication.timeSinceStartup > changeSprite + animationStartTime)
                {
                    rend.sprite = weapon.cooldownAnimation.GetFrame(collisionIndex);
                    collisionIndex++;
                    if (collisionIndex == weapon.cooldownAnimation.frames.Length)
                    {
                        collisionIndex = 0;
                        playFireAnimation = false;
                    }
                    else
                        changeSprite += weapon.timeToCooldown / weapon.cooldownAnimation.frames.Length;
                }
            }
            else
                playFireAnimation = false;

        }
        else
        {
            playChargeAnimation = true;

        }
    }

    void OnDisable()
    {
        if (startAnimation)
        {
            startAnimation = false;
            EditorApplication.update -= Animate;
        }
    }

}
