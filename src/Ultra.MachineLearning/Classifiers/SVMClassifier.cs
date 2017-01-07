using System;
using System.Collections.Generic;
using Ultra.Contract;
using Ultra.Contract.Model;
using OpenCvSharp;
using OpenCvSharp.ML;

namespace Ultra.MachineLearning.Classifiers
{
    public class SVMClassifier : IClassifier
    {
        private readonly SVM _svm;

        private SVMClassifier(SVM svm)
        {
            _svm = svm;
        }

        public bool Predict(ImageFeatures imageFeatures)
        {
            return Convert.ToBoolean(_svm.Predict(imageFeatures.ToPredictionMat()));
        }


        public static SVMClassifier Create(List<ImageFeatures> trainingData)
        {
            var svm = SVM.Create();
            svm.Type = SVM.Types.CSvc;
            svm.KernelType = SVM.KernelTypes.Linear;
            svm.TermCriteria = TermCriteria.Both(maxCount: 1000, epsilon: 0.000001);
            svm.Gamma = 100.0;
            svm.C = 1.0;

            svm.Train(trainingData.ToTrainingMat(), SampleTypes.RowSample, trainingData.ToResponseMat());

            return new SVMClassifier(svm);
        }
    }
}