using Game.Weapons;

namespace Game.Resources
{
    public class ProjectileResources
    {

        [System.Serializable]
        public class ProjectileModel
        {
            public string   Name;
            public float    Speed;
            public Blast    Blast;
            public uint     Cost;

            public ProjectileModel() { }
            public ProjectileModel(string name, float speed, Blast blast, uint cost)
            {
                Name    = name;
                Speed   = speed;
                Blast   = blast;
                Cost    = cost;
            }
        }

        public ProjectileModel[] Projectiles;

    }

}