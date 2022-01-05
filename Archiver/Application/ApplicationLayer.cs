using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Extensions.Conventions;
using Archiver.Domain.Models.File;
using Archiver.Infrastructure;
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
            var fHandler = new FileHandler(pathFrom, pathTo);
            try
            {
                var rightImplementation = archivesDictionary[algName];
                foreach (var bytes in fHandler.TryReadAllBytesInPortions())
                {
                    var algExtension = rightImplementation.AlgorithmExtension;
                    var initExtension = FileHandler.GetFileExtensionFromPath(pathFrom);
                    var tuple = rightImplementation.CompressData(bytes);
                    var accessoryData = ToRightFormatConverter.ConvertAccessoryDictToByteArray(rightImplementation.AccessoryData);
                    new FileSmart(initExtension, algExtension, tuple, accessoryData).WriteSmartFile(fHandler);
                }
            }
            catch (Exception ex)
            {
                // текст ошибки будем пробрасывать в окно формы
            }
        }

        public void Decompress(string pathFrom, string pathTo)
        { 
            var fHandler = new FileHandler(pathFrom, pathTo);
            try
            {
                fHandler.TryWriteBytesInPortions(DecompressedDataInPortions(fHandler));
            }
            catch (Exception ex)
            {
                // текст ошибки будем пробрасывать в окно формы
            }
        }

        private IEnumerable<byte[]> DecompressedDataInPortions(FileHandler fHandler)
        {
            foreach (var bytes in fHandler.TryReadCompressedDataBytes(algotihmsExtensionsDict))
            {
                var fSmart = new FileSmart(bytes);
                var rightImplementation = archivesDictionary[algotihmsExtensionsDict[fSmart.algExtension]];
                var accessoryData = ToRightFormatConverter.ConvertAccessoryDataToDictionary(fSmart.accecoryData);
                yield return rightImplementation.DecompressData(fSmart.compressedData, accessoryData);
            }
        }

        private Dictionary<string, IArchiverBase> archivesDictionary = new Dictionary<string, IArchiverBase>();
        private Dictionary<string, string> algotihmsExtensionsDict = new Dictionary<string, string>();
    }
}
