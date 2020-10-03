using Zenject;

namespace Core
{
    public class MemoryLoop
    {
        private Memory m_memoryA;
        private Memory m_memoryB;
        private Memory m_memoryC;

        public MemoryLoop(
            [InjectOptional, Inject(Id = "memA")] Memory memoryA, 
            [InjectOptional, Inject(Id = "memB")] Memory memoryB, 
            [InjectOptional, Inject(Id = "memC")] Memory memoryC)
        {
            m_memoryA = memoryA;
            m_memoryB = memoryB;
            m_memoryC = memoryC;
        }
    }
}