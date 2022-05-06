using UnityEngine;
using UnityEngine.SceneManagement;
using BepInEx;
using HarmonyLib;

namespace NoSword
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        PlayerController playerController;
        PlayerActor playerActor = null;
        SpriteRenderer playerSprite = null;
        Rigidbody2D playerBody = null;
        private void Awake()
        {
            Logger.LogInfo("The power of \"no bitches\" compels you. You can now jump.");
            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Main")
            {
                // Getting player stuff
                playerController = GameObject.Find("[PlayerController]").GetComponent<PlayerController>();
                playerActor  = GameObject.Find("[PlayerController]/[Player]").GetComponent<PlayerActor>();
                playerSprite = GameObject.Find("[PlayerController]/[Player]/Model/Sprite").GetComponent<SpriteRenderer>();
                playerBody   = playerActor.gameObject.GetComponent<Rigidbody2D>();
            }
        }

        const double defaultJumpForce = 10;
        double jumpForce = defaultJumpForce;
        private void Update()
        {
            if (playerController._allowControl && playerActor._isGrounded)
            {
                if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    if (jumpForce < 50)
                        jumpForce += (60 * Time.deltaTime);
                    playerSprite.transform.localScale = new Vector2(
                        playerSprite.transform.localScale.x,
                        0.5f + ((float)jumpForce / 90)
                    );
                    // Logger.LogInfo($"jumpForce = {jumpForce}");
                }
                else if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
                {
                    playerBody.velocity = new Vector2(
                        playerBody.velocity.x,
                        (float)jumpForce
                    );
                    jumpForce = defaultJumpForce;
                    playerSprite.transform.localScale = Vector2.one;
                }
            }
            else
            {
                jumpForce = 0f;
            }
        }
    }
}

// Patch to enable in-air movement
[HarmonyPatch(typeof(PlayerController))]
class PlayerControllerPatch
{
    static bool isCurrentlyGrounded;

    #region 
    [HarmonyPatch("UpdateControls")]
    static void Prefix(PlayerController __instance)
    {
        isCurrentlyGrounded = __instance._playerActor._isGrounded;
        __instance._playerActor._isGrounded = true;
    }

    [HarmonyPatch("UpdateControls")]
    static void Postfix(PlayerController __instance)
    {
        __instance._playerActor._isGrounded = isCurrentlyGrounded;
    }
    #endregion
}

// Lil anti-cheat patch
[HarmonyPatch(typeof(Main))]
class MainPatch
{
    [HarmonyPatch("Start")]
    static void Prefix(Main __instance)
    {
        __instance._playtime = 60000;
    }
}