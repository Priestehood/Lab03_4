using System;
using System.ComponentModel;
using System.Windows.Forms;
using Lab04_4.MyForms.SpatialQuery.Forms;


namespace Lab04_4.MyForms.SpatialQuery.Services
{
    public delegate bool DoStepDelegate();
    public delegate void OnCompletedDeletegate(RunWorkerCompletedEventArgs e);

    class LongOperation
    {
        private BackgroundWorker _worker;
        private ProgressForm _progressForm;
        private DoStepDelegate DoStep;
        private OnCompletedDeletegate OnCompleted;
        private int totalSteps;

        public LongOperation(int totalSteps, DoStepDelegate doStep, OnCompletedDeletegate onCompleted)
        {
            this.totalSteps = totalSteps;
            this.DoStep = doStep;
            this.OnCompleted = onCompleted;
        }

        public void Start(string title = "进度")
        {
            // 创建进度窗口
            _progressForm = new ProgressForm(title);
            _progressForm.CancelRequested += (s, e) => _worker.CancelAsync();
            _progressForm.Show();

            // 配置BackgroundWorker
            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int currentStep = 0; ; currentStep++)
            {
                // 检查取消请求
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                // 执行步进操作
                bool result = DoStep();
                if (result is true) return;

                // 计算进度
                double progress = (double)currentStep / totalSteps;
                int progressPercentage = (int)Math.Floor(progress * 100);
                if (progressPercentage >= 100) progressPercentage = 100;

                // 报告进度
                string status = $"正在处理中：{Math.Min(currentStep + 1,totalSteps)}/{totalSteps}";
                _worker.ReportProgress(progressPercentage, status);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progressForm.UpdateProgress(e.ProgressPercentage,
                (string)e.UserState);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressForm.Close();
            OnCompleted(e);
        }
    }
}
