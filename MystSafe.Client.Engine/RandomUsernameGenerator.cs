// MystSafe is a secret vault with anonymous access and zero activity tracking protected by cryptocurrency-grade tech.
// 
//     Copyright (C) 2024-2025 MystSafe, NeoSoft99
// 
//     MystSafe: The Only Privacy-Preserving Password Manager.
//     https://mystsafe.com
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//     See the GNU Affero General Public License for more details.
// 
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

namespace MystSafe.Client.Engine;

public static class RandomUsernameGenerator
{


    static string[] adjectives =
    {
        "Sly", "Sneak", "Sharp", "Quick", "Covert", "Clever", "Slick", "Keen", "Shrewd", "Nimble", "Bold", "Brave",
        "Calm", "Swift", "Tough", "Wary", "Zippy", "Cool", "Deft", "Fine",
        "Neat", "Prim", "Snug", "Soft", "Sure", "Tidy", "Trim", "Vast", "Wily", "Bare", "Cute", "Deep", "Firm", "Glad",
        "Kind", "Lean", "Mild", "Pure", "Rash", "Real",
        "Rude", "Shy", "Slim", "Tall", "Thin", "Warm", "Wild", "Wise", "Young", "Zest", "Brisk", "Chic", "Cool",
        "Crisp", "Dandy", "Dapper", "Daring", "Dash", "Cyber", "Quantum",
        "Astro", "Neo", "Virtual", "Digital", "Shadow", "Phantom", "Sonic", "Turbo", "Hyper", "Stealth", "Cosmic",
        "Binary", "Holo", "Void", "Neb", "Stellar", "Orbit", "Photon", "Nova"
    };

    static string[] nouns =
    {
        "Guard", "Agent", "Scout", "Spy", "Watcher", "Eagle", "Hawk", "Lion", "Tiger", "Wolf", "Fox", "Bear", "Owl",
        "Bee", "Ant", "Bat", "Cat", "Dog", "Elk", "Frog",
        "Gull", "Hare", "Jack", "Kite", "Lark", "Mole", "Orca", "Puma", "Quail", "Raven", "Seal", "Toad", "Viper",
        "Whale", "Yak", "Zebra", "Arch", "Baron", "Chief", "Duke",
        "Earl", "Guru", "Hero", "Icon", "Judge", "King", "Lord", "Mage", "Ninja", "Pilot", "Queen", "Rogue", "Sage",
        "Titan", "Viking", "Wizard", "Alpha", "Bravo", "Charlie", "Delta",
        "Echo", "Foxtrot", "Golf", "Hotel", "Porsche", "Juliet", "Kilo", "Lima", "Mike", "Nov", "Oscar", "Papa",
        "Romeo", "Sierra", "Tango", "Uniform", "Victor", "Whiskey", "X-ray", "Yankee", "Zulu",
        "Droid", "Bot", "Voyager", "Ranger", "Drone", "Pixel", "Gamer", "Alien", "Comet", "Meteor", "Galaxy", "Star",
        "Portal", "Rocket", "Satellite", "Cosmo", "Astronaut", "Nebula", "Quasar", "Ship"
    };

    public static string GenerateRandomUsername()
    {
        Random random = new Random();
        string adjective = adjectives[random.Next(adjectives.Length)];
        string noun = nouns[random.Next(nouns.Length)];
        // Adjusting the numeric suffix to allow for up to a 3-digit number, ensuring zeros are omitted from the beginning
        int numberSuffix = random.Next(1, 1000);

        return $"{adjective}{noun}{numberSuffix}";
    }
}





