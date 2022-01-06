using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using Archiver.Domain.Models.File;
using Archiver.Domain.Interfaces;

namespace Archiver.Application
{
    public class ApplicationLayer
    {
        public ApplicationLayer()
        {
            var kernel = new StandardKernel();
            kernel.Bind(x =>
                x.FromThisAssembly()
                .SelectAllClasses()
                .InheritedFrom<IArchiverBase>()
                .BindAllInterfaces());

            var archievers = kernel.GetAll<IArchiverBase>().ToList();

            foreach (var e in archievers)
            {
                archivesDictionary.Add(e.GetType().Name, e);
                algotihmsExtensionsDict.Add(e.AlgorithmExtension, e.GetType().Name);
            }
        }

        public void Compress(string algName, string pathFrom, string pathTo)
        {
            try
            {
                var rightImplementation = archivesDictionary[algName];
                var accessoryData = rightImplementation.AccessoryData;
                var fHandler = new FileHandler(pathFrom, pathTo, rightImplementation.AlgorithmExtension, true);
                var initExtension = FileHandler.GetFileExtensionFromPath(pathFrom);
                new ArchiveFile(initExtension, accessoryData).WriteArchiveFile(fHandler, rightImplementation);
                fHandler.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Decompress(string pathFrom, string pathTo)
        {
            try
            {
                FileHandler.CheckPathExtension(pathFrom, algotihmsExtensionsDict);
                var algExtension = FileHandler.GetFileExtensionFromPath(pathFrom);
                var fHandler = new FileHandler(pathFrom, pathTo, algExtension, false);
                var rightImplementation = archivesDictionary[algotihmsExtensionsDict[algExtension]];
                ArchiveFile.WriteInitFile(fHandler, rightImplementation);
                fHandler.Dispose();
            }
            catch (Exception ex)
            {
                // текст ошибки будем пробрасывать в окно формы
            }
        }

        private Dictionary<string, IArchiverBase> archivesDictionary = new Dictionary<string, IArchiverBase>();
        private Dictionary<string, string> algotihmsExtensionsDict = new Dictionary<string, string>();
    }
}