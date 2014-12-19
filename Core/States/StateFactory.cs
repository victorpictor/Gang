using Core.States.TheCandidate;
using Core.States.TheFollower;
using Core.States.TheLead;

namespace Core.States
{
    public class StateFactory
    {
        public FinitState Leader()
        {
            return new Leader();
        }
        public FinitState Candidate()
        {
            return new Candidate();
        }
        public FinitState Follower()
        {
            return new Follower();
        } 
    }
}