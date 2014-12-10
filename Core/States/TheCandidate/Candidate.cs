using Core.Clustering;
using Core.Messages;

namespace Core.States.TheCandidate
{
    public class Candidate : FinitState
    {
        private ElectionState Granted = new ElectionState();

        public override void EnterState(Node node)
        {
            base.node = node;

            RegisterService(
                new ElectionService(base.node)
                    .Reference());

            RegisterService(
                new ElectionTimeOutService(base.node, this.Granted)
                    .Reference());

            base.EnterState(node);
            
            StartRegisteredServices();
        }

        public override MessageResponse Receive(VoteGranted voteGranted)
        {
            var settings = node.GetSettings();

            Granted.AddVote(voteGranted.VoterId);

            if (Granted.Votes() >= settings.Majority)
                return new MessageResponse(true, () => node.Next(Leader()));

            return new MessageResponse(false, () => { });
        }

        public override MessageResponse Receive(TimedOut timedOut)
        {
            return new MessageResponse(true, () =>
            {
                var state = node.GetState();
                state.Term++;
                node.Next(Candidate());
            });
        }
    }
}