using System;
using System.Collections.Generic;
using POSMini.Service.Observer;

namespace SPOSMini.Service.Observer
{
    public class ThanhToanSubject : ISubject
    {
        private readonly List<IObserver> observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }
    }
}