using Game.Weapons;

namespace Game.Resources
{
    public class ProjectileResources
    {

        [System.Serializable]
        public class ProjectileModel
        {
            public string Name;
            public float Speed;
            public Blast Blast;

            public ProjectileModel() { }
            public ProjectileModel(string name, float speed, Blast blast)
            {
                Name = name;
                Speed = speed;
                Blast = blast;
            }
        }

        public ProjectileModel[] Projectiles;

    }

}