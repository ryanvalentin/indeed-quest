using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CreditsProfile", menuName = "Indeed/Credits Profile", order = 1)]
public class CreditsProfile : ScriptableObject
{
    public string Title;

    public string Description;

    public string VoteLink;

    public Credit[] Credits;

    [Serializable]
    public class Credit
    {
        public string Name;
        public Sprite Avatar;
        public string Role;
        public GameplayProfile Gameplay;
    }
}
