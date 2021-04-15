// DeathMusicIV by ItsClonkAndre
// Version 1.4

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTA;
using Un4seen.Bass;

namespace DeathMusicIV {
    public class Main : Script {

        #region Variables and Enums
        private Random rnd;

        private bool tempBool;
        private bool isHandleCurrentlyFadingOut;
        private bool fadeOut;
        private bool fadeIn;
        private bool playWhenNearDeath;
        //private bool alsoPlayDeathMusicWhenNearDeathIsActive;

        private int fadingSpeed;
        private int initalVolume;
        private int nearDeathHealth;
        private int musicHandle;

        private string[] musicFiles;
        private readonly string DataDir = Game.InstallFolder + @"\scripts\DeathMusicIV";

        private enum AudioPlayMode
        {
            Play,
            Pause,
            Stop,
            None
        }
        #endregion

        #region Methods
        private int CreateFile(string file, bool createWithZeroDecibels, bool dontDestroyOnStreamEnd = false)
        {
            if (!string.IsNullOrWhiteSpace(file)) {
                if (createWithZeroDecibels) {
                    if (dontDestroyOnStreamEnd) {
                        int handle = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_PRESCAN);
                        SetStreamVolume(handle, 0f);
                        return handle;
                    }
                    else {
                        int handle = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE);
                        SetStreamVolume(handle, 0f);
                        return handle;
                    }
                }
                else {
                    if (dontDestroyOnStreamEnd) {
                        return Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_PRESCAN);
                    }
                    else {
                        return Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE);
                    }
                }
            }
            else {
                return 0;
            }
        }
        public bool SetStreamVolume(int stream, float volume)
        {
            if (stream != 0) {
                return Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, volume / 100.0F);
            }
            else {
                return false;
            }
        }
        private AudioPlayMode GetStreamPlayMode(int stream)
        {
            if (stream != 0) {
                switch (Bass.BASS_ChannelIsActive(stream)) {
                    case BASSActive.BASS_ACTIVE_PLAYING:
                        return AudioPlayMode.Play;
                    case BASSActive.BASS_ACTIVE_PAUSED:
                        return AudioPlayMode.Pause;
                    case BASSActive.BASS_ACTIVE_STOPPED:
                        return AudioPlayMode.Stop;
                    default:
                        return AudioPlayMode.None;
                }
            }
            else {
                return AudioPlayMode.None;
            }
        }
        private async void FadeStreamOut(int stream, AudioPlayMode after, int fadingSpeed = 1000)
        {
            if (!isHandleCurrentlyFadingOut) {
                isHandleCurrentlyFadingOut = true;

                float handleVolume = 0f;
                Bass.BASS_ChannelSlideAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, 0f, fadingSpeed);

                while (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING) {
                    Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, ref handleVolume);

                    if (handleVolume <= 0f) {
                        switch (after) {
                            case AudioPlayMode.Stop:
                                Bass.BASS_ChannelStop(stream);
                                isHandleCurrentlyFadingOut = false;
                                musicHandle = 0;
                                break;
                            case AudioPlayMode.Pause:
                                Bass.BASS_ChannelPause(stream);
                                isHandleCurrentlyFadingOut = false;
                                musicHandle = 0;
                                break;
                        }
                        break;
                    }

                    await Task.Delay(5);
                }
            }
        }
        private void FadeStreamIn(int stream, float fadeToVolumeLevel, int fadingSpeed)
        {
            Bass.BASS_ChannelPlay(stream, false);
            Bass.BASS_ChannelSlideAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, fadeToVolumeLevel / 100.0f, fadingSpeed);
        }

        private int GetEpisodicSpecificMusicHandle()
        {
            switch (Game.CurrentEpisode) {
                case GameEpisode.GTAIV:
                    string[] ivSoundtracks = musicFiles.Where(file => Path.GetFileNameWithoutExtension(file).ToLower().StartsWith("gtaiv_")).ToArray();
                    if (ivSoundtracks.Length != 0) {
                        string rFile = ivSoundtracks[rnd.Next(0, ivSoundtracks.Length)];
                        return CreateFile(rFile, fadeIn);
                    }
                    break;
                case GameEpisode.TBOGT:
                    string[] tbogtSoundtracks = musicFiles.Where(file => Path.GetFileNameWithoutExtension(file).ToLower().StartsWith("tbogt_")).ToArray();
                    if (tbogtSoundtracks.Length != 0) {
                        string rFile = tbogtSoundtracks[rnd.Next(0, tbogtSoundtracks.Length)];
                        return CreateFile(rFile, fadeIn);
                    }
                    break;
                case GameEpisode.TLAD:
                    string[] tladSoundtracks = musicFiles.Where(file => Path.GetFileNameWithoutExtension(file).ToLower().StartsWith("tlad_")).ToArray();
                    if (tladSoundtracks.Length != 0) {
                        string rFile = tladSoundtracks[rnd.Next(0, tladSoundtracks.Length)];
                        return CreateFile(rFile, fadeIn);
                    }
                    break;
            }
            return 0;
        }
        private void PlayRandomSoundtrack()
        {
            try {
                if (playWhenNearDeath) {
                    switch (Game.CurrentEpisode) {
                        case GameEpisode.GTAIV:
                            string[] nearDeathSoundtracksGTAIV = musicFiles.Where(file => Path.GetFileNameWithoutExtension(file).ToLower().StartsWith("lh_gtaiv_")).ToArray();
                            if (nearDeathSoundtracksGTAIV.Length != 0) {
                                string rFile = nearDeathSoundtracksGTAIV[rnd.Next(0, nearDeathSoundtracksGTAIV.Length)];
                                musicHandle = CreateFile(rFile, fadeIn);
                            }
                            break;
                        case GameEpisode.TBOGT:
                            string[] nearDeathSoundtracksTBOGT = musicFiles.Where(file => Path.GetFileNameWithoutExtension(file).ToLower().StartsWith("lh_tbogt_")).ToArray();
                            if (nearDeathSoundtracksTBOGT.Length != 0) {
                                string rFile = nearDeathSoundtracksTBOGT[rnd.Next(0, nearDeathSoundtracksTBOGT.Length)];
                                musicHandle = CreateFile(rFile, fadeIn);
                            }
                            break;
                        case GameEpisode.TLAD:
                            string[] nearDeathSoundtracksTLAD = musicFiles.Where(file => Path.GetFileNameWithoutExtension(file).ToLower().StartsWith("lh_tlad_")).ToArray();
                            if (nearDeathSoundtracksTLAD.Length != 0) {
                                string rFile = nearDeathSoundtracksTLAD[rnd.Next(0, nearDeathSoundtracksTLAD.Length)];
                                musicHandle = CreateFile(rFile, fadeIn);
                            }
                            break;
                    }
                }
                else {
                    musicHandle = GetEpisodicSpecificMusicHandle();
                }

                if (musicHandle == 0) {
                    string[] nearDeathSoundtracksAny = musicFiles.Where(file => Path.GetFileNameWithoutExtension(file).ToLower().StartsWith("lh_")).ToArray();
                    if (nearDeathSoundtracksAny.Length != 0) {
                        string rFile = nearDeathSoundtracksAny[rnd.Next(0, nearDeathSoundtracksAny.Length)];
                        musicHandle = CreateFile(rFile, fadeIn);
                    }
                }

                if (musicHandle != 0) {
                    if (fadeIn) {
                        FadeStreamIn(musicHandle, initalVolume, fadingSpeed);
                    }
                    else {
                        Bass.BASS_ChannelPlay(musicHandle, false);
                    }
                }
                else { // Get random file if musicHandle is zero
                    List<string> files = new List<string>();
                    for (int i = 0; i < musicFiles.Length; i++) {
                        string filename = Path.GetFileNameWithoutExtension(musicFiles[i]).ToLower();
                        if (!filename.StartsWith("gtaiv_") && !filename.StartsWith("tbogt_") && !filename.StartsWith("tlad_")) {
                            files.Add(musicFiles[i]);
                        }
                    }

                    if (files.Count != 0) {
                        string rFile = files[rnd.Next(0, files.Count)];
                        musicHandle = CreateFile(rFile, fadeIn);

                        if (fadeIn) {
                            FadeStreamIn(musicHandle, initalVolume, fadingSpeed);
                        }
                        else {
                            Bass.BASS_ChannelPlay(musicHandle, false);
                        }
                    }
                    else {
                        Game.Console.Print("DeathMusicIV: Could not play soundtrack because there are no soundtracks available.");
                    }

                    files.Clear();
                    files = null;
                }
            }
            catch (Exception ex) {
                Game.Console.Print("DeathMusicIV error in Play method. Details: " + ex.ToString());
            }
        }
        private void StopSoundtrack()
        {
            if (musicHandle != 0) {
                if (GetStreamPlayMode(musicHandle) == AudioPlayMode.Play) {
                    if (fadeOut) {
                        FadeStreamOut(musicHandle, AudioPlayMode.Stop, fadingSpeed);
                    }
                    else {
                        Bass.BASS_ChannelStop(musicHandle);
                        musicHandle = 0;
                    }
                }
                else {
                    Bass.BASS_ChannelStop(musicHandle);
                    musicHandle = 0;
                }
            }
        }
        #endregion

        public Main()
        {
            try {
                // Setup Bass.dll
                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

                // Load settings...
                rnd = new Random(Settings.GetValueInteger("RndSeed", "General", DateTime.Now.Millisecond));
                fadeOut = Settings.GetValueBool("FadeOut", "Music", true);
                fadeIn = Settings.GetValueBool("FadeIn", "Music", true);
                fadingSpeed = Settings.GetValueInteger("FadingSpeed", "Music", 3000);
                playWhenNearDeath = Settings.GetValueBool("PlayWhenNearDeath", "Music", false);
                //alsoPlayDeathMusicWhenNearDeathIsActive = Settings.GetValueBool("AlsoPlayDeathMusicWhenNearDeathIsActive", "Music", true);
                nearDeathHealth = Settings.GetValueInteger("NearDeathHealth", "Music", 15);
                initalVolume = Settings.GetValueInteger("Volume", "Music", 20);

                if (!nearDeathHealth.ToString().Contains('-')) {
                    if (nearDeathHealth > 100 || nearDeathHealth <= 0) {
                        nearDeathHealth = 10;
                    }
                }
                else {
                    nearDeathHealth = int.Parse(nearDeathHealth.ToString().Replace('-', ' '));
                    if (nearDeathHealth > 100 || nearDeathHealth <= 0) {
                        nearDeathHealth = 10;
                    }
                }

                // Script stuff
                this.Interval = 100;
                this.Tick += Main_Tick;
            }
            catch (Exception ex) {
                Game.Console.Print("DeathMusicIV error in constructor. Details: " + ex.ToString() + " - This script can't continue.");
                this.Abort();
            }
        }

        private void Main_Tick(object sender, EventArgs e)
        {
            if (Directory.Exists(DataDir)) {
                musicFiles = Directory.EnumerateFiles(DataDir).Where(file => file.ToLower().EndsWith("mp3") || file.ToLower().EndsWith("wav")).ToArray();
                if (musicFiles.Length != 0) {
                    if (playWhenNearDeath) {
                        if (Game.LocalPlayer.Character.Health <= nearDeathHealth) {
                            if (!tempBool) {
                                PlayRandomSoundtrack();
                                tempBool = true;
                            }
                        }
                        else {
                            if (tempBool) {
                                StopSoundtrack();
                                tempBool = false;
                            }

                            if (!isHandleCurrentlyFadingOut) {
                                if (GetStreamPlayMode(musicHandle) == AudioPlayMode.Play) {
                                    if (fadeOut) {
                                        FadeStreamOut(musicHandle, AudioPlayMode.Stop, fadingSpeed);
                                    }
                                    else {
                                        Bass.BASS_ChannelStop(musicHandle);
                                        musicHandle = 0;
                                    }
                                }
                            }
                        }
                    }
                    else {
                        if (Game.LocalPlayer.Character.isDead) { // Play
                            if (!tempBool) {
                                PlayRandomSoundtrack();
                                tempBool = true;
                            }
                        }
                        else { // Stop
                            if (tempBool) {
                                StopSoundtrack();
                                tempBool = false;
                            }

                            if (!isHandleCurrentlyFadingOut) {
                                if (GetStreamPlayMode(musicHandle) == AudioPlayMode.Play) {
                                    if (fadeOut) {
                                        FadeStreamOut(musicHandle, AudioPlayMode.Stop, fadingSpeed);
                                    }
                                    else {
                                        Bass.BASS_ChannelStop(musicHandle);
                                        musicHandle = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
