namespace OrganisationRegistry.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public interface INodeValue
    {
        string Id { get; }
    }

    public static class NodeValueListExtension
    {
        public static string ToSeparatedList<T>(
            this IEnumerable<T> list,
            string separator = "|",
            Func<T, string>? valueFunc = null) where T : INodeValue
        {
            valueFunc ??= x => x.Id;

            return ToSeparatedListExtension.ToSeparatedList(list, separator, valueFunc);
        }
    }

    public interface INode<T> where T : INodeValue
    {
        string Id { get; }
        T Value { get; }
        INode<T>? Parent { get; }
        IList<INode<T>> Children { get; }

        bool Dirty { get; }

        void ChangeParent(INode<T> newParentNode);
        void RemoveParent();

        void AcceptChanges();

        IEnumerable<T> Traverse();
    }

    public class Node<T> : INode<T> where T : INodeValue
    {
        private readonly T _value;

        public string Id => _value.Id;
        public T Value => _value;

        [JsonIgnore]
        public INode<T>? Parent { get; private set; }

        public IList<INode<T>> Children { get; }
        public bool Dirty { get; private set; }

        public Node(T value, INode<T>? parent = null)
        {
            _value = value;
            Parent = parent;
            Children = new List<INode<T>>();

            // New nodes are always dirty
            MarkDirty(this);

            // If there is a parent, we join it's children
            parent?.Children.Add(this);
        }

        public void ChangeParent(INode<T> newParentNode)
        {
            if (newParentNode == null)
                throw new ArgumentNullException(nameof(newParentNode), "Parent node cannot be null.");

            MarkDirty(this);
            MarkDirty(newParentNode);

            Parent?.Children.Remove(this);
            newParentNode.Children.Add(this);

            Parent = newParentNode;
        }

        public void RemoveParent()
        {
            if (Parent == null)
                return;

            MarkDirty(this);

            Parent.Children.Remove(this);
            Parent = null;
        }

        public void AcceptChanges()
        {
            Dirty = false;
        }

        private static void MarkDirty(INode<T> node)
        {
            if (node is Node<T> dirtyNode)
                dirtyNode.Dirty = true;

            MarkParentsDirty(node);
        }

        private static void MarkParentsDirty(INode<T> node)
        {
            // If this node has a parent, it becomes dirty, as well as all parents above
            var parent = node.Parent;
            while (parent != null)
            {
                MarkDirty(parent);
                parent = parent.Parent;
            }
        }

        public IEnumerable<T> Traverse()
        {
            var stack = new Stack<INode<T>>();
            stack.Push(this);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current.Value;
                foreach (var child in current.Children.OrderBy(x => x.Id))
                    stack.Push(child);
            }
        }
    }

    public interface ITree<T> where T : INodeValue
    {
        void AddNode(T value);
        void AddNode(T value, T parent);

        void ChangeNodeParent(T value, T newParentValue);

        void RemoveNodeParent(T value);

        IEnumerable<INode<T>> GetChanges();
        void AcceptChanges();

        IDictionary<string, string> DrawTree();
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Tree<T> : ITree<T> where T : INodeValue
    {
        [JsonProperty]
        private readonly IDictionary<string, INode<T>> _tree = new Dictionary<string, INode<T>>();

        public void AddNode(T value)
        {
            var node = new Node<T>(value);
            _tree.Add(node.Id, node);
        }

        public void AddNode(T value, T parent)
        {
            var parentNode = _tree[parent.Id];
            var node = new Node<T>(value, parentNode);
            _tree.Add(node.Id, node);
        }

        public void ChangeNodeParent(T value, T newParentValue)
        {
            var node = _tree[value.Id];
            var newParentNode = _tree[newParentValue.Id];

            node.ChangeParent(newParentNode);
        }

        public void RemoveNodeParent(T value)
        {
            var node = _tree[value.Id];
            node.RemoveParent();
        }

        public IEnumerable<INode<T>> GetChanges()
        {
            return _tree.Values.Where(x => x.Dirty).OrderBy(x => x.Id);
        }

        public void AcceptChanges()
        {
            var changes = GetChanges();
            foreach (var change in changes)
                change.AcceptChanges();
        }

        public IDictionary<string, string> DrawTree()
        {
            return _tree.Values.ToDictionary(x => x.Id, x => x.Traverse().ToSeparatedList());
        }
    }
}
