using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarPosCommands
{
    public class EntityCollection : IEnumerable<Entity>, IList<Entity>
    {
        List<Entity> items = new List<Entity>();

        public EntityCollection()
        {
            items = new List<Entity>();
        }

        public EntityCollection(IEnumerable<Entity> collection)
        {
            items = new List<Entity>(collection);
        }

        public Extents3d? Bounds
        {
            get
            {
                bool hasExtents = false;
                Extents3d ex = new Extents3d();
                foreach (Entity e in items)
                {
                    Extents3d? itemEx = e.Bounds;
                    if (itemEx.HasValue)
                    {
                        hasExtents = true;
                        ex.AddExtents(itemEx.Value);
                    }
                }
                if (!hasExtents)
                    return null;
                else
                    return ex;
            }
        }

        public void TransformBy(Matrix3d transform)
        {
            foreach (Entity e in items)
            {
                e.TransformBy(transform);
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(Entity item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, Entity item)
        {
            items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public Entity this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        public void Add(Entity item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            foreach (Entity e in items)
            {
                e.Dispose();
            }
            items.Clear();
        }

        public bool Contains(Entity item)
        {
            return items.Contains(item);
        }

        public void CopyTo(Entity[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Entity item)
        {
            return items.Remove(item);
        }
    }
}
