using System;
using System.Collections.Generic;
using System.Linq;

namespace Roro.Workflow
{
    public partial class Page
    {
        public const int GRID_SIZE = 20;

        public const string MAIN_PAGE_NAME = "Main";

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

                this._nodes.Clear();
                objNodes.ToList().ForEach(x => this._nodes.Add(x));
            }
        }

        public void CommitPendingChanges()
        {
            var xmlNodes = XmlSerializerHelper.ToString(this._nodes.ToArray());
            if (!this.CanUndo || xmlNodes != this._undoStack.Peek())
            {
                this._undoStack.Push(xmlNodes);
                this._redoStack.Clear();
            }
        }

        public void Add(IEditableNode node)
        {
            if (node is StartNode)
            {
                this._nodes.Where(x => x is StartNode).ToList().ForEach(x => this._nodes.Remove(x));
            }
            if (node is LoopStartNode loopStartNode)
            {
                var loopEndNode = new LoopEndNode();
                loopEndNode.LoopStart.To = loopStartNode.Id;
                loopStartNode.LoopEnd.To = loopEndNode.Id;
                loopEndNode.Rect = loopStartNode.Rect;
                loopEndNode.SetLocation(loopStartNode.Rect.X, loopStartNode.Rect.Y + 12 * Page.GRID_SIZE);
                this._nodes.Add(loopEndNode);
            }
            this._nodes.Add((Node)node);
        }

        public string Cut()
        {
            var xmlNodes = this.Copy();
            this.Delete();
            return xmlNodes;
        }

        public string Copy()
        {
            this._nodes.Where(x => x is LoopStartNode).Cast<LoopStartNode>().ToList()
                .ForEach(ls => this._nodes.Where(x => x.Id == ls.LoopEnd.To).ToList()
                    .ForEach(le => le.Selected = true));

            this._nodes.Where(x => x is LoopEndNode).Cast<LoopEndNode>().ToList()
                .ForEach(le => this._nodes.Where(x => x.Id == le.LoopStart.To).ToList()
                    .ForEach(ls => ls.Selected = true));

            var objNodes = this.SelectedNodes.Cast<Node>().ToArray();
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

                objNodes.SelectMany(x => x.Ports).Cast<Port>().ToList() // assign ehe new node.Id to port.To
                    .ForEach(p => p.To = objNodeIds.TryGetValue(p.To, out Node n) ? n.Id : Guid.Empty);

                if (objNodes.Count() > 0)
                {
                    this.CommitPendingChanges();
                    if (objNodes.FirstOrDefault(x => x is StartNode) is StartNode)
                    {
                        this._nodes.Remove((Node)this.StartNode);
                    }
                    objNodes.ToList().ForEach(x => this._nodes.Add(x));
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
            this._nodes.Where(x => x is StartNode).ToList()
                .ForEach(x => x.Selected = false);

            this._nodes.Where(x => x is LoopStartNode).Cast<LoopStartNode>().ToList()
                .ForEach(ls => this._nodes.Where(x => x.Id == ls.LoopEnd.To).ToList()
                    .ForEach(le => le.Selected = true));

            this._nodes.Where(x => x is LoopEndNode).Cast<LoopEndNode>().ToList()
                .ForEach(le => this._nodes.Where(x => x.Id == le.LoopStart.To).ToList()
                    .ForEach(ls => ls.Selected = true));

            if (this.SelectedNodes.Count() > 0)
            {
                this.CommitPendingChanges();
                this.SelectedNodes.Cast<Node>().ToList().ForEach(x => this._nodes.Remove(x));
                this.CommitPendingChanges();
            }
        }

        public void Undo()
        {
            if (this.CanUndo)
            {
                var xmlPeekNodes = this._undoStack.Peek();
                var xmlThisNodes = XmlSerializerHelper.ToString(this._nodes.ToArray());

                if (xmlThisNodes == xmlPeekNodes) // nothing changed since the last commit.
                {
                    this._undoStack.Pop();
                    this.Undo();
                    return;
                }

                var objNodes = XmlSerializerHelper.ToObject<Node[]>(this._undoStack.Pop());
                var xmlNodes = XmlSerializerHelper.ToString(this._nodes.ToArray());
                this._redoStack.Push(xmlNodes);

                this._nodes.Clear();
                objNodes.ToList().ForEach(x => this._nodes.Add(x));
            }
        }

        public void Redo()
        {
            if (this.CanRedo)
            {
                var objNodes = XmlSerializerHelper.ToObject<Node[]>(this._redoStack.Pop());
                var xmlNodes = XmlSerializerHelper.ToString(this._nodes.ToArray());
                this._undoStack.Push(xmlNodes);

                this._nodes.Clear();
                objNodes.ToList().ForEach(x => this._nodes.Add(x));
            }
        }
    }
}
