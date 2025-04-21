using Game.Utilities;

namespace Game.Stages
{
    public class StageQueue : CircularQueue<GameStage>
    {
        public StageQueue(int size) : base(size) { }

        private int _CurrentStage;

        public GameStage Advance()
        {
            // interrupt current stage, start next stage, update index
            return null;
        }

        public void Start()
        {
            // Interrupt current stage, reset index, start first stage
        }

        public void Stop()
        {
            // Interrupt current stage
        }

        public void Initialize()
        {
            //Initialize all stages. Not sure if possible yet
        }

        public void Deinitialize()
        {
            //Initialize all stages. Not sure if possible yet
        }
    }
}