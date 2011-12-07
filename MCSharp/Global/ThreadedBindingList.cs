using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace MCSharp
{
    public class ThreadedBindingList<T> : BindingList<T>
    {
        private readonly SynchronizationContext ctx;

        public ThreadedBindingList ()
        {
            ctx = SynchronizationContext.Current;
        }


        protected override void OnAddingNew (AddingNewEventArgs e)
        {
            SynchronizationContext ctx = SynchronizationContext.Current;
            if (ctx == null)
            {
                BaseAddingNew(e);
            }
            else
            {
                ctx.Send(delegate
                {
                    BaseAddingNew(e);
                }, null);
            }
        }


        void BaseAddingNew (AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }


        protected override void OnListChanged (ListChangedEventArgs e)
        {
            if (ctx == null)
            {
                BaseListChanged(e);
            }
            else
            {
                ctx.Send(delegate
                {
                    BaseListChanged(e);
                }, null);
            }
        }


        void BaseListChanged (ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }
    }
}
