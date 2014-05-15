using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class EntitiesHelper
    {

        /// <summary>
        /// Convert ienumerable to WPF-typical observable collection
        /// </summary>
        /// <typeparam name="T">Type of IEnumerable</typeparam>
        /// <param name="enumeration"></param>
        /// <returns></returns>
        
        public static ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> enumeration)
        {
            return new ObservableCollection<T>(enumeration);
        }


        /// <summary>
        /// Calculate next free highest id of an ienumerable
        /// </summary>
        /// <typeparam name="T">Type of IEnumerable</typeparam>
        /// <param name="items">IEnumerable of items</param>
        /// <param name="lambdaExpression">Expression like: item => item.Id</param>
        /// <returns></returns>
        
        public static int CalcNextId<T>(IEnumerable<T> items, Func<T, int> lambdaExpression)
        {
            if (items != null && items.Any())
            {
                return items.Max(lambdaExpression) + 1;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// Remove multiple items from ObservableCollection
        /// </summary>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <param name="collection">Collection with items</param>
        /// <param name="condition">Conditions which items to remove</param>
        /// <see cref="https://stackoverflow.com/questions/5118513/removeall-for-observablecollections"/>
        public static void RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> condition)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (condition(collection[i]))
                {
                    collection.RemoveAt(i);
                }
            }
        }
    }
}
