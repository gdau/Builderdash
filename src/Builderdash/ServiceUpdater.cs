using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Synoptic.Service;

namespace Builderdash
{
    public class ServiceUpdater
    {
        private readonly string _sourceDirectory;
        private readonly string _destinationDirectory;
        private readonly string _tempDirectory;
        private readonly string _serviceName;
        private readonly string _exeFileName;

        public ServiceUpdater(
            string sourceDirectory,
            string destinationDirectory,
            string serviceName)
        {
            _sourceDirectory = sourceDirectory;
            _destinationDirectory = destinationDirectory;
            _tempDirectory = Path.Combine(_destinationDirectory, "__update");
            _serviceName = serviceName;

            _exeFileName = AppDomain.CurrentDomain.FriendlyName;
        }

        public void InitializeUpdate()
        {
            CloneDestinationToTemp();
            Process.Start(new ProcessStartInfo(Path.Combine(_tempDirectory, _exeFileName)) { Arguments = "update" });
        }

        public void Update()
        {
            Trace.WriteLine("Stopping service..");

            var serviceControllerify = new SafeServiceController();
            serviceControllerify.Stop(_serviceName);

            Trace.WriteLine("Stopped");

            Thread.Sleep(1000);

            UpdateDestination();

            Trace.WriteLine("Starting service..");
            serviceControllerify.Start(_serviceName);
            Trace.WriteLine("Started");
        }

        private void CloneDestinationToTemp()
        {
            if (!Directory.Exists(_tempDirectory))
                Directory.CreateDirectory(_tempDirectory);

            foreach (var file in Directory.GetFiles(_destinationDirectory))
            {
                File.Copy(file, Path.Combine(_tempDirectory, new FileInfo(file).Name), true);
            }
        }

        private void UpdateDestination()
        {
            foreach (var file in Directory.GetFiles(_sourceDirectory))
            {
                Trace.WriteLine(String.Format("from {0} to {1}", file, Path.Combine(_destinationDirectory, new FileInfo(file).Name)));
                File.Copy(file, Path.Combine(_destinationDirectory, new FileInfo(file).Name), true);
            }
        }
    }
}