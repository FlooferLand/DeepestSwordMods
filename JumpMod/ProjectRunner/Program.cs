using System.IO;
using System.Diagnostics;

// Global variables
string dsDir;
string dsPlugins;
string dsExec;
string pluginFileInput;
string pluginFileOutput;

// Setting variables
using (StreamReader sr = new StreamReader("./BuildVars/deepest_sword_path.txt"))
    dsDir = sr.ReadToEnd();

dsPlugins = Path.Combine(dsDir, "BepInEx", "Plugins");
dsExec = Path.Combine(dsDir, "Deepest Sword");
pluginFileInput = Path.Combine("JumpMod", "bin", "Debug", "netstandard2.1", "JumpMod.dll");
pluginFileOutput = Path.Combine(dsPlugins, "JumpMod.dll");

// Moving files over
if (File.Exists(pluginFileOutput))
    File.Delete(pluginFileOutput);
File.Copy(pluginFileInput, pluginFileOutput);

// Running DS
Process.Start(dsExec);