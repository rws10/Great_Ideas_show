using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdeaSite.Models
{
    public class IEAttachments : IEnumerable
    {
        private Attachment[] _IEAttachments;
        public IEAttachments(Attachment[] pArray)
        {
            _IEAttachments = new Attachment[pArray.Length];

            for (int i = 0; i < pArray.Length; i++)
            {
                _IEAttachments[i] = pArray[i];
            }
        }

        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public IEAttachmentsEnum GetEnumerator()
        {
            return new IEAttachmentsEnum(_IEAttachments);
        }
    }

    // When you implement IEnumerable, you must also implement IEnumerator.
    public class IEAttachmentsEnum : IEnumerator
    {
        public Attachment[] _IEAttachments;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public IEAttachmentsEnum(Attachment[] list)
        {
            _IEAttachments = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _IEAttachments.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Attachment Current
        {
            get
            {
                try
                {
                    return _IEAttachments[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}