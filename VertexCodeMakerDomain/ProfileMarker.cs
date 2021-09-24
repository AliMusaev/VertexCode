using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VertexCodeMakerDomain
{
    public class ProfileMarker
    {
        private int _counter;
        private readonly char[] alphabet = new char[]{'A','B','C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
        public ProfileMarker()
        {
            Reset();
        }
        public char GetNextMark()
        {
            if (_counter < alphabet.Length)
            {
                return alphabet[_counter++];
            }
            else
            {
                throw new Exception("Alphabet is gone, to mush sections in one frame =(");
            }
        }
        public void Reset()
        {
            _counter = 0;
        }

    }
}
