<p align="center">
  <img width="500" height="60" src="https://user-images.githubusercontent.com/39125931/113291733-bcf7d980-92f3-11eb-98ba-f47aca15eab8.png">
</p>

Death Music IV is a simple mod where when you die, a random music track will be played. There are 6 default death musics included, these 6 are the original ones from GTA IV. 

## Custom Music
You can add you own music as well! Just drag & drop any of your .mp3 or .wav file into the DeathMusicIV folder and your music is now implemented!

## Episode Specific Music
You can also add the following prefixes to the beginning of your music filename: *gtaiv_***, *tbogt_*** and *tlad_*** if you play for example The Lost And Damned, only the files with *tlad_*** at the beginning will be played. The same applies for GTAIV and TBOGT. If you dont give your file a prefix, then this file is going to be a general file. General files are being played when there are no files with a prefix for the specific episode.

* : Any filename

Little Example:
*Death_1.mp3 - Death_6.mp3* are general files because they have no prefix and can be played in every episode if there are no soundtracks for the specific episode.
*gtaiv_Death_1.mp3* would be a soundtrack specifically for GTA IV.
*tlad_Death_1.mp3* would be a soundtrack specifically for The Lost And Damned.
*tbogt_Death_1.mp3* would be a soundtrack specifically for The Ballad Of Gay Tony.

## Ini File Explanation
**RndSeed** : Seed for randomized number generator. Please only enter numbers. Leave blank for random seed. (Default: *empty*)
**FadeOut** : Fades the music out when you respawn. (Default: true)
**FadeIn** : Fades the music in when you die or are on low health (see: PlayWhenNearDeath). (Default: true)
**FadingSpeed** : How fast the music should fade in milliseconds. Please only enter numbers. (Default: 3000)
**PlayWhenNearDeath** : Music will play if you are NEAR death (low health). (Default: false)
**NearDeathHealth** : At wich health level should the music start? (Default: 15)
**Volume** : The volume of the music. (Default: 20)

## How to Contribute
Do you have an idea to improve this mod, or did you happen to run into a bug? Please share your idea or the bug you found in the **[issues page](https://github.com/ClonkAndre/DeathMusicIV/issues)**, or even better: feel free to fork and contribute to this project with a **[Pull Request](https://github.com/ClonkAndre/DeathMusicIV/pulls)**.

Make sure you have **Visual Studio** installed, and that you add a **reference** to the file "**ScriptHookDotNet.dll**" in **Visual Studio**, otherwise you will run in to a hole lot of **errors**.

If you dont have the **ScriptHookDotNet.dll** file, then here is a link for you to download this file: https://www.dropbox.com/s/9mhvnmy101aspkw/ScriptHookDotNet.dll?dl=1
