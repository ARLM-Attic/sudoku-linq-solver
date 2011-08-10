//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;

//namespace SudokuLib
//{
//    public static class IEnumerableExtensions
//    {
//        public static void ForEachWithIndex<T>(this IEnumerable<T> a_enumerable, Action<T, int> a_handler)
//        {
//            int index = 0;
//            foreach (T item in a_enumerable)
//                a_handler(item, index++);
//        }

//        public static void ForEach<T>(this IEnumerable<T> a_enumerable, Action<T> a_handler)
//        {
//            foreach (T item in a_enumerable)
//                a_handler(item);
//        }

//        public static IEnumerable<T> Except<T>(this IEnumerable<T> a_enumerable, T a_element)
//        {
//            foreach (T ele in a_enumerable)
//            {
//                if (!ele.Equals(a_element))
//                    yield return ele;
//            }
//        }

//        public static IEnumerable<T> Except<T>(this IEnumerable<T> a_enumerable, T a_element, IEqualityComparer<T> a_comparer)
//        {
//            foreach (T ele in a_enumerable)
//            {
//                if (!a_comparer.Equals(ele, a_element))
//                    yield return ele;
//            }
//        }

//        public static bool ContainsAny<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values)
//        {
//            return a_enumerable.Intersect(a_values).Any();
//        }

//        public static bool ContainsAny<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values, IEqualityComparer<T> a_comparer)
//        {
//            return a_enumerable.Intersect(a_values, a_comparer).Any();
//        }

//        public static bool Contains<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values)
//        {

//            if (a_values.FirstOrDefault() == null)
//                return false;

//            foreach (T ele in a_values)
//            {
//                if (!a_enumerable.Contains(ele))
//                    return false;
//            }

//            return true;
//        }

//        public static bool Contains<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values, IEqualityComparer<T> a_comparer)
//        {
//            if (a_values.FirstOrDefault() == null)
//                return false;

//            foreach (T ele in a_values)
//            {
//                if (!a_enumerable.Contains(ele, a_comparer))
//                    return false;
//            }

//            return true;
//        }

//        public static bool Exact<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values)
//        {
//            List<T> list = new List<T>(a_values);

//            int init_count = list.Count;
//            int count = 0;

//            foreach (T ele in a_enumerable)
//            {
//                count++;

//                if (count > init_count)
//                    return false;

//                int index = list.IndexOf(ele);
//                if (index == -1)
//                    return false;
//                else
//                    list.RemoveAt(index);
//            }

//            return count == init_count;
//        }

//        public static bool Exact<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values, IEqualityComparer<T> a_comparer)
//        {
//            List<T> list = new List<T>(a_values);

//            int init_count = list.Count;
//            int count = 0;

//            foreach (T ele in a_enumerable)
//            {
//                count++;

//                if (count > init_count)
//                    return false;

//                int index = list.IndexOf(ele, a_comparer);
//                if (index == -1)
//                    return false;
//                else
//                    list.RemoveAt(index);
//            }

//            return count == init_count;
//        }

//        public static IEnumerable<T> Substract<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values)
//        {
//            List<T> list = new List<T>(a_values);

//            foreach (T ele in a_enumerable)
//            {
//                int index = list.IndexOf(ele);
//                if (index != -1)
//                    list.RemoveAt(index);
//                else
//                    yield return ele;
//            }
//        }

//        public static IEnumerable<T> Substract<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values, IEqualityComparer<T> a_comparer)
//        {
//            List<T> list = new List<T>(a_values);

//            foreach (T ele in a_enumerable)
//            {
//                int index = list.IndexOf(ele, a_comparer);
//                if (index != -1)
//                    list.RemoveAt(index);
//                else
//                    yield return ele;
//            }
//        }

//        public static bool ContainsExact<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values)
//        {
//            List<T> list = new List<T>(a_enumerable);

//            if (a_values.FirstOrDefault() == null)
//                return false;

//            foreach (T ele in a_values)
//            {
//                int index = list.IndexOf(ele);
//                if (index == -1)
//                    return false;
//                else
//                    list.RemoveAt(index);
//            }

//            return true;
//        }

//        public static bool ContainsExact<T>(this IEnumerable<T> a_enumerable, IEnumerable<T> a_values, IEqualityComparer<T> a_comparer)
//        {
//            List<T> list = new List<T>(a_enumerable);

//            if (a_values.FirstOrDefault() == null)
//                return false;

//            foreach (T ele in a_values)
//            {
//                int index = list.IndexOf(ele, a_comparer);
//                if (index == -1)
//                    return false;
//                else
//                    list.RemoveAt(index);
//            }

//            return true;
//        }

//        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> a_enum)
//        {
//            return a_enum.SelectMany(obj => obj);
//        }

//        public static IEnumerable<T> TakeAllOrOne<T>(this IEnumerable<T> a_enum, bool a_all, Func<T, bool> a_take)
//        {
//            bool first = false;

//            foreach (var el in a_enum)
//            {
//                if (first && !a_all)
//                    yield break;

//                if (!a_take(el))
//                    continue;

//                first = true;
//                yield return el;
//            }
//        }
//    }
//}
