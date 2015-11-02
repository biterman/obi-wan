using System;
using System.Collections.Generic;
using System.Linq;
using QuineMcCluskey.Parser;

namespace QuineMcCluskey
{
    public partial class ReducedBooleanExpression
    {
        // Map each variable to its sort order
        private Dictionary<char, ushort> _alphabet = new Dictionary<char, ushort>();
        // Hold the sorted variables, eg: PQRST or ABCDE
        private char[] _sortedVariables;
        private List<Implicant> _implicants = new List<Implicant>();
        private SortedSet<Implicant> _primeImplicants;
        private Dictionary<uint, List<Implicant>> _primeImplicantTable = new Dictionary<uint, List<Implicant>>();
        private int _mintermCount;
        private HashSet<Implicant> _finalImplicants = new HashSet<Implicant>();
        private string _reducedExpression;

        public ReducedBooleanExpression(string expression)
            : this(new ExpandedBooleanExpression(expression))
        { }

        public ReducedBooleanExpression(ExpandedBooleanExpression bexpr)
        {
            _sortedVariables = bexpr.GetAllVariables();
            ushort index = 0;
            foreach (char c in _sortedVariables)
            {
                _alphabet[c] = index;
                index++;
            }
            Implicant i = null;
            foreach (string factor in bexpr.AsString.Split('+'))
            {
                i = new Implicant(this, factor);
                if ((i == null) || (i.IsContradiction))
                    continue;
                _implicants.Add(i);
            }
            //
            CalculatePrimeImplicants();
            BuildMintermToPrimeImplicantTable();
            _mintermCount = _primeImplicantTable.Count();
            PickFinalImplicants();
        }
        
        public override string ToString()
        {
            if (!String.IsNullOrEmpty(_reducedExpression))
                return _reducedExpression;
            _reducedExpression = string.Join("+", _finalImplicants);
            return _reducedExpression;
        }

        private void CalculatePrimeImplicants()
        {
            List<Implicant> startingSet;
            List<Implicant> iterationPrimes = new List<Implicant>();
            List<Implicant> nextSet = _implicants;
            do
            {
                startingSet = nextSet;
                startingSet.AddRange(iterationPrimes);
                startingSet.Sort();
                nextSet = new List<Implicant>();
                iterationPrimes = new List<Implicant>();
                for (int currIndex = 0; currIndex < startingSet.Count; currIndex++)
                {
                    Implicant current = startingSet[currIndex];
                    bool combined = false;
                    for (int nextIndex = currIndex + 1; nextIndex < startingSet.Count; nextIndex++)
                    {
                        Implicant next = startingSet[nextIndex];
                        if (Math.Abs(current.Ones - next.Ones) > 1)
                            break;
                        Implicant mergedImplicant = current.Merge(next);
                        if (mergedImplicant != null)
                        {
                            combined = true;
                            nextSet.Add(mergedImplicant);
                        }
                    }
                    //
                    if (!combined)
                        iterationPrimes.Add(current);
                }
            } while (nextSet.Count > 0);
            //
            startingSet.AddRange(iterationPrimes);
            _primeImplicants = new SortedSet<Implicant>(startingSet);
        }

        private void BuildMintermToPrimeImplicantTable()
        {
            foreach (Implicant i in _primeImplicants)
            {
                foreach (uint minterm in i.minterms)
                {
                    if (!_primeImplicantTable.ContainsKey(minterm))
                    {
                        _primeImplicantTable[minterm] = new List<Implicant>();
                    }
                    _primeImplicantTable[minterm].Add(i);
                }
            }
        }

        private void PickFinalImplicants()
        {
            HashSet<uint> mintermsCovered = new HashSet<uint>();
            foreach (var kv in _primeImplicantTable)
            {
                if (kv.Value.Count == 1)
                {
                    Implicant i = kv.Value[0];
                    mintermsCovered.UnionWith(i.minterms);
                    _finalImplicants.Add(i);
                }
            }
            //
            if (mintermsCovered.Count < _mintermCount)
            {
                // if not all minterms are covered, apply the following heuristic:
                // from the list of implicants belonging to uncovered minterms
                // greedily add the implicants that cover the largest number of minterms
                // until all minterms are covered.
                //
                // the minimum expression is not guaranteed, but fuck it
                // I'm not implementing Petrick's method.
                foreach (var kv in _primeImplicantTable)
                {
                    if (mintermsCovered.Contains(kv.Key))
                        continue;
                    int maxTersmCovered = 0;
                    Implicant toAdd = null;
                    foreach (Implicant i in kv.Value)
                    {
                        if (i.minterms.Count > maxTersmCovered)
                        {
                            toAdd = i;
                            maxTersmCovered = i.minterms.Count;
                        }
                    }
                    _finalImplicants.Add(toAdd);
                    mintermsCovered.UnionWith(toAdd.minterms);
                    if (mintermsCovered.Count == _mintermCount)
                        break;
                }
            }
        }

        //
        public static string solveQuineMcCluskey(string expression)
        {
            return new ReducedBooleanExpression(expression).ToString();
        }
    }
}
