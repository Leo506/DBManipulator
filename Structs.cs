using System;

namespace DBTest
{
    public struct Player
    {
        public int id;
        public string email;
        public string password;
        public DateTime lastOnline;
        public DateTime registrationDate;

        public Player(int id, string email, string password, DateTime online, DateTime registration)
        {
            this.id = id;
            this.email = email;
            this.password = password;
            this.lastOnline = online;
            this.registrationDate = registration;
        }

        public override string ToString()
        {
            return $"\nInformation about player #{id}\nEmail: {email}\nPassword: {password}\nLast online: {lastOnline}\nRegistration date: {registrationDate}";
        }
    }


    public struct PlayerSettings
    {
        public int id;
        public bool soundOn;
        public bool musicOn;
        public string language;

        public PlayerSettings(int id, bool sound, bool music, string language)
        {
            this.id = id;
            this.soundOn = sound;
            this.musicOn = music;
            this.language = language;
        }

        public override string ToString()
        {
            string soundStr = "Sounds: " + (soundOn ? "On" : "Off");
            string musicStr = "Music: " + (musicOn ? "On" : "Off");
            return $"\nPlayer #{id} has follow settings:\n{soundStr}\n{musicStr}\nLanguage: {language}";
        }
    }
}
