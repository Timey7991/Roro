using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed class Flow : NotifyPropertyHelper, IEditableFlow
    {
        [XmlAttribute]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name
        {
            get => this._name;
            set => this.OnPropertyChanged(ref this._name, value);
        }
        private string _name;

        [XmlArray("Pages")]
        public ObservableCollection<Page> _pages { get; } = new ObservableCollection<Page>();

        public IEnumerable<IEditablePage> Pages => this._pages;

        [XmlIgnore]
        public IEditablePage SelectedPage
        {
            get => this._selectedPage ?? this.MainPage;
            set => this.OnPropertyChanged(ref this._selectedPage, value);
        }
        private IEditablePage _selectedPage;

        public Page MainPage => this._pages.First(x => x.Name == Page.MAIN_PAGE_NAME);

        public ObservableCollection<NodeExecutionResult> LogEvents { get; } = new ObservableCollection<NodeExecutionResult>();

        private Flow()
        {
            this._pages.CollectionChanged += Pages_CollectionChanged;
        }

        public Flow(string name) : this()
        {
            this.Name = name;
            this._pages.Add(new Page(Page.MAIN_PAGE_NAME));
        }

        private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Page addedPage in e.NewItems)
                    {
                        addedPage.ParentFlow = this;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Page removedPage in e.OldItems)
                    {
                        removedPage.ParentFlow = null;
                    }
                    break;
            }
        }

        public void AddPage(IEditablePage page)
        {
            this._pages.Add((Page)page);
        }

        public void RemovePage(IEditablePage page)
        {
            if (page == this.MainPage)
            {
                return;
            }
            this._pages.Remove((Page)page);
        }

        public override string ToString()
        {
            return XmlSerializerHelper.ToString(this);
        }

        public static Flow Load(string xmlFlow)
        {
            return XmlSerializerHelper.ToObject<Flow>(xmlFlow);
        }

        #region FLOW RUNNER

        [XmlIgnore]
        public FlowState State
        {
            get => this._state;
            set => this.OnPropertyChanged(ref this._state, value);
        }
        private FlowState _state;

        [XmlIgnore]
        public IEditablePage NextPage
        {
            get => this._nextPage;
            private set
            {
                this.OnPropertyChanged(ref this._nextPage, value);
                this.SelectedPage = this.NextPage ?? this.SelectedPage;
            }
        }
        private IEditablePage _nextPage;

        [XmlIgnore]
        public IEditableNode NextNode
        {
            get => this._nextNode;
            private set
            {
                this.OnPropertyChanged(ref this._nextNode, value);
                this.NextPage = this.NextNode?.ParentPage;
            }
        }
        private IEditableNode _nextNode;

        private CancellationTokenSource _ctsPause = new CancellationTokenSource();

        private CancellationTokenSource _ctsStop = new CancellationTokenSource();

        public void Run()
        {
            switch (this.State)
            {
                case FlowState.Idle:
                case FlowState.Paused:
                case FlowState.Stopped:
                case FlowState.Completed:
                    this._ctsPause = new CancellationTokenSource();
                    this._ctsStop = new CancellationTokenSource();
                    this.NextNode = this.NextNode ?? this.MainPage.StartNode;
                    this.State = FlowState.Running;
                    this.LogEvents.Clear();
                    this.RunNextAsync();
                    break;
            }

        }

        private void RunNextAsync()
        {
            Task.Run(() =>
            {
                this.SelectedPage = this.NextPage;
                Thread.Sleep(500);
                try
                {
                    var result = this.NextNode.Execute(null);
                    this.Dispatch(() => {
                        this.LogEvents.Add(result);
                    });
                    if (result.NextPage is Page nextPage)
                    {
                        if (nextPage.Nodes.FirstOrDefault(x => x.Id == result.NextNodeId) is Node nextNode)
                        {
                            this.NextNode = nextNode;
                            this.RunNextAsync();
                        }
                        else
                        {
                            this.State = FlowState.Paused;
                        }
                    }
                    else
                    {
                        this.State = FlowState.Completed;
                        this.NextNode = null;
                    }
                }
                catch (OperationCanceledException opex)
                {
                    if (opex.CancellationToken == this._ctsPause.Token)
                    {
                        this.State = FlowState.Paused;
                    }
                    else if (opex.CancellationToken == this._ctsStop.Token)
                    {
                        this.State = FlowState.Stopped;
                        this.NextNode = null;
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    this.State = FlowState.Failed;
                    Debug.WriteLine(ex);
                }
            });
        }

        public void Pause()
        {
            switch (this.State)
            {
                case FlowState.Running:
                    this.State = FlowState.Pausing;
                    this._ctsPause.Cancel();
                    break;
            }
        }

        public void Stop()
        {
            switch (this.State)
            {
                case FlowState.Running:
                    this.State = FlowState.Stopping;
                    this._ctsStop.Cancel();
                    break;
            }
        }

        public void Reset()
        {
            this.NextNode = null;
            this.Pages.Cast<Page>().ToList().ForEach(x => x.Reset());
            this.State = FlowState.Idle;
        }

        public delegate void Dispatcher(Action action);

        public Dispatcher Dispatch { get; set; } = (Action action) => { action.Invoke(); };

        #endregion
    }
}
