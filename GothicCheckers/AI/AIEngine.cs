using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GothicCheckers.AI
{
    public abstract class AIEngine : DispatcherObject, IAIEngine
    {
        private bool _disposed;
        private IMove _bestMove;
        private Task<IMove> _task;

        protected CancellationTokenSource CTS { get; set; }

        public IMove BestMove
        {
            get
            {
                VerifyAccess();
                return _bestMove;
            }
            private set
            {
                _bestMove = value;
            }
        }

        public event EventHandler BestMoveChosen;

        public void ComputeBestMove(GameBoard board, PlayerColor player, int depth)
        {
            VerifyAccess();
            Reset();
            CTS = new CancellationTokenSource();
            AIState state = new AIState(board, player, depth);
            _task = new Task<IMove>(GetBestMove, state, CTS.Token);
            _task.ContinueWith(task =>
                {
                    BestMove = task.Result;
                    Dispatcher.BeginInvoke((Action)OnBestMoveChosen, null);
                });
            _task.Start();
        }

        public void StopThinking()
        {
            VerifyAccess();
            if (CTS != null) CTS.Cancel();
        }

        public void Dispose()
        {
            VerifyAccess();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Reset();
                }

                _disposed = true;
            }
        }

        protected virtual void OnBestMoveChosen()
        {
            if (BestMoveChosen != null) BestMoveChosen(this, EventArgs.Empty);
        }

        protected void Reset()
        {
            if (_task != null)
            {
                if (_task.Status == TaskStatus.Running)
                {
                    CTS.Cancel(true);
                    while (_task.Status == TaskStatus.Running) ;
                }

                _task.Dispose();
            }

            if (CTS != null) CTS.Dispose();

            CTS = null;
            _task = null;
        }

        protected abstract IMove GetBestMove(object oState);
    }
}
