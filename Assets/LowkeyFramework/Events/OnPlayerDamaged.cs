using Aether;

namespace AetherEvents
{
    public class OnPlayerDamaged : Event<OnPlayerDamaged>
    {
        public readonly float damageValue;
        public readonly IPlayer player;

        public OnPlayerDamaged(float damageValue, IPlayer player)
        {
            this.damageValue = damageValue;
            this.player = player;
        }
    }
}