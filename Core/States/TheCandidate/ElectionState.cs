using System.Collections.Generic;

namespace Core.States.TheCandidate
{
    public class ElectionState
    {
        private HashSet<int> granted = new HashSet<int>();

        public void AddVote(int votter)
        {
            granted.Add(votter);
        }

        public int Votes()
        {
            return granted.Count;
        }
    }
}