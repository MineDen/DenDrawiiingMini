using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing
{
    public static class History
    {
        public static HistoryStack<Bitmap> undoHistory = new HistoryStack<Bitmap>();
        public static HistoryStack<Bitmap> redoHistory = new HistoryStack<Bitmap>();


        public static void Action(Bitmap bitmap)
        {
            redoHistory.Clear();
            undoHistory.Push(bitmap);
            if (undoHistory.Count == 21)
                undoHistory.Remove(0);
        }

        public static void Undo(Graphics restoreTo, Bitmap bitmap)
        {
            Bitmap act = undoHistory.Pop();
            redoHistory.Push(bitmap);
            restoreTo.Clear(Color.Transparent);
            restoreTo.DrawImage(act, 0, 0);
        }

        public static void Redo(Graphics restoreTo, Bitmap bitmap)
        {
            Bitmap act = redoHistory.Pop();
            undoHistory.Push(bitmap);
            restoreTo.Clear(Color.Transparent);
            restoreTo.DrawImage(act, 0, 0);
        }
    }

    public class HistoryStack<T>
    {
        private List<T> items = new List<T>();
        public int Count { get => items.Count; }

        public void Push(T item)
        {
            items.Add(item);
        }

        public T Pop()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return temp;
            }
            else
                return default(T);
        }

        public void Remove(int itemAtPosition)
        {
            items.RemoveAt(itemAtPosition);
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}