// ***********************************************************************
// Assembly         : ScriptControl
// Author           : chou
// Created          : 03-31-2016
//
// Last Modified By : chou
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="ObjectPool.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    /// <summary>
    /// Class ObjectPool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T>
    {
        /// <summary>
        /// The _objects
        /// </summary>
        private ConcurrentBag<T> _objects;
        /// <summary>
        /// The _object generator
        /// </summary>
        private Func<T> _objectGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="objectGenerator">The object generator.</param>
        /// <exception cref="ArgumentNullException">objectGenerator</exception>
        public ObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <returns>T.</returns>
        public T GetObject()
        {

            T item;
            if (_objects.TryTake(out item)) return item;
            Console.WriteLine(_objects.Count());
            return _objectGenerator();
        }

        /// <summary>
        /// Puts the object.
        /// </summary>
        /// <param name="item">The item.</param>
        public void PutObject(T item)
        {
            if (item == null)
                return;
            if (item is List<ValueRead>)
                (item as List<ValueRead>).Clear();
            else if (item is List<ValueWrite>)
                (item as List<ValueWrite>).Clear();
            else if (item is Dictionary<string, string>)
                (item as Dictionary<string, string>).Clear();
            else if (item is StringBuilder)
                (item as StringBuilder).Clear();
            else if (item is Stopwatch)
            {
                Stopwatch sw = item as Stopwatch;
                if (sw.IsRunning) sw.Stop();
                sw.Reset();
            }
            else if (item is AVEHICLE)
            {
                (item as AVEHICLE).initialParameter();
            }
            else if (item is PLC_FunBase)
                (item as PLC_FunBase).reset();
            else if (item is LogObj)
            {
                (item as LogObj).reset();
            }

            _objects.Add(item);
        }

        /// <summary>
        /// Counts all objects.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int countAllObjects()
        {
            return _objects.Count();
        }
    }
}