namespace GGJ2014
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
    using System;

    /// <summary>
    /// Manages audio for the game.
    /// </summary>
    public class AudioManager
    {
        private float globalVolume;
        private float musicVolume;
        private float SFXVolume;
        private float soundVolume;
        private float voiceVolume;
        private bool fadingVolume = false;
        private bool fadeDown;
        private float minFadeVol;
        private float maxFadeVol;
        private float gFadeTimer;
        private float gFadeTime;
        private Queue<Cue> CueQueue;

        private List<FadeInfo> fadingCues;

        /// <summary>
        /// The audio engine.
        /// </summary>
        private AudioEngine audioEngine;

        /// <summary>
        /// The bank that stores all the cues.
        /// </summary>
        private SoundBank soundBank;

        /// <summary>
        /// The bank that stores all the wave files.
        /// </summary>
        private WaveBank waveBank;

        /// <summary>
        /// The cue for the current music.
        /// </summary>
        private Cue currentMusic;

        /// <summary>
        /// The cue for the next music to be played.
        /// </summary>
        private Cue nextMusic;

        /// <summary>
        /// Prevents a default instance of the AudioManager class from being created. 
        /// Initialises the class and points to the audio engine.
        /// </summary>
        public AudioManager()
        {
            this.audioEngine = new AudioEngine("Content\\Sounds\\testProj.xgs");
            this.waveBank = new WaveBank(this.audioEngine, "Content\\Sounds\\Wave Bank.xwb");
            this.soundBank = new SoundBank(this.audioEngine, "Content\\Sounds\\Sound Bank.xsb");
            this.CueQueue = new Queue<Cue>();

            this.SetGlobalVolume(1f);
            this.SetMusicVolume(0.5f);
            this.SetSFXVolume(0.85f);
            this.SetSoundVolume(0.85f);
            this.SetVoiceVolume(1.0f);
            this.AdjustGlobalVolume(this.globalVolume);

            this.fadingCues = new List<FadeInfo>();
        }

        /// <summary>
        /// Loads a cue from the sound bank.
        /// </summary>
        /// <param name="cueName">The name of the cue.</param>
        /// <returns>Returns the cue from the sound bank.</returns>
        public Cue LoadCue(string cueName)
        {
            Cue cue;
            cue = this.soundBank.GetCue(cueName);
            return cue;
        }

        /// <summary>
        /// Plays a sound cue that has already been loaded.
        /// </summary>
        /// <param name="cue">The sound cue that is to be played.</param>
        public Cue PlayCue(ref Cue cue, bool allowMultipleInstances)
        {
            if (!cue.IsPlaying)
            {
                if (cue.IsStopped)
                {
                    LoadCue(cue.Name);
                }

                cue.Play();
            }
            else if (allowMultipleInstances)
            {
                LoadCue(cue.Name).Play();
            }

            return cue;
        }

        /// <summary>
        /// Plays a music cue that has already been loaded.
        /// </summary>
        /// <param name="cue">The music cue that is to be played.</param>
        public void PlayMusic(Cue cue)
        {
            if (this.currentMusic != null)
            {
                this.currentMusic.Stop(AudioStopOptions.Immediate);
            }

            cue = LoadCue(cue.Name);

            this.nextMusic = cue;
        }

        public void Update(GameTime gameTime)
        {
            if ((this.currentMusic == null || this.currentMusic.IsStopped) && this.nextMusic != null)
            {
                this.currentMusic = LoadCue(this.nextMusic.Name);
                this.currentMusic.Play();
            }

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            #region fade cues

            for (int i = fadingCues.Count - 1; i >= 0; --i)
            {
                bool done = fadingCues[i].Update(gameTime);

                if (done)
                {
                    if (fadingCues[i].StopAfterFade)
                    {
                        fadingCues[i].Cue.Stop(AudioStopOptions.Immediate);
                    }

                    this.fadingCues.RemoveAt(i);
                }
            }

            #endregion

            #region fade all

            if (this.fadingVolume)
            {
                if (this.fadeDown)
                {
                    this.gFadeTimer -= elapsedTime;

                    if (this.gFadeTimer < 0)
                    {
                        this.gFadeTimer = 0;
                        this.fadingVolume = false;
                    }
                }
                else
                {
                    this.gFadeTimer += elapsedTime;

                    if (this.gFadeTimer > this.gFadeTime)
                    {
                        this.gFadeTimer = this.gFadeTime;
                        this.fadingVolume = false;
                    }
                }

                float lerp = this.gFadeTimer / this.gFadeTime;
                float volume = this.minFadeVol + lerp * (this.maxFadeVol - this.minFadeVol);

                this.AdjustGlobalVolume(this.globalVolume * volume);
            }

            #endregion

            if (CueQueue.Count > 0)
            {
                if (!CueQueue.Peek().IsPlaying)
                {
                    CueQueue.Dequeue();

                    if (CueQueue.Count > 0)
                    {
                        Cue cue = CueQueue.Peek();
                        cue = this.PlayCue(ref cue, false);
                    }
                }
            }

            this.audioEngine.Update();
        }

        public Cue EnqueueCue(Cue cue)
        {
            this.CueQueue.Enqueue(cue);

            if (CueQueue.Count == 1)
            {
                cue = this.PlayCue(ref cue, false);
            }

            return cue;
        }

        /// <summary>
        /// Pauses a sound cue that is currently playing.
        /// </summary>
        /// <param name="cue">The sound cue to be paused.</param>
        public void Pause(Cue cue)
        {
            cue.Pause();
        }

        /// <summary>
        /// Resumes a sound cue that is currently paused.
        /// </summary>
        /// <param name="cue">The sound cue that is to be resumed.</param>
        public void Resume(Cue cue)
        {
            cue.Resume();
        }

        public void FadeOut(Cue cue, float fadeTime, bool stopAfterFade)
        {
            this.fadingCues.Add(new FadeInfo(cue, fadeTime, false, stopAfterFade));
        }

        public void FadeOut(Cue cue, float fadeTime, bool stopAfterFade, float minVolume, float maxVolume)
        {
            FadeInfo f = new FadeInfo(cue, fadeTime, false, stopAfterFade);
            f.MinVolume = minVolume;
            f.MaxVolume = maxVolume;
            this.fadingCues.Add(f);
        }

        public Cue FadeIn(Cue cue, float fadeTime)
        {
            if (!cue.IsPlaying)
            {
                this.PlayCue(ref cue, false);
            }

            this.fadingCues.Add(new FadeInfo(cue, fadeTime, true, false));

            return cue;
        }
        
        public Cue FadeIn(Cue cue, float fadeTime, float minVolume, float maxVolume)
        {
            if (!cue.IsPlaying)
            {
                this.PlayCue(ref cue, false);
            }

            FadeInfo f = new FadeInfo(cue, fadeTime, true, false);
            f.MinVolume = minVolume;
            f.MaxVolume = maxVolume;
            this.fadingCues.Add(f);

            return cue;
        }

        public Cue CrossFade(Cue fadeOut, Cue fadeIn, float fadeTime, bool stopFirstCueAfterFade)
        {
            this.FadeOut(fadeOut, fadeTime, stopFirstCueAfterFade);
            return this.FadeIn(fadeIn, fadeTime);
        }

        public void FadeVolumeDown(float fadeTime, float minVolume, float maxVolume)
        {
            this.fadingVolume = true;
            this.fadeDown = true;
            this.minFadeVol = minVolume;
            this.maxFadeVol = maxVolume;
            this.gFadeTimer = fadeTime;
            this.gFadeTime = fadeTime;
        }

        public void FadeVolumeUp(float fadeTime, float minVolume, float maxVolume)
        {
            this.fadingVolume = true;
            this.fadeDown = false;
            this.minFadeVol = minVolume;
            this.maxFadeVol = maxVolume;
            this.gFadeTime = fadeTime;
            this.gFadeTimer = 0;
        }

        public void SetGlobalVolume(float volume)
        {
            this.globalVolume = volume;
        }

        public void AdjustGlobalVolume(float volume)
        {
            this.AdjustMusicVolume(this.musicVolume * volume);
            this.AdjustSFXVolume(this.SFXVolume * volume);
            this.AdjustSoundVolume(this.soundVolume * volume);
            this.AdjustVoiceVolume(this.voiceVolume * volume);
        }

        public void SetMusicVolume(float volume)
        {
            this.musicVolume = volume;          
        }
        
        public void SetSFXVolume(float volume)
        {
            this.SFXVolume = volume;
        }

        public void SetSoundVolume(float volume)
        {
            this.soundVolume = volume;
        }

        public void SetVoiceVolume(float volume)
        {
            this.voiceVolume = volume;
        }

        public void AdjustMusicVolume(float volume)
        {
            this.audioEngine.GetCategory("Music").SetVolume(volume * this.globalVolume);  
        }

        public void AdjustSFXVolume(float volume)
        {
            this.audioEngine.GetCategory("SFX").SetVolume(volume * this.globalVolume);
        }

        public void AdjustSoundVolume(float volume)
        {
            this.audioEngine.GetCategory("Sound").SetVolume(volume * this.globalVolume);
        }

        public void AdjustVoiceVolume(float volume)
        {
            this.audioEngine.GetCategory("Voice").SetVolume(volume * this.voiceVolume);
        }

        public static float TranslateVolume(float volume)
        {
            return -96 + volume * (6 - (-96));
        }
    }

    public class FadeInfo
    {
        public Cue Cue;
        public bool fadeIn;
        public float FadeTime;
        public float FadeTimer;
        public float MinVolume;
        public float MaxVolume;
        public bool StopAfterFade;

        public FadeInfo(Cue cue, float fadeTime, bool fadeIn, bool stopAfterFade)
        {
            this.Cue = cue;
            this.FadeTime = fadeTime;
            this.fadeIn = fadeIn;
            this.FadeTimer = fadeIn ? 0 : fadeTime;
            this.MinVolume = 0;
            this.MaxVolume = 1;
            this.StopAfterFade = stopAfterFade;
        }

        public bool Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool done = false;

            if (this.fadeIn)
            {
                this.FadeTimer += elapsedTime;

                if (this.FadeTimer > this.FadeTime)
                {
                    this.FadeTimer = this.FadeTime;
                    done = true;
                }
            }
            else
            {
                this.FadeTimer -= elapsedTime;

                if (this.FadeTimer < 0)
                {
                    this.FadeTimer = 0;
                    done = true;
                }
            }

            float lerp = this.FadeTimer / this.FadeTime;
            float vol = this.MinVolume + lerp * (this.MaxVolume - this.MinVolume);
            vol = AudioManager.TranslateVolume(vol);
            this.Cue.SetVariable("Volume", vol);

            return done;
        }
    }
}
