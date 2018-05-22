using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Util
{
    public class TokenManager
    {
        private Hashtable tokenHashTable = new Hashtable();

        public TokenManager()
        {
        }

        public TokenManager(string[] keys, string[] values)
        {
            for (int i = 0; i < (keys.GetUpperBound(0) + 1); i++)
                this.tokenHashTable.Add(keys[i], values[i]);

        }

        public void AddToken(string key, string val)
        {
            if (!this.tokenHashTable.Contains(key))
                this.tokenHashTable.Add(key, val);
            else
                this.tokenHashTable[key] = val;
        }

        public string ReplaceTokens(string TextWithTokens)
        {
            IDictionaryEnumerator myEnumerator = this.tokenHashTable.GetEnumerator();
            string _tempText = TextWithTokens;

            while (myEnumerator.MoveNext())
                _tempText = _tempText.Replace(myEnumerator.Key.ToString(), myEnumerator.Value.ToString());

            return (_tempText);
        }
    }
}
