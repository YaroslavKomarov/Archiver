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
                var algExtension = rightImplementation.AlgorithmExtension;
                var initExtension = FileHandler.GetFileExtensionFromPath(pathFrom);
                var compressedData = CompressedDataInPortions(rightImplementation, fHandler);
                var accessoryData = rightImplementation.AccessoryData;
                new FileSmart(initExtension, algExtension, compressedData, accessoryData).WriteSmartFile(fHandler);
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
                fHandler.CheckPathExtension(algotihmsExtensionsDict);
                fHandler.TryWriteBytesInPortions(DecompressedDataInPortions(fHandler));
            }
            catch (Exception ex)
            {
                // текст ошибки будем пробрасывать в окно формы
            }
        }

        private IEnumerable<byte[]> CompressedDataInPortions(IArchiverBase rightImplementation, FileHandler fHandler)
        {
            foreach (var compressedBytes in rightImplementation.CompressData(fHandler.TryReadBytesInPortions()))
                yield return compressedBytes;
        }

        private IEnumerable<byte[]> DecompressedDataInPortions(FileHandler fHandler)
        {
            var fSmart = new FileSmart(fHandler.PathFrom);
            var algExtension = ToRightFormatConverter.GetStringFromBytes(fSmart.algExtensionBytes);
            var rightImplementation = archivesDictionary[algotihmsExtensionsDict[algExtension]];
            rightImplementation.AccessoryData = ToRightFormatConverter.ConvertAccessoryDataToDictionary(fSmart.accecoryData);
            foreach (var decompressedBytes in rightImplementation.DecompressData(fSmart.compressedData))
                yield return decompressedBytes;
        }

        private Dictionary<string, IArchiverBase> archivesDictionary = new Dictionary<string, IArchiverBase>();
        private Dictionary<string, string> algotihmsExtensionsDict = new Dictionary<string, string>();
    }
}