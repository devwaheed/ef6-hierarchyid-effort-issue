// Decompiled with JetBrains decompiler
// Type: System.Data.Entity.Hierarchy.HierarchyId
// Assembly: EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=6847f3395fc61b47
// MVID: 4CD72062-B20F-47A5-86F5-30CA81D14B02

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Systems.Data.Entity.Hierarchy
{
    /// <summary>Represents hierarchical data.</summary>
    [DataContract]
    [Serializable]
    public class HierarchyId : IComparable
    {
        /// <summary>The Path separator character</summary>
        public const string PathSeparator = "/";
        private const string InvalidHierarchyIdExceptionMessage = "The input string '{0}' is not a valid string representation of a HierarchyId node.";
        private const string GetReparentedValueOldRootExceptionMessage = "HierarchyId.GetReparentedValue failed because 'oldRoot' was not an ancestor node of 'this'.  'oldRoot' was '{0}', and 'this' was '{1}'.";
        private const string GetDescendantMostBeChildExceptionMessage = "HierarchyId.GetDescendant failed because '{0}' must be a child of 'this'.  '{0}' was '{1}' and 'this' was '{2}'.";
        private const string GetDescendantChild1MustLessThanChild2ExceptionMessage = "HierarchyId.GetDescendant failed because 'child1' must be less than 'child2'.  'child1' was '{0}' and 'child2' was '{1}'.";
        private readonly string _hierarchyId;
        private readonly int[][] _nodes;

        /// <summary>Constructs an HierarchyId.</summary>
        public HierarchyId()
        {
        }

        /// <summary>
        ///     Constructs an HierarchyId with the given canonical string representation value.
        /// </summary>
        /// <returns>Hierarchyid value.</returns>
        /// <param name="hierarchyId">Canonical string representation</param>
        public HierarchyId(string hierarchyId)
        {
            this._hierarchyId = hierarchyId;
            if (hierarchyId == null)
                return;
            string[] strArray1 = hierarchyId.Split('/');
            if (!string.IsNullOrEmpty(strArray1[0]) || !string.IsNullOrEmpty(strArray1[strArray1.Length - 1]))
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "The input string '{0}' is not a valid string representation of a HierarchyId node.", (object)hierarchyId), nameof(hierarchyId));
            int length = strArray1.Length - 2;
            int[][] numArray1 = new int[length][];
            for (int index1 = 0; index1 < length; ++index1)
            {
                string[] strArray2 = strArray1[index1 + 1].Split('.');
                int[] numArray2 = new int[strArray2.Length];
                for (int index2 = 0; index2 < strArray2.Length; ++index2)
                {
                    int result;
                    if (!int.TryParse(strArray2[index2], out result))
                        throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "The input string '{0}' is not a valid string representation of a HierarchyId node.", (object)hierarchyId), nameof(hierarchyId));
                    numArray2[index2] = result;
                }
                numArray1[index1] = numArray2;
            }
            this._nodes = numArray1;
        }

        /// <summary>
        ///     Returns a hierarchyid representing the nth ancestor of this.
        /// </summary>
        /// <returns>A hierarchyid representing the nth ancestor of this.</returns>
        /// <param name="n">n</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "n")]
        public HierarchyId GetAncestor(int n)
        {
            if (this._nodes == null || (int)this.GetLevel() < n)
                return new HierarchyId((string)null);
            return new HierarchyId("/" + string.Join("/", ((IEnumerable<int[]>)this._nodes).Take<int[]>((int)this.GetLevel() - n).Select<int[], string>(new Func<int[], string>(HierarchyId.IntArrayToStirng))) + "/");
        }

        /// <summary>Returns a child node of the parent.</summary>
        /// <param name="child1"> null or the hierarchyid of a child of the current node. </param>
        /// <param name="child2"> null or the hierarchyid of a child of the current node. </param>
        /// <returns>
        /// Returns one child node that is a descendant of the parent.
        /// If parent is null, returns null.
        /// If parent is not null, and both child1 and child2 are null, returns a child of parent.
        /// If parent and child1 are not null, and child2 is null, returns a child of parent greater than child1.
        /// If parent and child2 are not null and child1 is null, returns a child of parent less than child2.
        /// If parent, child1, and child2 are not null, returns a child of parent greater than child1 and less than child2.
        /// If child1 is not null and not a child of parent, an exception is raised.
        /// If child2 is not null and not a child of parent, an exception is raised.
        /// If child1 &gt;= child2, an exception is raised.
        /// </returns>
        public HierarchyId GetDescendant(HierarchyId child1, HierarchyId child2)
        {
            if (this._nodes == null)
                return new HierarchyId((string)null);
            if (child1 != (HierarchyId)null && ((int)child1.GetLevel() != (int)this.GetLevel() + 1 || !child1.IsDescendantOf(this)))
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "HierarchyId.GetDescendant failed because '{0}' must be a child of 'this'.  '{0}' was '{1}' and 'this' was '{2}'.", (object)nameof(child1), (object)child1, (object)this.ToString()), nameof(child1));
            if (child2 != (HierarchyId)null && ((int)child2.GetLevel() != (int)this.GetLevel() + 1 || !child2.IsDescendantOf(this)))
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "HierarchyId.GetDescendant failed because '{0}' must be a child of 'this'.  '{0}' was '{1}' and 'this' was '{2}'.", (object)nameof(child2), (object)child1, (object)this.ToString()), nameof(child2));
            if (child1 == (HierarchyId)null && child2 == (HierarchyId)null)
                return new HierarchyId(this.ToString() + (object)1 + "/");
            if (child1 == (HierarchyId)null)
            {
                HierarchyId hierarchyId = new HierarchyId(child2.ToString());
                int[] numArray = ((IEnumerable<int[]>)hierarchyId._nodes).Last<int[]>();
                --numArray[numArray.Length - 1];
                return new HierarchyId("/" + string.Join("/", ((IEnumerable<int[]>)hierarchyId._nodes).Select<int[], string>(new Func<int[], string>(HierarchyId.IntArrayToStirng))) + "/");
            }
            if (child2 == (HierarchyId)null)
            {
                HierarchyId hierarchyId = new HierarchyId(child1.ToString());
                int[] numArray = ((IEnumerable<int[]>)hierarchyId._nodes).Last<int[]>();
                ++numArray[numArray.Length - 1];
                return new HierarchyId("/" + string.Join("/", ((IEnumerable<int[]>)hierarchyId._nodes).Select<int[], string>(new Func<int[], string>(HierarchyId.IntArrayToStirng))) + "/");
            }
            int[] array1 = ((IEnumerable<int[]>)child1._nodes).Last<int[]>();
            int[] array2 = ((IEnumerable<int[]>)child2._nodes).Last<int[]>();
            if (HierarchyId.CompareIntArrays(array1, array2) >= 0)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "HierarchyId.GetDescendant failed because 'child1' must be less than 'child2'.  'child1' was '{0}' and 'child2' was '{1}'.", (object)child1, (object)child2), nameof(child1));
            int index = 0;
            while (index < array1.Length && array1[index] >= array2[index])
                ++index;
            int[] array = ((IEnumerable<int>)array1).Take<int>(index + 1).ToArray<int>();
            if (array[index] + 1 < array2[index])
                ++array[index];
            else
                array = ((IEnumerable<int>)array).Concat<int>((IEnumerable<int>)new int[1]
                {
          1
                }).ToArray<int>();
            return new HierarchyId("/" + string.Join("/", ((IEnumerable<int[]>)this._nodes).Select<int[], string>(new Func<int[], string>(HierarchyId.IntArrayToStirng))) + "/" + HierarchyId.IntArrayToStirng((IEnumerable<int>)array) + "/");
        }

        /// <summary>
        ///     Returns an integer that represents the depth of the node this in the tree.
        /// </summary>
        /// <returns>An integer that represents the depth of the node this in the tree.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public short GetLevel()
        {
            if (this._nodes == null)
                return 0;
            return (short)this._nodes.Length;
        }

        /// <summary>Returns the root of the hierarchy tree.</summary>
        /// <returns>The root of the hierarchy tree.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static HierarchyId GetRoot()
        {
            return DbHierarchyServices.GetRoot();
        }

        /// <summary>Returns true if this is a descendant of parent.</summary>
        /// <returns>True if this is a descendant of parent.</returns>
        /// <param name="parent">parent</param>
        public bool IsDescendantOf(HierarchyId parent)
        {
            if (parent == (HierarchyId)null)
                return true;
            if (this._nodes == null || (int)parent.GetLevel() > (int)this.GetLevel())
                return false;
            for (int index = 0; index < (int)parent.GetLevel(); ++index)
            {
                if (HierarchyId.CompareIntArrays(this._nodes[index], parent._nodes[index]) != 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///     Returns a node whose path from the root is the path to newRoot, followed by the path from oldRoot to this.
        /// </summary>
        /// <returns>Hierarchyid value.</returns>
        /// <param name="oldRoot">oldRoot</param>
        /// <param name="newRoot">newRoot</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Reparented")]
        public HierarchyId GetReparentedValue(HierarchyId oldRoot, HierarchyId newRoot)
        {
            if (oldRoot == (HierarchyId)null || newRoot == (HierarchyId)null)
                return new HierarchyId((string)null);
            if (!this.IsDescendantOf(oldRoot))
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "HierarchyId.GetReparentedValue failed because 'oldRoot' was not an ancestor node of 'this'.  'oldRoot' was '{0}', and 'this' was '{1}'.", (object)oldRoot, (object)this.ToString()), nameof(oldRoot));
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("/");
            foreach (int[] node in newRoot._nodes)
            {
                stringBuilder.Append(HierarchyId.IntArrayToStirng((IEnumerable<int>)node));
                stringBuilder.Append("/");
            }
            foreach (int[] numArray in ((IEnumerable<int[]>)this._nodes).Skip<int[]>((int)oldRoot.GetLevel()))
            {
                stringBuilder.Append(HierarchyId.IntArrayToStirng((IEnumerable<int>)numArray));
                stringBuilder.Append("/");
            }
            return new HierarchyId(stringBuilder.ToString());
        }

        /// <summary>
        ///     Converts the canonical string representation of a hierarchyid to a hierarchyid value.
        /// </summary>
        /// <returns>Hierarchyid value.</returns>
        /// <param name="input">input</param>
        public static HierarchyId Parse(string input)
        {
            return new HierarchyId(input);
        }

        private static string IntArrayToStirng(IEnumerable<int> array)
        {
            return string.Join<int>(".", array);
        }

        private static int CompareIntArrays(int[] array1, int[] array2)
        {
            int num1 = Math.Min(array1.Length, array2.Length);
            for (int index = 0; index < num1; ++index)
            {
                int num2 = array1[index];
                int num3 = array2[index];
                if (num2 < num3)
                    return -1;
                if (num2 > num3)
                    return 1;
            }
            if (array1.Length > num1)
                return 1;
            return array2.Length > num1 ? -1 : 0;
        }

        /// <summary>Compares two HierarchyIds by their values.</summary>
        /// <param name="hid1"> a HierarchyId to compare </param>
        /// <param name="hid2"> a HierarchyId to compare </param>
        /// <returns>
        ///     A 32-bit signed integer that indicates the lexical relationship between the two comparands.
        ///     Value Condition Less than zero: hid1 is less than hid2.
        ///     Zero: hid1 equals hid2.
        ///     Greater than zero: hid1 is greater than hid2.
        /// </returns>
        public static int Compare(HierarchyId hid1, HierarchyId hid2)
        {
            int[][] numArray1 = (object)hid1 == null ? (int[][])null : hid1._nodes;
            int[][] numArray2 = (object)hid2 == null ? (int[][])null : hid2._nodes;
            if (numArray1 == null && numArray2 == null)
                return 0;
            if (numArray1 == null)
                return -1;
            if (numArray2 == null)
                return 1;
            int num1 = Math.Min(numArray1.Length, numArray2.Length);
            for (int index = 0; index < num1; ++index)
            {
                int num2 = HierarchyId.CompareIntArrays(numArray1[index], numArray2[index]);
                if (num2 != 0)
                    return num2;
            }
            if (hid1._nodes.Length > num1)
                return 1;
            return hid2._nodes.Length > num1 ? -1 : 0;
        }

        /// <summary>Compares two HierarchyIds by their values.</summary>
        /// <param name="hid1"> a HierarchyId to compare </param>
        /// <param name="hid2"> a HierarchyId to compare </param>
        /// <returns>
        ///     true if the the first parameter is less than the second parameter, false otherwise
        /// </returns>
        public static bool operator <(HierarchyId hid1, HierarchyId hid2)
        {
            return HierarchyId.Compare(hid1, hid2) < 0;
        }

        /// <summary>Compares two HierarchyIds by their values.</summary>
        /// <param name="hid1"> a HierarchyId to compare </param>
        /// <param name="hid2"> a HierarchyId to compare </param>
        /// <returns>
        ///     true if the the first parameter is greater than the second parameter, false otherwise
        /// </returns>
        public static bool operator >(HierarchyId hid1, HierarchyId hid2)
        {
            return hid2 < hid1;
        }

        /// <summary>Compares two HierarchyIds by their values.</summary>
        /// <param name="hid1"> a HierarchyId to compare </param>
        /// <param name="hid2"> a HierarchyId to compare </param>
        /// <returns>
        ///     true if the the first parameter is less or equal than the second parameter, false otherwise
        /// </returns>
        public static bool operator <=(HierarchyId hid1, HierarchyId hid2)
        {
            return HierarchyId.Compare(hid1, hid2) <= 0;
        }

        /// <summary>Compares two HierarchyIds by their values.</summary>
        /// <param name="hid1"> a HierarchyId to compare </param>
        /// <param name="hid2"> a HierarchyId to compare </param>
        /// <returns>
        ///     true if the the first parameter is greater or equal than the second parameter, false otherwise
        /// </returns>
        public static bool operator >=(HierarchyId hid1, HierarchyId hid2)
        {
            return hid2 <= hid1;
        }

        /// <summary>Compares two HierarchyIds by their values.</summary>
        /// <param name="hid1"> a HierarchyId to compare </param>
        /// <param name="hid2"> a HierarchyId to compare </param>
        /// <returns> true if the two HierarchyIds are equal, false otherwise </returns>
        public static bool operator ==(HierarchyId hid1, HierarchyId hid2)
        {
            return HierarchyId.Compare(hid1, hid2) == 0;
        }

        /// <summary>Compares two HierarchyIds by their values.</summary>
        /// <param name="hid1"> a HierarchyId to compare </param>
        /// <param name="hid2"> a HierarchyId to compare </param>
        /// <returns> true if the two HierarchyIds are not equal, false otherwise </returns>
        public static bool operator !=(HierarchyId hid1, HierarchyId hid2)
        {
            return !(hid1 == hid2);
        }

        /// <summary>
        ///     Compares this instance to a given HierarchyId by their values.
        /// </summary>
        /// <param name="other"> the HierarchyId to compare against this instance </param>
        /// <returns> true if this instance is equal to the given HierarchyId, and false otherwise </returns>
        protected bool Equals(HierarchyId other)
        {
            return this == other;
        }

        /// <summary>
        ///     Returns a value-based hash code, to allow HierarchyId to be used in hash tables.
        /// </summary>
        /// <returns> the hash value of this HierarchyId </returns>
        public override int GetHashCode()
        {
            if (this._hierarchyId == null)
                return 0;
            return this._hierarchyId.GetHashCode();
        }

        /// <summary>
        ///     Compares this instance to a given HierarchyId by their values.
        /// </summary>
        /// <param name="obj"> the HierarchyId to compare against this instance </param>
        /// <returns> true if this instance is equal to the given HierarchyId, and false otherwise </returns>
        public override bool Equals(object obj)
        {
            return this.Equals((HierarchyId)obj);
        }

        /// <summary>
        ///     Returns a string representation of the hierarchyid value.
        /// </summary>
        /// <returns>A string representation of the hierarchyid value.</returns>
        public override string ToString()
        {
            return this._hierarchyId;
        }

        /// <summary>Implementation of IComparable.CompareTo()</summary>
        /// <param name="obj"> The object to compare to </param>
        /// <returns> 0 if the HierarchyIds are "equal" (i.e., have the same _hierarchyId value) </returns>
        public int CompareTo(object obj)
        {
            if (obj as HierarchyId != (HierarchyId)null)
                return HierarchyId.Compare(this, (HierarchyId)obj);
            return -1;
        }
    }
}
