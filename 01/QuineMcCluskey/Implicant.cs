using QuineMcCluskey.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuineMcCluskey
{
    public partial class ReducedBooleanExpression
    {
        /// <summary>
        /// This class represents a product of literals that implies a set of minterms
        /// in a canonical sum of products eg: ABCD implies 1111 and ABC implies 1110 as well
        /// as 1111.
        /// 
        /// An implicant belongs to a specific boolean expression referred to by its <c>parent</c>
        /// field.
        /// </summary>
        private class Implicant : IComparable<Implicant>, IEquatable<Implicant>
        {
            private const char DASH = '-';
            private static char[] DASHES = { '-', '-', '-', '-', '-', '-', '-', '-', '-',
                                        '-', '-', '-', '-', '-', '-', '-', '-', '-', '-',
                                        '-', '-', '-', '-', '-', '-', '-' };
            //
            private ReducedBooleanExpression parent;
            private char[] _BinArray;
            public bool IsContradiction { get; private set; }
            //
            private int _onesCount = 0;
            private int _dashesCount = 0;
            private string _asString = string.Empty;
            public int Ones
            {
                get { return _onesCount; }
                private set { Ones = _onesCount; }
            }
            public int Dashes
            {
                get { return _dashesCount; }
                private set { Dashes = _dashesCount; }
            }
            public HashSet<uint> minterms = new HashSet<uint>();
            // -------------
            // Constructors
            private Implicant(ReducedBooleanExpression parent, char[] binArray)
            {
                this.parent = parent;
                _BinArray = new char[binArray.Length];
                Array.Copy(binArray, _BinArray, binArray.Length);
                foreach (char c in _BinArray)
                {
                    switch (c)
                    {
                        case '1':
                            _onesCount++;
                            break;
                        case '-':
                            _dashesCount++;
                            break;
                    }
                }
                //
                CalculateImpliedMinterms();
            }
            //
            public Implicant(ReducedBooleanExpression parent, string input)
            {
                // No input checking!
                this.parent = parent;
                int variableCount = parent._sortedVariables.Length;
                _BinArray = new char[variableCount];
                Array.Copy(DASHES, _BinArray, variableCount);
                _dashesCount = variableCount;
                string[] factors = input.Split('*');
                foreach (string f in factors)
                {
                    char literal;
                    char value;
                    //
                    if (f.Length == 0)
                    {
                        throw new Exception(string.Format("Malformed implicant ({0})", input));
                    }
                    else if (f.Length == 1)
                    {
                        literal = f[0];
                        value = '1';
                        if (!Literal.IsValid(literal))
                        {
                            throw new Exception(string.Format("Malformed implicant ({0})", input));
                        }
                    }
                    else if (f.Length == 2)
                    {
                        literal = f[0];
                        value = '0';
                        if (!Literal.IsValid(literal) || (f[1] != '\''))
                        {
                            throw new Exception(string.Format("Malformed implicant ({0})", input));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Malformed implicant ({0})", input));
                    }
                    // what is this literals sort order
                    int position = parent._alphabet[literal];
                    char previousValue = _BinArray[position];
                    // if we've seen this literals complement then this string does not represent
                    // a valid implicant, it is a contradiction e.g.: A*A'
                    if ((previousValue != DASH) && (previousValue != value))
                    {
                        // reset the internal representation and break
                        Array.Copy(DASHES, _BinArray, variableCount);
                        _onesCount = 0;
                        _dashesCount = variableCount;
                        IsContradiction = true;
                        break;
                    }
                    else
                    {
                        _BinArray[position] = value;
                        _dashesCount--;
                        if (value == '1')
                            _onesCount++;
                    }
                }
                //
                CalculateImpliedMinterms();
            }
            // Construction Helpers
            private void CalculateImpliedMinterms()
            {
                if (IsContradiction)
                    return;
                // Starting from this implicants internal binary char[] representation generate
                // all implied minterms by queueing two new char array copies for each DASH found
                // one with the DASH changed to 1 and the other with it set to 0.
                // Any char[] that doesn't add new candidates to the queue
                // is a valid minterm and its integer value must be added to the set
                // of minterms implied. 
                // When the queue is empty all possible minterms have been generated.
                Queue<char[]> mintermCandidates = new Queue<char[]>();
                mintermCandidates.Enqueue(_BinArray);
                while (mintermCandidates.Count > 0)
                {
                    char[] current = mintermCandidates.Dequeue();
                    bool queued = false;
                    uint minterm = 0;
                    for (uint index = 0; index < current.Length; index++)
                    {
                        if (current[index] == DASH)
                        {
                            queued = true;
                            char[] ones = new char[current.Length];
                            char[] zeros = new char[current.Length];
                            Array.Copy(current, ones, current.Length);
                            Array.Copy(current, zeros, current.Length);
                            ones[index] = '1';
                            zeros[index] = '0';
                            mintermCandidates.Enqueue(ones);
                            mintermCandidates.Enqueue(zeros);
                            break;
                        }
                        else
                        {
                            switch (current[index])
                            {
                                case '1':
                                    minterm = (minterm << 1) | 1;
                                    break;
                                default:
                                    minterm <<= 1;
                                    break;
                            }
                        }
                    }
                    //
                    if (!queued)
                    {
                        minterms.Add(minterm);
                    }
                }
            }
            // -------------
            // Overriden methods from Object, IComparable and IEquatable
            public override string ToString()
            {
                if (_dashesCount == _BinArray.Length)
                    return String.Empty;
                if (!string.IsNullOrEmpty(_asString))
                    return _asString;
                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < _BinArray.Length; i++)
                {
                    if (_BinArray[i] != DASH)
                    {
                        strBuilder.Append(parent._sortedVariables[i]);
                        if (_BinArray[i] == '0')
                            strBuilder.Append('\'');
                    }
                }
                _asString =  strBuilder.ToString();
                return _asString;
            }
            // Implicants will be sorted by # of ones and # of dashes.
            // This ordering will allow the correct iteration
            // over the sets of implicants used in the QM reduction process
            public int CompareTo(Implicant other)
            {
                if (other == null) return 1;
                //
                int result = _dashesCount.CompareTo(other._dashesCount);
                if (result == 0)
                    result = _onesCount.CompareTo(other._onesCount);
                if (result == 0)
                    result = ToString().CompareTo(other.ToString());
                return result;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Implicant);
            }

            public bool Equals(Implicant other)
            {
                if (other == null)
                    return false;
                //
                return ToString().Equals(other.ToString());
            }

            public override int GetHashCode()
            {
                // this implementation is naive and lazy,
                // but should only be a problem if Implicants are put in the same
                // collection as strings
                return ToString().GetHashCode();
            }
            // -------------
            // Functionality methods
            //
            public Implicant Merge(Implicant other)
            {
                Implicant merged = null;
                if (other == null)
                {
                    merged = new Implicant(parent, _BinArray);
                }
                else if ((_BinArray.Length == other._BinArray.Length)
                    && (other._dashesCount == _dashesCount)
                    && (Math.Abs(_onesCount - other._onesCount) < 2)
                    && !Equals(other))
                {
                    char[] newBinArray = new char[_BinArray.Length];
                    uint changes = 0;
                    for (int i = 0; i < _BinArray.Length; i++)
                    {
                        // Matching values pass on
                        if (_BinArray[i] == other._BinArray[i])
                        {
                            newBinArray[i] = _BinArray[i];
                        }
                        // On mismatch dashes are not allowed
                        else if ((_BinArray[i] == DASH) || (other._BinArray[i] == DASH))
                        {
                            newBinArray = null;
                            break;
                        }
                        else
                        {
                            newBinArray[i] = DASH;
                            changes++;
                        }
                    }
                    //
                    if ((changes == 1) && (newBinArray != null))
                        merged = new Implicant(parent, newBinArray);
                }
                return merged;
            }
        }
    }
}
