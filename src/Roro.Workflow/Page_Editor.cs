using System;
using System.Collections.Generic;
using System.Linq;

namespace Roro.Workflow
{
    public partial class Page
    {
        private Stack<string> _undoStack { get; } = new Stack<string>();

        private Stack<string> _redoStack { get; } = new Stack<string>();

        public bool CanUndo => this._undoStack.Count() > 0;

        public bool CanRedo => this._redoStack.Count() > 0;

        public void CancelPendingChanges()
        {
            if (this.CanUndo)
            {
                var xmlNodes = this._undoStack.Peek();
                var objNodes = XmlSerializerHelper.ToObject<Node[]>(xmlNodes);

                this.Nodes.Clear();
                objNodes.ToList().ForEach(x => this.Nodes.Add(x));
            }
        }

        public void CommitPendingChanges()
        {
            var xmlNodes = XmlSerializerHelper.ToString(this.Nodes.ToArray());
            if (!this.CanUndo || xmlNodes != this._undoStack.Peek())
            {
                this._undoStack.Push(xmlNodes);
                this._redoStack.Clear();
            }
        }

        public void AddNode(Node node)
        {
            if (node is StartNode startNode)
            {
                this.Nodes.Where(x => x is StartNode && x != startNode).ToList()
                    .ForEach(x => this.Nodes.Remove(x));
            }
            if (node is LoopStartNode loopStartNode)
            {
                var loopEndNode = new LoopEndNode();
                loopEndNode.LoopStart.To = loopStartNode.Id;
                loopStartNode.LoopEnd.To = loopEndNode.Id;
                loopEndNode.Rect = loopStartNode.Rect;
                loopEndNode.SetPosition(loopStartNode.Rect.X, loopStartNode.Rect.Y + 12 * Page.GRID_SIZE);
                this.AddNode(loopEndNode);
            }
        }

        public void RemoveNode(Node node)
        {
            if (node is StartNode && this.Nodes.Count(x => x is StartNode) == 1)
            {
                return;
            }
            this.Nodes.Remove(node);
        }

        public string Cut()
        {
            var xmlNodes = this.Copy();
            this.Delete();
            return xmlNodes;
        }

        public string Copy()
        {
            this.Nodes.Where(x => x is LoopStartNode).Cast<LoopStartNode>().ToList()
                .ForEach(ls => this.Nodes.Where(x => x.Id == ls.LoopEnd.To).ToList()
                    .ForEach(le => le.Selected = true));

            this.Nodes.Where(x => x is LoopEndNode).Cast<LoopEndNode>().ToList()
                .ForEach(le => this.Nodes.Where(x => x.Id == le.LoopStart.To).ToList()
                    .ForEach(ls => ls.Selected = true));

            var objNodes = this.SelectedNodes.ToArray();
            var xmlNodes = XmlSerializerHelper.ToString(objNodes);
            if (objNodes.Count() > 0)
            {
                return xmlNodes;
            }

            return string.Empty;
        }

        public void Paste(string xmlNodes)
        {
            try
            {
                var objNodes = XmlSerializerHelper.ToObject<Node[]>(xmlNodes);
                var objNodeIds = objNodes.ToDictionary(x => x.Id); // store node.Id

                objNodes.ToList().ForEach(x => x.Id = Guid.NewGuid()); // assign new node.Id

                objNodes.SelectMany(x => x.Ports).ToList() // assign new node.Id to port.To
                    .ForEach(p => p.To = objNodeIds.TryGetValue(p.To, out Node n) ? n.Id : Guid.Empty);

                if (objNodes.Count() > 0)
                {
                    this.CommitPendingChanges();
                    objNodes.ToList().ForEach(x => this.Nodes.Add(x));
                    this.CommitPendingChanges();
                }
            }
            catch (Exception ex)
            {
                var error = ex;
            }
        }

        public void Delete()
        {
            this.Nodes.Where(x => x is StartNode).ToList()
                .ForEach(x => x.Selected = false);

            this.Nodes.Where(x => x is LoopStartNode).Cast<LoopStartNode>().ToList()
                .ForEach(ls => this.Nodes.Where(x => x.Id == ls.LoopEnd.To).ToList()
                    .ForEach(le => le.Selected = true));

            this.Nodes.Where(x => x is LoopEndNode).Cast<LoopEndNode>().ToList()
                .ForEach(le => this.Nodes.Where(x => x.Id == le.LoopStart.To).ToList()
                    .ForEach(ls => ls.Selected = true));

            if (this.SelectedNodes.Count() > 0)
            {
                this.CommitPendingChanges();
                this.SelectedNodes.ToList().ForEach(x => this.Nodes.Remove(x));
                this.CommitPendingChanges();
            }
        }

        public void Undo()
        {
            if (this.CanUndo)
            {
                var xmlPeekNodes = this._undoStack.Peek();
                var xmlThisNodes = XmlSerializerHelper.ToString(this.Nodes.ToArray());

                if (xmlThisNodes == xmlPeekNodes) // nothing changed since the last commit.
                {
                    this._undoStack.Pop();
                    this.Undo();
                    return;
                }

                var objNodes = XmlSerializerHelper.ToObject<Node[]>(this._undoStack.Pop());
                var xmlNodes = XmlSerializerHelper.ToString(this.Nodes.ToArray());
                this._redoStack.Push(xmlNodes);

                this.Nodes.Clear();
                objNodes.ToList().ForEach(x => this.Nodes.Add(x));
            }
        }

        public void Redo()
        {
            if (this.CanRedo)
            {
                var objNodes = XmlSerializerHelper.ToObject<Node[]>(this._redoStack.Pop());
                var xmlNodes = XmlSerializerHelper.ToString(this.Nodes.ToArray());
                this._undoStack.Push(xmlNodes);

                this.Nodes.Clear();
                objNodes.ToList().ForEach(x => this.Nodes.Add(x));
            }
        }
    }
}
